using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
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
        private static float prevScrollOffset = 0;
        /// <summary>
        /// if is true, inputs will not activate their default logic
        /// </summary>
        private static bool paused = false;

        public static void updateInput()
        {
            previouskeyboardState = keyboardState;
            keyboardState = GameInstance.get.KeyboardState.GetSnapshot();
            previousMouseState = mouseState;
            mouseState = GameInstance.get.MouseState.GetSnapshot();
            /*Only update keyboard input if the game window is focused, and if any key is being pressed.*/
            if (!paused && GameInstance.get.IsFocused)
            {
                if (Input.singleKeyPress(Keys.Escape))
                {
                    if (GameInstance.paused)
                    {
                        GUIManager.closeCurrentGUI();
                    }
                    else
                    {
                        GUIManager.openGUI(new GUIPauseMenu());
                    }
                }

                if (singleKeyPress(Keys.F1))
                {
                    toggleBoolean(ref GameSettings.drawHitboxes);
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
                if (singleKeyPress(Keys.F5))
                {
                    toggleBoolean(ref GameSettings.noclip);
                }
                if (singleKeyPress(Keys.F12))
                {
                    ScreenShotter.takeScreenshot();
                }
                PlayerController.updateInput(keyboardState, mouseState);//do player input 
                PlayerController.updateSinglePressInput(keyboardState);//do single button input
            }
            updateMouse();
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

        public static bool keyIsDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
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

        public static void onKeyDown(KeyboardKeyEventArgs e)
        {
            GUIManager.onKeyDown(e);
        }
        public static void onMouseDown(MouseButtonEventArgs e)
        {
            GUIManager.onMouseDown(e);
        }
        public static void onMouseWheel(MouseWheelEventArgs e)
        {
            GUIManager.onMouseWheel(e.OffsetY - prevScrollOffset);
            prevScrollOffset = e.OffsetY;
        }
        public static bool mouseleftButtonDown()
        {
            return mouseState.IsButtonDown(MouseButton.Left);
        }

        public static Vector2 getGrabbedMouseDelta()
        {
            return mouseDelta;
        }

        public static void pause()
        {
            paused = true;
        }

        public static void unPause()
        {
            paused = false;
        }
    }
}
