using FredrickTechDemo.FredsMath;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Drawing;

namespace FredrickTechDemo
{
    //enum for text alignment choices
    public enum TextAlign { LEFT, CENTER, RIGHT };

    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {

        private static int windowWidth;
        private static int windowHeight;
        private static int mouseCenterX;//the x position of the center of the game window
        private static int mouseCenterY;//the y position of the center of the game window
        private static float dpiY;
        public EntityPlayer thePlayer;
        public Planet currentPlanet;

        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            windowWidth = width;
            windowHeight = height;
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
            Graphics g = Graphics.FromHwnd(this.WindowInfo.Handle);
            dpiY = g.DpiY;
            g.Dispose();
            
            GameSettings.loadSettings(this);
            thePlayer = new EntityPlayer("Steve", new Vector3D(0.0, 0.0, 2.0));
            currentPlanet = new Planet();
            currentPlanet.spawnEntityInWorld(thePlayer);
            for (int i = 0; i < 30; i++)
            {
                currentPlanet.spawnEntityInWorld(new EntityCactus());
            }
            currentPlanet.spawnEntityInWorld(new EntityTank(new Vector3D(5, 10, -5)));
        }

        /*Called before game runs*/
        public void init()
        {
            TicksAndFps.init(30.0, this);
            Renderer.init(this);
            Renderer.textRenderer2D.addNewTextPanel("flying", "Flying: OFF", new Vector2F(), ColourF.darkRed, TextAlign.RIGHT);
            Renderer.textRenderer2D.addNewTextPanel("label", "Fredricks OpenGL Math tech demo.", new Vector2F(0, 0.97F), ColourF.black);
            Renderer.textRenderer2D.addNewTextPanel("help", new string[] { "Press 'W,A,S,D and SPACE' to move. Move mouse to look around.", "Tap 'V' to toggle flying. Tap 'E' to release mouse.", "Walk up to tank and press F to drive, Left click to fire.", "Press 'ESC' to close game."}, new Vector2F(0.5F, 0.0F), ColourF.black, TextAlign.CENTER);
            Input.setGameInstance(this);
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
            updateGUI();
            
            currentPlanet.onTick();

            Profiler.beginEndProfile(Profiler.gameLoopName);
        }

        private void updateGUI()
        {
            if (thePlayer.getIsFlying())
            {
                Renderer.textRenderer2D.addNewTextPanel("flying", "Flying: ON", Vector2F.zero, ColourF.darkGreen, TextAlign.RIGHT);
            }
            else
            {
                Renderer.textRenderer2D.addNewTextPanel("flying", "Flying: OFF", Vector2F.zero, ColourF.darkRed, TextAlign.RIGHT);
            }
            DebugScreen.displayOrClearDebugInfo(this);

            Renderer.textRenderer2D.onTick();//do this last for gui text
        }

        public static int gameWindowWidth { get => windowWidth; }
        public static int gameWindowHeight { get => windowHeight; }
        public static int windowCenterX { get => mouseCenterX; }
        public static int windowCenterY { get => mouseCenterY; }
        public static float aspectRatio { get => (float)windowWidth / (float)windowHeight; }
        public static float dpiScale { get => (float)windowHeight / dpiY; }
    }
}
