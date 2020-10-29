using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.GUI.Text;
using RabbetGameEngine.Sound;
using RabbetGameEngine.Src.Game;
using System;
using System.Drawing;

namespace RabbetGameEngine
{

    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        public static int temp = 0;
        private static GameInstance instance;
        private static Random privateRand;
        private static int windowWidth;
        private static int windowHeight;
        private static Vector2 windowCenter;
        private static float dpiY;
        public EntityPlayer thePlayer;
        public Planet currentPlanet;

        public GameInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings windowSettings) : base(gameWindowSettings, windowSettings)
        {
            GameInstance.windowWidth = this.ClientRectangle.Size.X;
            GameInstance.windowHeight = this.ClientRectangle.Size.Y;
            Title = Application.applicationName;
            int iconWidth, iconHeight;
            byte[] data;
            IconLoader.getIcon("icon", out iconWidth, out iconHeight, out data);
            Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image[] { new OpenTK.Windowing.Common.Input.Image(iconWidth, iconHeight, data) });
        }

        protected override void OnLoad()
        {
            GameInstance.instance = this;
            GameInstance.privateRand = new Random();
            GameSettings.loadSettings();
            TextUtil.loadAllFoundTextFiles();
            SoundManager.init();
            windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
            setDPIScale();
            Renderer.init();
            TicksAndFrames.init(30);
            MainGUI.init();
            DebugInfo.init();
            currentPlanet = new Planet(0xdeadbeef);
            //create and spawn player in new world
            thePlayer = new EntityPlayer(currentPlanet, "Steve", new Vector3(0, 3, 2));
            SoundManager.setListener(thePlayer);
            SoundManager.playSoundAux("Explosion_1");
            currentPlanet.spawnEntityInWorld(new EntityTank(currentPlanet, new Vector3(5, 10, -5)));
            for (int i = 0; i < 2; i++)
            {
                currentPlanet.spawnEntityInWorld(new EntityCactus(currentPlanet, new Vector3(0, 10, 0)));
            }
            currentPlanet.spawnEntityInWorld(thePlayer);
            Input.setCursorHiddenAndGrabbed(true);
            base.OnLoad();
        }

        public Size getGameWindowSize()
        {
            return new Size(ClientSize.X, ClientSize.Y);
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            Input.updateInput();
            TicksAndFrames.doOnTickUntillRealtimeSync(onTick);
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
            if (thePlayer != null && !thePlayer.paused)
            {
                //pausing the game if the window focus changes
                pauseGame();
            }
            base.OnFocusedChanged(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            Profiler.beginEndProfile("Loop");
            windowCenter = new Vector2(this.Location.X / this.Bounds.Size.X + this.Bounds.Size.X / 2, this.Location.Y / this.Bounds.Size.Y + this.Bounds.Size.Y / 2);
            Renderer.onTickStart();
            GUIManager.onTick();
            MainGUI.onTick();
            currentPlanet.onTick();
            Profiler.updateAverages();
            Renderer.onTickEnd();
            Profiler.beginEndProfile("Loop");
        }
        
        /*Called when player lands direct hit on a cactus, TEMPORARY!*/
        public static void onDirectHit()
        {
            MainGUI.onDirectHit();
        }

        /*Called when player lands air shot on a cactus, TEMPORARY!*/
        public static void onAirShot()
        {
            MainGUI.onAirShot();
        }

        public void pauseGame()
        {
            thePlayer.togglePause();
            Input.setCursorHiddenAndGrabbed(!thePlayer.paused);
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
        public static GameInstance get { get => instance; }
    }
}
