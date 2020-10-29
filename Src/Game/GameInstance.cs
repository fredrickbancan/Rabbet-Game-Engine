using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.GUI.Text;
using RabbetGameEngine.Sound;
using System;
using System.ComponentModel;

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
        private static int screenWidth;
        private static int screenHeight;
        private static float mouseCenterX;//the x position of the center of the game window
        private static float mouseCenterY;//the y position of the center of the game window
        private static float dpiY;
        public EntityPlayer thePlayer;
        public Planet currentPlanet;

        public GameInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings windowSettings) : base(gameWindowSettings, windowSettings)
        {
            GameInstance.windowWidth = this.Bounds.;
            GameInstance.windowHeight = initialWindowHeight;
            GameInstance.screenHeight = screenHeight;
            GameInstance.screenWidth = screenWidth;
        }

        protected override void OnLoad(EventArgs e)
        {
            GameInstance.instance = this;
            GameInstance.privateRand = new Random();
            GameInstance.mouseCenterX = this.Bounds.Center.X;
            GameInstance.mouseCenterY = this.Bounds.Center.Y;
            GameSettings.loadSettings();
            TextUtil.loadAllFoundTextFiles();
            SoundManager.init();
            setDPIScale();
            Renderer.init();

            this.Icon = new Icon(ResourceUtil.getIconFileDir("icon.ico"));

            TicksAndFrames.init(30);
            MainGUI.init();
            DebugInfo.init();
            currentPlanet = new Planet(0xdeadbeef);
            //create and spawn player in new world
            thePlayer = new EntityPlayer(currentPlanet, "Steve", new Vector3(0, 3, 2));
            SoundManager.setListener(thePlayer);
            SoundManager.playSoundAux("Explosion_1");
            currentPlanet.spawnEntityInWorld(new EntityTank(currentPlanet, new Vector3(5, 10, -5)));
            for (int i = 0; i < 128; i++)
            {
                currentPlanet.spawnEntityInWorld(new EntityCactus(currentPlanet, new Vector3(0, 10, 0)));
            }
            currentPlanet.spawnEntityInWorld(thePlayer);
            //center mouse in preperation for first person 
            Input.centerMouse();
            Input.toggleHideMouse();
            base.OnLoad(e);
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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            windowWidth = Width;
            windowHeight = Height;
            Renderer.onResize();
        }
        protected override void OnFocusedChanged(EventArgs e)
        {
            if (thePlayer != null && !thePlayer.paused)
            {//pausing the game if the window focus changes
                GameInstance.pauseGame();
            }
            base.OnFocusedChanged(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            Profiler.beginEndProfile("Loop");
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
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

        public static void pauseGame()
        {
            Input.centerMouse(); // center the mouse cursor when closing or opening menu
            Input.toggleHideMouse();
            GameInstance.get.thePlayer.togglePause();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            currentPlanet.onLeavingPlanet();
            Renderer.onClosing();
            SoundManager.onClosing();
            base.OnClosing(e);
        }
        private void setDPIScale()
        {
            Graphics g = Graphics.FromHwnd(this.WindowInfo.Handle);
            GameInstance.dpiY = g.DpiY;
            g.Dispose();
        }
        public static int gameWindowWidth { get => windowWidth; }
        public static int gameWindowHeight { get => windowHeight; }
        public static int realScreenWidth { get => screenWidth; }
        public static int realScreenHeight { get => screenHeight; }
        public static int windowCenterX { get => mouseCenterX; }
        public static int windowCenterY { get => mouseCenterY; }
        public static float aspectRatio { get => (float)windowWidth / (float)windowHeight; }
        public static float dpiScale { get => (float)windowHeight / dpiY; }
        public static Random rand { get => privateRand; }
        public static GameInstance get { get => instance; }
    }
}
