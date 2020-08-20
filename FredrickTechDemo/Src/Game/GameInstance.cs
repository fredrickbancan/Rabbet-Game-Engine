using FredrickTechDemo.FredsMath;
using FredrickTechDemo.GUI;
using FredrickTechDemo.GUI.Text;
using FredrickTechDemo.VFX;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Drawing;

namespace FredrickTechDemo
{


    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        public static int temp = 0;
        public static readonly String mainGUIName = "mainGUI";
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
        public Planet currentPlanet;


        public GameInstance(int screenWidth, int screenHeight, int initialWidth, int initialHeight, String title) : base(initialWidth, initialHeight, new GraphicsMode(32,24,0,8), title)
        {
            Application.debug("Game window width: " + initialWidth);
            Application.debug("Game window height: " + initialHeight);
            GameInstance.privateRand = new Random();
            GameInstance.instance = this;
            GameInstance.windowWidth = initialWidth;
            GameInstance.windowHeight = initialHeight;
            GameInstance.screenHeight = screenHeight;
            GameInstance.screenWidth = screenWidth;
            GameInstance.mouseCenterX = this.X + this.Width / 2;
            GameInstance.mouseCenterY = this.Y + this.Height / 2;
            GameSettings.loadSettings();
            initialize();
        }

        /*Called before game runs*/
        private void initialize()
        {
            TextUtil.loadAllFoundTextFiles();
            setDPIScale();
            Renderer.init();
            GUIHandler.addNewGUIScreen(mainGUIName, "Trebuchet");
            TicksAndFps.init(30.0);
            DebugScreen.init();
            GUIHandler.addTextPanelToGUI(mainGUIName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(ColourF.darkRed)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "label", new GUITextPanel(new TextFormat(0,0.97F).setLine("Fredricks OpenGL Math tech demo.").setPanelColor(ColourF.black)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "help", new GUITextPanel(new TextFormat(0.5F,0).setAlign(TextAlign.CENTER).setLines(new string[] { "Press 'W,A,S,D and SPACE' to move. Move mouse to look around.", "Tap 'V' to toggle flying. Tap 'E' to release mouse.", "Walk up to tank and press F to drive, Left click to fire.", "Press 'ESC' to close game." }).setPanelColor(ColourF.black)));
            GUIHandler.addGUIComponentToGUI(mainGUIName, "crossHair", new GUICrosshair());
           
            //create and spawn player in new world
            thePlayer = new EntityPlayer("Steve", new Vector3D(0.0, 0.0, 2.0));
            currentPlanet = new Planet();
            currentPlanet.spawnEntityInWorld(thePlayer);
            for (int i = 0; i < 30; i++)
            {
                currentPlanet.spawnEntityInWorld(new EntityCactus());
            }
            currentPlanet.spawnEntityInWorld(new EntityTank(new Vector3D(5, 10, -5)));

            //center mouse in preperation for first person 
            Input.centerMouse();
            Input.toggleHideMouse();
        }

        /*overriding OpenTk game update function, called every frame.*/
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
            for (int i = 0; i < TicksAndFps.getTicksElapsed(); i++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                onTick();
            }
            Input.updateInput();
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            TicksAndFps.update();
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

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            Profiler.beginEndProfile(Profiler.gameLoopName);
            GUIHandler.onTick();
            updateGUI();
            VFXUtil.doDebugSmokeEffect(currentPlanet);
            currentPlanet.onTick();

            Profiler.beginEndProfile(Profiler.gameLoopName);
        }

        /*update the gui*/
        private void updateGUI()
        {
            if (thePlayer.getIsFlying())
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(ColourF.darkGreen).setLine("Flying: ON");
            }
            else
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(ColourF.darkRed).setLine("Flying: OFF");
            }
            DebugScreen.displayOrClearDebugInfo();
            GUIHandler.rebuildTextInGUI(mainGUIName);//do last, applies any changes to the text on screen.
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
