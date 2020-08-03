using FredrickTechDemo.FredsMath;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Drawing;

namespace FredrickTechDemo
{
    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    public class GameInstance : GameWindow
    {
        private static int windowWidth;
        private static int windowHeight;
        private static int mouseCenterX;//the x position of the center of the game window
        private static int mouseCenterY;//the y position of the center of the game window
        private static float dpiY;
        private static float dpiX;
        public EntityPlayer thePlayer;

        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            windowWidth = width;
            windowHeight = height;
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
            Graphics g = Graphics.FromHwnd(this.WindowInfo.Handle);
            dpiY = g.DpiY;
            dpiX = g.DpiX;
            
            GameSettings.loadSettings(this);
            thePlayer = new EntityPlayer("Steve", new Vector3F(0.0F, 16.0F, 32F));
        }

        /*Called before game runs*/
        public void init()
        {
            TicksAndFps.init(30.0D, this);
            Renderer.init(this);
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
            Input.updateInput();
            DebugScreen.displayOrClearDebugInfo(this);
            thePlayer.onTick();
            Profiler.beginEndProfile(Profiler.gameLoopName);
        }


        public static int gameWindowWidth { get => windowWidth; }
        public static int gameWindowHeight { get => windowHeight; }
        public static int windowCenterX { get => mouseCenterX; }
        public static int windowCenterY { get => mouseCenterY; }
        public static float aspectRatio { get => (float)windowWidth / (float)windowHeight; }
        public static float dpiScale { get => (float)windowHeight / dpiY; }
    }
}
