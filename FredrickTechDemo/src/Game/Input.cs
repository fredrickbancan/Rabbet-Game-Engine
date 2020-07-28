using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for checking the input of the mouse and keyboard,
      and manipulating the games logic respectively. Checking should be done each tick.*/
    static class Input
    {
        private static bool mouseHidden = false;
        private static GameInstance gameInstance;
        private static PlayerController playerController;
        private static KeyboardState previouskeyboardState;
        private static KeyboardState keyboardState;

        /*Set the game instance for input to manipulate*/
        public static void setGameInstance(GameInstance game)
        {
            gameInstance = game;
            playerController = new PlayerController(gameInstance.thePlayer);
            previouskeyboardState = Keyboard.GetState();
        }

        /*called every tick to check which keys are being pressed and manipulates the provided game instance reference's logic and entities */
        public static void updateInput()
        {
            keyboardState = Keyboard.GetState();

           /*Only update keyboard input if the game window is focused, and if any key is being pressed.*/
            if (gameInstance.Focused && keyboardState.IsAnyKeyDown)
            {
                playerController.updateInput(keyboardState);//do player input

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
                    playerController.updateSinglePressInput(keyboardState);//do player single button input

                    if (keyboardState.IsKeyDown(Key.Escape))
                    {
                        gameInstance.Exit();
                    }
                }
                #endregion
            }
            previouskeyboardState = keyboardState;
        }

        /*Places the mouse cursor at the center of the game window*/
        public static void centerMouse()
        {
            Mouse.SetPosition(GameInstance.mouseCenterX, GameInstance.mouseCenterY); // center the mouse cursor
        }

        /*toggles the visibility of the mouse cursor*/
        public static void toggleHideMouse()
        {
            if(!mouseHidden)
            {
                gameInstance.CursorVisible = false;
                mouseHidden = true;
            }
            else
            {
                gameInstance.CursorVisible = true;
                mouseHidden = false;
            }
        }

        public static bool getIsMouseHidden()
        {
            return mouseHidden;
        }
    }
}
