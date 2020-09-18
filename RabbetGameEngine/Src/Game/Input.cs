using OpenTK.Input;

namespace RabbetGameEngine
{
    /*This class is responsable for checking the input of the mouse and keyboard,
      and manipulating the games logic respectively. Checking should be done each tick.*/
    public static class Input
    {
        private static bool mouseHidden = false;

        private static KeyboardState previouskeyboardState;
        private static KeyboardState keyboardState;

        private static MouseState previousMouseState;
        private static MouseState mouseState;

        /*called every tick to check which keys are being pressed and manipulates the provided game instance reference's logic and entities */
        public static void updateInput()
        {
            updateMouseButtonInput();
            updateKeyboardInput();
        }
        private static void updateMouseButtonInput()
        {
            previousMouseState = mouseState;
            mouseState = Mouse.GetState();
            /*Only update mouse input if the game window is focused, and if any key is being pressed.*/
            if (GameInstance.get.Focused && mouseState.IsAnyButtonDown)
            {
                //do constant input here

                //this does single button input
                PlayerController.updateMouseButtonInput(mouseState);
            }
        }

        private static void updateKeyboardInput()
        {
            previouskeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            /*Only update keyboard input if the game window is focused, and if any key is being pressed.*/
            if (GameInstance.get.Focused && keyboardState.IsAnyKeyDown)
            {
                if (singleKeyPress(Key.Escape))
                {
                    GameInstance.get.Exit();
                }

                if (singleKeyPress(Key.F3))
                {
                    toggleBoolean(ref GameSettings.debugScreen);
                }

                if (singleKeyPress(Key.F4))
                {
                    toggleBoolean(ref GameSettings.drawHitboxes);
                }
                if (singleKeyPress(Key.F5))
                {
                    toggleBoolean(ref GameSettings.noclip);
                }

                if (singleKeyPress(Key.F12))
                {
                    toggleBoolean(ref GameSettings.fullscreen);
                    Renderer.onToggleFullscreen();
                }
                PlayerController.updateInput(keyboardState);//do player input 
                PlayerController.updateSinglePressInput(keyboardState);//do player single button input
            }
        }
        /*Places the mouse cursor at the center of the game window*/
        public static void centerMouse()
        {
            Mouse.SetPosition(GameInstance.windowCenterX, GameInstance.windowCenterY); // center the mouse cursor
        }

        public static void toggleBoolean(ref bool boolean)
        {
            if (!boolean)
            {
                boolean  = true;
            }
            else
            {
                boolean = false;
            }
        }

        /*toggles the visibility of the mouse cursor*/
        public static void toggleHideMouse()
        {
            if(!mouseHidden)
            {
                GameInstance.get.CursorVisible = false;
                mouseHidden = true;
            }
            else
            {
                GameInstance.get.CursorVisible = true;
                mouseHidden = false;
            }
        }

        public static bool getIsMouseHidden()
        {
            return mouseHidden;
        }

        //returns true if this key is pressed, and only for the first frame.
        public static bool singleKeyPress(Key key)
        {
            return keyboardState[key] && !previouskeyboardState[key];
        }
        public static bool singleMouseButtonPress(MouseButton key)
        {
            return mouseState[key] && !previousMouseState[key];
        }
    }
}
