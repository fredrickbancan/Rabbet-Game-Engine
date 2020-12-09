using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RabbetGameEngine
{
    /*This class is responsable for checking the input of the mouse and keyboard,
      and manipulating the games logic respectively. Checking should be done each tick.*/
    public static class Input
    {
        private static KeyboardState previouskeyboardState;
        private static KeyboardState keyboardState;

        private static MouseState previousMouseState;
        private static MouseState mouseState;

        private static bool mouseGrabbed = false;
        private static Vector2 mouseDelta = new Vector2(0,0);

        /*called every tick to check which keys are being pressed and manipulates the provided game instance reference's logic and entities */
        public static void updateInput()
        {
            PlayerController.resetActions();
            updateMouseButtonInput();
            updateKeyboardInput();
            updateMouse();
        }
        private static void updateMouseButtonInput()
        {
            previousMouseState = mouseState;
            mouseState = GameInstance.get.MouseState.GetSnapshot();
            /*Only update mouse input if the game window is focused, and if any key is being pressed.*/
            if (GameInstance.get.IsFocused && mouseState.IsAnyButtonDown)
            {
                //do constant input here

                //this does single button input
                PlayerController.updateMouseButtonInput(mouseState);
            }
        }

        private static void updateKeyboardInput()
        {
            previouskeyboardState = keyboardState;
            keyboardState = GameInstance.get.KeyboardState.GetSnapshot();

            /*Only update keyboard input if the game window is focused, and if any key is being pressed.*/
            if (GameInstance.get.IsFocused && keyboardState.IsAnyKeyDown)
            {
                if (Input.singleKeyPress(Keys.Escape))
                {
                    if (GameInstance.paused)
                    {
                        GameInstance.get.unPauseGame();
                        GUIManager.closeCurrentGUI();
                    }
                    else
                    {
                        GameInstance.get.pauseGame();
                        GUIManager.openGUI(new GUIPauseMenu());
                    }
                }

                if (singleKeyPress(Keys.F1))
                {
                    toggleBoolean(ref GameSettings.drawHitboxes);
                }
                
                if (singleKeyPress(Keys.F2))
                {
                    toggleBoolean(ref GameSettings.entityLabels);
                    GameInstance.get.onToggleEntityLabels();
                }

                if (singleKeyPress(Keys.F3))
                {
                    toggleBoolean(ref GameSettings.debugScreen);
                    if(GameSettings.debugScreen)
                    {
                        GUIManager.addPersistentGUI(new GUIDebugInfo());
                    }
                    else
                    {
                        GUIManager.removePersistentGUI("debugInfo");
                    }
                }

                if (singleKeyPress(Keys.F4))
                {
                    toggleBoolean(ref GameSettings.fullscreen);
                    Renderer.onToggleFullscreen();
                }
                if (singleKeyPress(Keys.F5))//TODO: Move to playercontroller
                {
                    toggleBoolean(ref GameSettings.noclip);
                }
                if (singleKeyPress(Keys.F12))
                {
                    ScreenShotter.takeScreenshot();
                }
                PlayerController.updateInput(keyboardState);//do player input 
                PlayerController.updateSinglePressInput(keyboardState);//do player single button input
            }
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

        //returns true if this key is pressed, and only for the first frame.
        public static bool singleKeyPress(Keys key)
        {
            return keyboardState.IsKeyDown(key) && !previouskeyboardState.IsKeyDown(key);
        }

        public static bool singleMouseButtonPress(MouseButton key)
        {
            return mouseState.IsButtonDown(key) && !previousMouseState.IsButtonDown(key);
        }

        public static void setCursorHiddenAndGrabbed(bool flag)
        {
            GameInstance.get.CursorVisible = !flag;
            GameInstance.get.CursorGrabbed = flag;
            if(!flag)
            {
                GameInstance.get.MousePosition = GameInstance.gameWindowCenter;
            }
            mouseGrabbed = flag;
        }

        private static void updateMouse()
        {
            if(mouseGrabbed)
            {
                mouseDelta =  GameInstance.get.MouseState.Delta;
            }
            else
            {
                mouseDelta = Vector2.Zero;
            }
        }

        public static bool mouseleftButtonDown()
        {
            return mouseState.IsButtonDown(MouseButton.Left);
        }

        public static Vector2 getMouseDelta()
        {
            return mouseDelta;
        }

    }
}
