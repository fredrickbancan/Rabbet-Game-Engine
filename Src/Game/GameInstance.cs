using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.Sound;
using RabbetGameEngine.Text;
using RabbetGameEngine.VisualEffects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RabbetGameEngine
{

    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        public static int temp = 0;
        public static readonly string entityLabelName = "entLabel";
        private static GameInstance instance;
        private static Random privateRand;
        private static int windowWidth;
        private static int windowHeight;
        private static int screenWidth;
        private static int screenHeight;
        private static Vector2 windowCenter;
        private static float dpiY;
        private static bool gamePaused = false;
        public EntityPlayer thePlayer;
        public Planet currentPlanet;

        /// <summary>
        /// Will be true if there has been atleast one onTick() call since last frame.
        /// </summary>
        private bool doneOneTick = false; 

        public unsafe GameInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings windowSettings) : base(gameWindowSettings, windowSettings)
        {
            GameInstance.instance = this;
            TicksAndFrames.init(30);
            Title = Application.applicationName;
            int iconWidth, iconHeight;
            byte[] data;
            IconLoader.getIcon("icon", out iconWidth, out iconHeight, out data);
            Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image[] { new OpenTK.Windowing.Common.Input.Image(iconWidth, iconHeight, data) });
            
            OpenTK.Windowing.Common.MonitorHandle m = CurrentMonitor;
            
            VideoMode mode = *GLFW.GetVideoMode(m.ToUnsafePtr<OpenTK.Windowing.GraphicsLibraryFramework.Monitor>());
            screenWidth = mode.Width;
            screenHeight = mode.Height;
            int hw = screenWidth / 2;
            int hh = screenHeight / 2;
            ClientRectangle = new Box2i(hw - hw / 2, hh - hh / 2, hw + hw / 2, hh + hh / 2);

            GameInstance.windowWidth = this.ClientRectangle.Size.X;
            GameInstance.windowHeight = this.ClientRectangle.Size.Y;
            Context.MakeCurrent();
        }
        
        protected override void OnLoad()
        {
                Application.infoPrint("loading.");
            try
            {
                GameInstance.privateRand = new Random();
                ResourceUtil.init();
                GameSettings.loadSettings();
                TextUtil.loadAllFoundTextFiles();
                SoundManager.init();
                windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
                setDPIScale();
                Renderer.init();
                GUIManager.addPersistentGUI(new GUIHud());
                currentPlanet = new Planet(0xdeadbeef);
                //create and spawn player in new world
                thePlayer = new EntityPlayer(currentPlanet, "Steve", new Vector3(0, 3, 2));
                for (int i = 0; i < 35; i++)
                {
                   currentPlanet.spawnEntityInWorld(new EntityCactus(currentPlanet, new Vector3(-privateRand.Next(-26, 27), 2.5F, -privateRand.Next(-26, 27))));
                }
                currentPlanet.spawnEntityInWorld(thePlayer);

                //temp sound examples
                SoundManager.playSoundLoopingAt("waterroll", new Vector3(16, 1, 16), 0.5F);
                currentPlanet.spawnVFXInWorld(new VFXStaticText3D("waterroll", GameSettings.defaultFont, "waterroll.ogg, 50% volume", new Vector3(16,2.5F,16), 5.0F, CustomColor.white));
                SoundManager.playSoundLoopingAt("waterroll_large", new Vector3(-16, 1, -16), 1.0F);
                currentPlanet.spawnVFXInWorld(new VFXStaticText3D("waterroll_large", GameSettings.defaultFont, "waterroll_large.ogg, 100% volume", new Vector3(-16,2.5F,-16), 5.0F, CustomColor.white));
                for(int i = 0; i < 5000; i++)
                {
                    currentPlanet.spawnVFXInWorld(new VFXLogoSprite3D(new Vector3(-privateRand.Next(-26, 27), 2.5F, -privateRand.Next(-26, 27)), new Vector2(5, 5)));
                }

                Input.setCursorHiddenAndGrabbed(true);
                Application.infoPrint("Initialized.");
            }
            catch(Exception e)
            {
                Application.error("Failed load game, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            base.OnLoad();
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
            if(GameSettings.entityLabels)
            {
                foreach(KeyValuePair<int, Entity> e in currentPlanet.entities)
                {
                    currentPlanet.addDebugLabel(new VFXMovingText3D(e.Value, entityLabelName, GameSettings.defaultFont, "Entity: " +  e.Key.ToString(), new Vector3(0,1,0), 2.0F, CustomColor.white));
                }
            }
            else
            {
                foreach(VFX v in currentPlanet.vfxList)
                {
                    if(v.vfxName == entityLabelName)
                    {
                        v.ceaseToExist();
                    }
                }
            }
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            Input.updateInput();
            try
            {
                doneOneTick = false;
                TicksAndFrames.doOnTickUntillRealtimeSync(onTick);
                if(doneOneTick)
                {
                    Application.updateRamUsage();
                    Renderer.doRenderUpdate();
                    GUIManager.doUpdate();
                }
            }
            catch(Exception e)
            {
                Application.error("Failed to run game tick, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            TicksAndFrames.updateFPS();
            SoundManager.onFrame();
            thePlayer.onCameraUpdate();//do this before calling on tick to prepare camera variables
            currentPlanet.onFrame();//should be called before rendering world since this may prepare certain elements for a frame perfect render
            Renderer.onFrame();
            Renderer.renderAll();
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
            Profiler.beginEndProfile("Loop");
            windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
            currentPlanet.onTick();
            Renderer.onTick();
            SoundManager.onTick();
            Profiler.onTick();
            Renderer.onTickEnd();
            Profiler.beginEndProfile("Loop");

            doneOneTick = true;//do last, ensures that certain functions are only called once per tick loop
        }

        public float getDrawDistance()
        {
            if(currentPlanet != null)
            {
                return currentPlanet.getDrawDistance();
            }
            return 1000.0F;
        }

        public void pauseGame()
        {
            Input.setCursorHiddenAndGrabbed(false);
            gamePaused = true;
        }

        public void unPauseGame()
        {
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

        public static bool paused { get => gamePaused; }
        public static GameInstance get { get => instance; }
    }
}
