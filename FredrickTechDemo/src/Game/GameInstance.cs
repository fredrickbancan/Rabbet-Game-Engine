using FredrickTechDemo.FredsMath;
using OpenTK;
using OpenTK.Graphics;
using System;

namespace FredrickTechDemo
{
    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    class GameInstance : GameWindow
    {
        private static int windowWidth;
        private static int windowHeight;
        private static int mouseCenterX;//the x position of the center of the game window
        private static int mouseCenterY;//the y position of the center of the game winwow
        private Renderer renderer;
        public EntityPlayer thePlayer;

        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            windowWidth = width;
            windowHeight = height;
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;
            renderer = new Renderer(this);
            thePlayer = new EntityPlayer("Steve", new Vector3F(0.0F, 16.0F, 32F));
            GameSettings.loadSettings(this);
        }

        /*Called before game runs*/
        public void init()
        {
            TicksAndFps.init(30.0D);
            renderer.init();
            Input.setGameInstance(this);
            Input.centerMouse();
            Input.toggleHideMouse();
        }

        /*overriding OpenTk game update function, called every frame.*/
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            mouseCenterX = this.X + this.Width / 2;
            mouseCenterY = this.Y + this.Height / 2;

            for (int i = 0; i < TicksAndFps.getTicksElapsed(); i++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                onTick();
            }
            TicksAndFps.update();
            base.OnUpdateFrame(args);
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            
            renderer.renderAll();
            base.OnRenderFrame(args);
        }

        /*Overriding OpenTK resize function, called every time the game window is resized*/
        protected override void OnResize(EventArgs e)
        {
            windowWidth = Width;
            windowHeight = Height;
            renderer.onResize();
            base.OnResize(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        {
            Input.updateInput();
            thePlayer.onTick();
        }

        public static int gameWindowWidth{get => windowWidth;}
        public static int gameWindowheight {get => windowHeight;}
        public static int windowCenterX {get => mouseCenterX;}
        public static int windowCenterY {get => mouseCenterY;}
        public static float aspectRatio {get => ((float)windowWidth / (float)windowHeight); }
    }
}
