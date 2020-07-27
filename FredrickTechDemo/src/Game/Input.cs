using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for checking the input of the mouse and keyboard,
      and manipulating the games logic respectively. Checking is done each tick.*/
    class Input
    {
        private GameInstance gameInstance;
        private KeyboardState keyboardState;
        public Input(GameInstance game)
        {
            this.gameInstance = game;
            this.keyboardState = Keyboard.GetState();
        }

        /*called every tick to check which keys are being pressed and manipulates the provided game instance reference's logic and entities */
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
