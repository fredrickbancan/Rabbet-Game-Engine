using OpenTK;
using OpenTK.Graphics;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.GUI.Text;
using System;
using System.ComponentModel;
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
        private static int screenWidth;
        private static int screenHeight;
        private static int mouseCenterX;//the x position of the center of the game window
        private static int mouseCenterY;//the y position of the center of the game window
        private static float dpiY;
        public EntityPlayer thePlayer;
        public World currentPlanet;

        public GameInstance(int screenWidth, int screenHeight, int initialWindowWidth, int initialWindowHeight, string title) : base(initialWindowWidth, initialWindowHeight, GraphicsMode.Default, title)
        {
            Application.debugPrint("Game window width: " + initialWindowWidth);
            Application.debugPrint("Game window height: " + initialWindowHeight);
            GameInstance.windowWidth = initialWindowWidth;
            GameInstance.windowHeight = initialWindowHeight;
            GameInstance.screenHeight = screenHeight;
            GameInstance.screenWidth = screenWidth;
        }

        protected override void OnLoad(EventArgs e)
        {
            GameInstance.instance = this;
            GameInstance.privateRand = new Random();
            GameInstance.mouseCenterX = this.X + this.Width / 2;
            GameInstance.mouseCenterY = this.Y + this.Height / 2;
            GameSettings.loadSettings();
            TextUtil.loadAllFoundTextFiles();
            setDPIScale();
            Renderer.init();
            TicksAndFps.init(30);
            MainGUI.init();
            DebugInfo.init();
            HitboxRenderer.init();
            //create and spawn player in new world
            thePlayer = new EntityPlayer("Steve", new Vector3(0, 0, 2));
            currentPlanet = new World();
            for (int i = 0; i < 10; i++)
            {
               // currentPlanet.spawnEntityInWorld(new EntityCactus(new Vector3(0, 10, 0)));
            }
            currentPlanet.spawnEntityInWorld(thePlayer);
          //  currentPlanet.spawnEntityInWorld(new EntityTank(new Vector3(5, 10, -5)));

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
            for (int i = 0; i < TicksAndFps.getTicksElapsed(); i++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                onTick();
            }
            PlayerController.resetActions();
            TicksAndFps.update();
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
            Profiler.beginEndProfile(Profiler.gameLoopName);
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
            GUIHandler.onTick();
            MainGUI.onTick();
            currentPlanet.onTick();
            Profiler.beginEndProfile(Profiler.gameLoopName);
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
            currentPlanet.onLeavingWorld();
            Renderer.onClosing();
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
