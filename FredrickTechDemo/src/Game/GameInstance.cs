using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using OpenTK;
using OpenTK.Graphics;
using System;

namespace FredrickTechDemo
{
    /*This class is the main game class. It contains all the execution code for rendering, logic loops and loading.*/
    class GameInstance : GameWindow
    {
        private Input input;
        private Renderer renderer;
        public EntityPlayer thePlayer;
        public JaredsQuadModel jaredsQuad;
        public Vector3F jaredsQuadPos = new Vector3F();
        public Vector3F jaredsQuadRot = new Vector3F();
        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            renderer = new Renderer(this);
            thePlayer = new EntityPlayer("Steve", new Vector3F(0.0F, 0.0F, 1.5F));
            input = new Input(this);
            jaredsQuad = new JaredsQuadModel();
            GameSettings.loadSettings(this);
        }

        /*Called before game runs*/
        public void init()
        {
            TicksAndFps.init(30.0D);
            renderer.init();
        }

        /*overriding OpenTk game update function, called every frame.*/
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            for (int i = 0; i < TicksAndFps.getTicksElapsed(); i++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                onTick();
            }
            TicksAndFps.update();
        }

        /*overriding OpenTk render update function, called every frame.*/
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            renderer.renderJaredsQuad();
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
            jaredsQuad.onTick(jaredsQuadPos, jaredsQuadRot);
            thePlayer.onTick();
        }

    }
}
