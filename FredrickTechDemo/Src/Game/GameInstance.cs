using FredrickTechDemo.FredsMath;
using FredrickTechDemo.GUI;
using FredrickTechDemo.GUI.Text;
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
        public World currentPlanet;

        /*Temporary arcade vars*/
        private static int directHitCounter = 0;
        private static int airShotCounter = 0;
        private static int ticksSinceDirectHitPopup = 0;
        private static int ticksSinceAirShotPopup = 0;
        private static bool showingDirectHitPopup = false;
        private static bool showingAirShotPopup = false;
        private static int maxPopupTicks = 0;
        private static int directHitPopupTicks = 0;
        private static int airShotPopupTicks = 0;

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
            TicksAndFps.init(15.0);
            DebugScreen.init();
            GUIHandler.addTextPanelToGUI(mainGUIName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(ColourF.darkRed)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "label", new GUITextPanel(new TextFormat(0,0.97F).setLine("Fredricks OpenGL Math tech demo.").setPanelColor(ColourF.black)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "help", new GUITextPanel(new TextFormat(0.5F,0).setAlign(TextAlign.CENTER).setLines(new string[] { "Press 'W,A,S,D and SPACE' to move. Move mouse to look around.", "Tap 'V' to toggle flying. Tap 'E' to release mouse.", "Walk up to tank and press F to drive, Left click to fire.", "Press 'ESC' to close game." }).setPanelColor(ColourF.black)));
            GUIHandler.addGUIComponentToGUI(mainGUIName, "crossHair", new GUICrosshair());

            /*TEMPORARY, just for arcade stuff*/
            GUIHandler.addTextPanelToGUI(mainGUIName, "directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(ColourF.flame)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "directHit");

            GUIHandler.addTextPanelToGUI(mainGUIName, "airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(ColourF.red)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "airShot");

            GUIHandler.addTextPanelToGUI(mainGUIName, "directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + directHitCounter).setPanelColor(ColourF.flame)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + airShotCounter).setPanelColor(ColourF.red)));
           
            //create and spawn player in new world
            thePlayer = new EntityPlayer("Steve", new Vector3D(0.0, 0.0, 2.0));
            currentPlanet = new World();
            for (int i = 0; i < 30; i++)
            {
                currentPlanet.spawnEntityInWorld(new EntityCactus(new Vector3D(0, 10, 0)));
            }
            currentPlanet.spawnEntityInWorld(thePlayer);
            currentPlanet.spawnEntityInWorld(new EntityTank(new Vector3D(5, 10, -5)));

            //center mouse in preperation for first person 
            Input.centerMouse();
            Input.toggleHideMouse();

            /*Temp arcade stuff*/
            maxPopupTicks = (int)TicksAndFps.getNumOfTicksForSeconds(1.5F);
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
            currentPlanet.onTick();

            /*Temporary, for arcade popups.*/
            if(showingDirectHitPopup)
            {
                directHitPopupTicks++;
                if (directHitPopupTicks >= maxPopupTicks)
                {
                    showingDirectHitPopup = false;
                    directHitPopupTicks = 0;
                }
            }

            if (showingAirShotPopup)
            {
                airShotPopupTicks++;
                if(airShotPopupTicks >= maxPopupTicks)
                {
                    showingAirShotPopup = false;
                    airShotPopupTicks = 0;
                }
            }



            Profiler.beginEndProfile(Profiler.gameLoopName);
        }

        /*Called when player lands direct hit on a cactus, TEMPORARY!*/
        public static void onDirectHit()
        {
            directHitCounter++;
            directHitPopupTicks = 0;
            showingDirectHitPopup = true;
        }

        /*Called when player lands air shot on a cactus, TEMPORARY!*/
        public static void onAirShot()
        {
            airShotCounter++;
            airShotPopupTicks = 0;
            showingAirShotPopup = true;
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

            GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "directHitCount").setLine("Direct Hits: " + directHitCounter);
            GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "airShotCount").setLine("AirShots: " + airShotCounter);

            if (showingDirectHitPopup)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, "directHit");
            }
            else 
            { 
                GUIHandler.hideTextPanelInGUI(mainGUIName, "directHit");
            }
            if (showingAirShotPopup)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, "airShot");
            }
            else 
            { 
                GUIHandler.hideTextPanelInGUI(mainGUIName, "airShot");
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
