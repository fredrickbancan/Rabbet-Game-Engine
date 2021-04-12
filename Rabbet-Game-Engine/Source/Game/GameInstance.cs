using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

using System;
using System.Drawing;

namespace RabbetGameEngine
{
    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        public static int temp = 0;
        private static GameInstance instance;
        private static Random nonCRand;
        private static FlyCamera defaultCam;
        public static int windowWidth;
        public static int windowHeight;
        public static int screenWidth;
        public static int screenHeight;
        private static Vector2 windowCenter;
        private static bool gamePaused = false;
        private static bool isClosing = false;
        public World currentWorld;

        /// <summary>
        /// Will be true if there has been atleast one onTick() call since last frame.
        /// </summary>
        private bool doneOneTick = false;

        public GameInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings windowSettings) : base(gameWindowSettings, windowSettings)
        {
            instance = this;
            Title = Application.applicationName;
            int iconWidth, iconHeight;
            byte[] data;
            IconLoader.getIcon("icon", out iconWidth, out iconHeight, out data);
            Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image[] { new OpenTK.Windowing.Common.Input.Image(iconWidth, iconHeight, data) });
        }

        protected override void OnLoad()
        {
            if (isClosing || IsExiting) return;
            Application.infoPrint("loading.");
            try
            {
                nonCRand = new Random();
                TicksAndFrames.init(30);
                ResourceUtil.init();
                GameSettings.loadSettings();
                Renderer.init();
                TextUtil.loadAllFoundTextFiles();
                SoundManager.init();
                windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
                GUIManager.addPersistentGUI(new GUIHud());
                currentWorld = new World(0xdeadbeef);
                Input.setCursorHiddenAndGrabbed(true);
                Input.updateInput();
                defaultCam = new FlyCamera(new Vector3(0, 0, 0));
                Renderer.setCamera(defaultCam);
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
            if (isClosing || IsExiting) return;
            this.WindowState = WindowState.Normal;
            Close();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (isClosing || IsExiting) return;
            base.OnMouseWheel(e);
            Input.onMouseWheel(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (isClosing || IsExiting) return;
            base.OnKeyDown(e);
            Input.onKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (isClosing || IsExiting) return;
            base.OnMouseDown(e);
            Input.onMouseDown(e);
        }

        public Size getGameWindowSize()
        {
            return new Size(ClientRectangle.Size.X, ClientRectangle.Size.Y);
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (isClosing || IsExiting) return;
            Profiler.startRoot();
            base.OnRenderFrame(args);
            Input.updateInput();
            try
            {
                doneOneTick = false;
                Profiler.startTick();
                Profiler.startSection("tickLoop");
                Profiler.startTickSection("tickLoop");
                TicksAndFrames.doOnTickUntillRealtimeSync(onTick);
                Profiler.endCurrentTickSection();
                Profiler.endCurrentSection();

                if (doneOneTick)
                {
                    //This area will be called at MAXIMUM of the tick rate. Meaning it will not be called multiple times in a laggy situation.
                    //It is called after the ticks are looped.
                    Application.updateRamUsage();
                    GUIManager.doUpdate();
                    Renderer.doWorldRenderUpdate();
                    SoundManager.onUpdate();
                    PlayerController.resetActions();
                }
                Profiler.endTick();
                Profiler.onTick();
            }
            catch (Exception e)
            {
                Application.error("Failed to run game tick, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            defaultCam.onFrame(TicksAndFrames.getPercentageToNextTick());
            SoundManager.onFrame();
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
            if (isClosing || IsExiting) return;
            base.OnResize(e);
            windowWidth = this.ClientRectangle.Size.X;
            windowHeight = this.ClientRectangle.Size.Y;
            Renderer.onResize();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            if (isClosing || IsExiting) return;
            //pausing the game if the window focus changes
            pauseGame();
            base.OnFocusedChanged(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            if (Bounds.Size.X > 0 && Bounds.Size.Y > 0)
                windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
            if (!gamePaused)
                currentWorld.onTick(TicksAndFrames.spt);
            defaultCam.onTick(TicksAndFrames.spt);
            doneOneTick = true;//do last, ensures that certain functions are only called once per tick loop
        }

        public void onVideoSettingsChanged()
        {
            Renderer.onVideoSettingsChanged();//do last
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

        public override void Close()
        {
            isClosing = true;
            Renderer.onClosing();
            SoundManager.onClosing();
            base.Close();
        }

        public static int gameWindowWidth { get => windowWidth; }
        public static int gameWindowHeight { get => windowHeight; }
        public static Vector2 gameWindowCenter { get => windowCenter; }
        public static float aspectRatio { get => (float)windowWidth / (float)windowHeight; }

        public static Random rand { get => nonCRand; }

        public static int realScreenWidth { get => screenWidth; }
        public static int realScreenHeight { get => screenHeight; }

        public static bool paused { get => gamePaused; }
        public static GameInstance get { get => instance; }
    }
}
