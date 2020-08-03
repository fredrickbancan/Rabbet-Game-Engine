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
            playerController = new PlayerController(game);
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
                

                #endregion

                #region single press input
                if (previouskeyboardState != keyboardState)
                {
                    playerController.updateSinglePressInput(keyboardState);//do player single button input

                    if (keyboardState.IsKeyDown(Key.Escape))
                    {
                        gameInstance.Exit();
                    }

                    if (keyboardState.IsKeyDown(Key.F3))
                    {
                        toggleDebugScreen();
                    }
                }
                #endregion
            }
            previouskeyboardState = keyboardState;
        }

        /*Places the mouse cursor at the center of the game window*/
        public static void centerMouse()
        {
            Mouse.SetPosition(GameInstance.windowCenterX, GameInstance.windowCenterY); // center the mouse cursor
        }

        public static void toggleDebugScreen()
        {
            if(!GameSettings.debugScreen)
            {
                GameSettings.debugScreen = true;
            }
            else
            {
                GameSettings.debugScreen = false;
            }
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
