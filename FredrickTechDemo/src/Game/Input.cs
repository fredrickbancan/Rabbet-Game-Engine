using OpenTK.Input;
using System;

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
                        Console.WriteLine();
                        Application.debug("F3 was pressed, printing debug info.");
                        Application.debug("Player position X: " + gameInstance.thePlayer.getPosition().x);
                        Application.debug("Player position Y: " + gameInstance.thePlayer.getPosition().y);
                        Application.debug("Player position Z: " + gameInstance.thePlayer.getPosition().z);
                        Application.debug("Player Head Pitch: " + gameInstance.thePlayer.getheadPitch());
                        Application.debug("Player Yaw       : " + gameInstance.thePlayer.getYaw());
                        Application.debug("Vsync enabled    : " + GameSettings.vsync);
                        Application.debug("Frames per second: " + TicksAndFps.getFps());
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
