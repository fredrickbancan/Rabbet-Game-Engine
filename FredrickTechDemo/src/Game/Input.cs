using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo
{
    class Input
    {
        private GameInstance gameInstance;
        private KeyboardState keyboardState;
        public Input(GameInstance theGame)
        {
            this.gameInstance = theGame;
            this.keyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// called every frame to check which keys are being pressed and manipulates the provided game instance reference's logic and entities 
        /// </summary>
        public void updateInput()
        {
            this.keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Key.Escape))
            {
                gameInstance.Exit();
            }

        }
    }
}
