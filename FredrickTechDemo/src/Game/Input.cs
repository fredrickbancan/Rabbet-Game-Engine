using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for checking the input of the mouse and keyboard,
      and manipulating the games logic respectively. Checking is done each tick.*/
    class Input
    {
        private GameInstance gameInstance;
        private PlayerController playerController;
        private KeyboardState previouskeyboardState;
        private KeyboardState keyboardState;
        public Input(GameInstance game)
        {
            this.gameInstance = game;
            this.playerController = new PlayerController(gameInstance.thePlayer);
            this.previouskeyboardState = Keyboard.GetState();
        }

        /*called every tick to check which keys are being pressed and manipulates the provided game instance reference's logic and entities */
        public void updateInput()
        {
            this.keyboardState = Keyboard.GetState();
            if (keyboardState.IsAnyKeyDown)
            {
                playerController.updateInput(keyboardState);
                #region constant input
                if (keyboardState.IsKeyDown(Key.Right))
                {
                    gameInstance.jaredsQuadRot.y -= 3.0F;
                }
                if (keyboardState.IsKeyDown(Key.Left))
                {
                    gameInstance.jaredsQuadRot.y += 3.0F;
                }
                if (keyboardState.IsKeyDown(Key.Up))
                {
                    gameInstance.jaredsQuadRot.x -= 3.0F;
                }
                if (keyboardState.IsKeyDown(Key.Down))
                {
                    gameInstance.jaredsQuadRot.x += 3.0F;
                }

                #endregion

                #region single press input
                if (previouskeyboardState != keyboardState)
                {
                    playerController.updateSinglePressInput(keyboardState);
                    if (keyboardState.IsKeyDown(Key.Escape))
                    {
                        gameInstance.Exit();
                    }
                }
                #endregion
            }
            previouskeyboardState = keyboardState;
            
        }
    }
}
