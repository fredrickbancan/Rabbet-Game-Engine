using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace FredrickTechDemo
{
    class GameInstance : GameWindow
    {
        private TicksAndFps tickFps;
        private Input input;

        public GameInstance(int width, int height, String title) : base(width, height, GraphicsMode.Default, title)
        {
            tickFps = new TicksAndFps(30.0D);
            input = new Input(this);
        }

        public void init()
        {
            tickFps.init();
        }
        public void runGame()//Will start the OpenTk Game instance running. Eeach frame will call OnUpdateFrame and OnRenderFrame. I am using my own tickrate class to controll ticks (TicksAndFps.cs)
        {
            base.Run();
        }

        #region overriding OpenTk base game methods
        protected override void OnUpdateFrame(FrameEventArgs args)//overriding OpenTk game update function, called every frame.
        {
            base.OnUpdateFrame(args);
            tickFps.update();

            for(TicksAndFps.itteratior = 0; TicksAndFps.itteratior < tickFps.getTicksElapsed(); TicksAndFps.itteratior++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                Application.say("Tick!");
                onTick();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)//overriding OpenTk render update function, called every frame.
        {
            base.OnRenderFrame(args);
            updateCameraAndDraw();
        }
        #endregion

        #region On Tick Update
        private void onTick()//insert game logic here
        {
            input.updateInput();
        }
        #endregion

        #region On Render Update
        public void updateCameraAndDraw()//rendering starts here
        {

        }
        #endregion

    }
}
