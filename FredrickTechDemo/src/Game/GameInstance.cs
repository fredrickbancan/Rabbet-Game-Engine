using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Model;
using OpenTK;
using OpenTK.Graphics;
using System;

namespace FredrickTechDemo
{
    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    class GameInstance : GameWindow
    {
        private TicksAndFps tickFps;
        private Input input;
        private Renderer renderer;
        public EntityPlayer thePlayer;

        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            tickFps = new TicksAndFps(30.0D);
            input = new Input(this);
            renderer = new Renderer(this);
            thePlayer = new EntityPlayer("Steve", new Vector3F(0.0F, 0.0F, 1.5F));
            GameSettings.loadSettings(this);
        }

        /*Called before game runs*/
        public void init()
        {
            tickFps.init();
            renderer.init();
        }

        /*overriding OpenTk game update function, called every frame.*/
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            for (int i = 0; i < tickFps.getTicksElapsed(); i++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                onTick();
            }
            tickFps.update();

            base.OnUpdateFrame(args);
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            renderer.renderJaredsQuad();

            base.OnRenderFrame(args);
        }

        /*Overriding OpenTK resize function, called every time the game window is resized*/
        protected override void OnResize(EventArgs e)
        {
            renderer.onResize();
            base.OnResize(e);
        }

        /*Each itteration of game logic is done here*/
        private void onTick()
        { 
            input.updateInput();
            JaredsQuadModel.onTick();
        }

        /*Returns a percentage of progress from current tick to the next tick, used for interpolation*/
        public float getPercentageNextTick()
        {
            return (float) tickFps.getPartialTicks();
        }

    }
}
