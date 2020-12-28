using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.Sound;
using RabbetGameEngine.Text;
using RabbetGameEngine.VisualEffects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RabbetGameEngine
{
    //TODO: Implement functions which are only called when game settings change. This will help optimize certain code, for example uniforms which only need to change when settings change.

    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        public static int temp = 0;
        public static readonly string entityLabelName = "entLabel";
        private static GameInstance instance;
        private static Random privateRand;
        public static int windowWidth;
        public static int windowHeight;
        public static int screenWidth;
        public static int screenHeight;
        private static Vector2 windowCenter;
        private static float dpiY;
        private static bool gamePaused = false;
        public EntityPlayer thePlayer;
        public Planet currentPlanet;

        /// <summary>
        /// Will be true if there has been atleast one onTick() call since last frame.
        /// </summary>
        private bool doneOneTick = false;

        public GameInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings windowSettings) : base(gameWindowSettings, windowSettings)
        {
            GameInstance.instance = this;
            Title = Application.applicationName;
            int iconWidth, iconHeight;
            byte[] data;
            IconLoader.getIcon("icon", out iconWidth, out iconHeight, out data);
            Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image[] { new OpenTK.Windowing.Common.Input.Image(iconWidth, iconHeight, data) });
        }

        protected override void OnLoad()
        {
            Application.infoPrint("loading.");
            try
            {
                privateRand = new Random();
                TicksAndFrames.init(30);
                ResourceUtil.init();
                GameSettings.loadSettings();
                Renderer.init();
                TextUtil.loadAllFoundTextFiles();
                SoundManager.init();
                windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
                setDPIScale();
                GUIManager.addPersistentGUI(new GUIHud());
                currentPlanet = new Planet(0xdeadbeef);
                //create and spawn player in new world
                thePlayer = new EntityPlayer(currentPlanet, "Steve", new Vector3(0, 3, 2));
                for (int i = 0; i < 65; i++)
                {
                    currentPlanet.spawnEntityInWorld(new EntityCactus(currentPlanet, new Vector3(-privateRand.Next(-26, 27), 2.5F, -privateRand.Next(-26, 27))));
                }
                currentPlanet.spawnEntityInWorld(thePlayer);

                //temp sound examples
                SoundManager.playSoundLoopingAt("waterroll", new Vector3(16, 1, 16), 0.5F);
                currentPlanet.spawnVFXInWorld(new VFXStaticText3D("waterroll", GameSettings.defaultFont, "waterroll.ogg, 50% volume", new Vector3(16, 2.5F, 16), 5.0F, Color.white));
                SoundManager.playSoundLoopingAt("waterroll_large", new Vector3(-16, 1, -16), 1.0F);
                currentPlanet.spawnVFXInWorld(new VFXStaticText3D("waterroll_large", GameSettings.defaultFont, "waterroll_large.ogg, 100% volume", new Vector3(-16, 2.5F, -16), 5.0F, Color.white));

                Input.setCursorHiddenAndGrabbed(true);
                Application.infoPrint("Initialized.");
            }
            catch (Exception e)
            {
                Application.error("Failed load game, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            base.OnLoad();
        }

        public void onError()
        {
            this.WindowState = WindowState.Normal;
        }

        public Size getGameWindowSize()
        {
            return new Size(ClientRectangle.Size.X, ClientRectangle.Size.Y);
        }

        /// <summary>
        /// Should be called after toggling gamesettings.debugscreen
        /// </summary>
        public void onToggleEntityLabels()
        {
            if (GameSettings.entityLabels)
            {
                foreach (KeyValuePair<int, Entity> e in currentPlanet.entities)
                {
                    currentPlanet.addDebugLabel(new VFXMovingText3D(e.Value, entityLabelName, GameSettings.defaultFont, "Entity: " + e.Key.ToString(), new Vector3(0, 1, 0), 2.0F, Color.white));
                }
            }
            else
            {
                foreach (VFX v in currentPlanet.vfxList)
                {
                    if (v.vfxName == entityLabelName)
                    {
                        v.ceaseToExist();
                    }
                }
            }
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Profiler.startRoot();
            base.OnRenderFrame(args);

            Input.updateInput();
            try
            {
                doneOneTick = false;
                TicksAndFrames.doOnTickUntillRealtimeSync(onTick);
                if (doneOneTick)
                {
                    //This area will be called at MAXIMUM of the tick rate. Meaning it will not be called multiple times in a laggy situation.
                    //It is called after the ticks are looped.
                    Application.updateRamUsage();
                    GUIManager.doUpdate();
                    Renderer.doWorldRenderUpdate();
                    SoundManager.onUpdate();
                }
            }
            catch (Exception e)
            {
                Application.error("Failed to run game tick, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            SoundManager.onFrame();
            thePlayer.onCameraUpdate();//do this before calling on tick to prepare camera variables
            currentPlanet.onFrame();//should be called before rendering world since this may prepare certain elements for a frame perfect render
            TicksAndFrames.updateFPS();
            GUIManager.onFrame();
            Renderer.doGUIRenderUpdate();
            Renderer.renderAll();
            Profiler.endRoot();
            Profiler.onFrame();
        }

        /*Overriding OpenTK resize function, called every time the game window is resized*/
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            windowWidth = this.ClientRectangle.Size.X;
            windowHeight = this.ClientRectangle.Size.Y;
            Renderer.onResize();
        }

        protected override void OnUnload()
        {
            if (currentPlanet != null)
                currentPlanet.onLeavingPlanet();
            Renderer.onClosing();
            SoundManager.onClosing();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            //pausing the game if the window focus changes
            pauseGame();
            base.OnFocusedChanged(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            Profiler.startSection("tickLoop");
            Profiler.startTick();
            if (Bounds.Size.X > 0 && Bounds.Size.Y > 0)
                windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
            Profiler.startSection("tickWorld");
            Profiler.startTickSection("tickWorld");
            if (!gamePaused)
                currentPlanet.onTick();
            Profiler.endCurrentTickSection();
            Profiler.endCurrentSection();
            doneOneTick = true;//do last, ensures that certain functions are only called once per tick loop
            Profiler.endTick();
            Profiler.onTick();
            Profiler.endCurrentSection();
        }

        public void onVideoSettingsChanged()
        {
            if (currentPlanet != null)
            {
                currentPlanet.onVideoSettingsChanged();
            }
            Renderer.onVideoSettingsChanged();//do last
        }

        public float getDrawDistance()
        {
            if (currentPlanet != null)
            {
                return currentPlanet.getDrawDistance();
            }
            return 1000.0F;
        }

        public void pauseGame()
        {
            if (!gamePaused)
                Input.setCursorHiddenAndGrabbed(false);
            gamePaused = true;
        }

        public void unPauseGame()
        {
            if (gamePaused)
                Input.setCursorHiddenAndGrabbed(true);
            gamePaused = false;
        }

        private void setDPIScale()
        {
            TryGetCurrentMonitorDpi(out _, out dpiY);
        }

        public static int gameWindowWidth { get => windowWidth; }
        public static int gameWindowHeight { get => windowHeight; }
        public static Vector2 gameWindowCenter { get => windowCenter; }
        public static float aspectRatio { get => (float)windowWidth / (float)windowHeight; }
        public static float dpiScale { get => (float)windowHeight / dpiY; }
        public static Random rand { get => privateRand; }

        public static int realScreenWidth { get => screenWidth; }
        public static int realScreenHeight { get => screenHeight; }

        public static bool paused { get => gamePaused; }
        public static GameInstance get { get => instance; }
    }
}
