using OpenTK.Input;

namespace RabbetGameEngine
{
    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    public static class PlayerController
    {
        private static bool[] playerActions = new bool[EntityLiving.actionsCount];
        private static KeyboardState currentKeyboardState;
        private static MouseState currentMouseState;
        /*Called every tick from Input.cs if a key is being pressed. for detecting player input*/
        public static void updateInput(KeyboardState keyboard)
        {
            if (!GameInstance.get.thePlayer.paused)
            {
                currentKeyboardState = keyboard;
                checkAndAddAction(Key.W, Action.fowards);
                checkAndAddAction(Key.S, Action.backwards);
                checkAndAddAction(Key.A, Action.strafeLeft);
                checkAndAddAction(Key.D, Action.strafeRight);
                checkAndAddAction(Key.Space, Action.jump);
                checkAndAddAction(Key.F, Action.interact);
            }
        }

        public static void updateMouseButtonInput(MouseState mouse)
        {
            if (!GameInstance.get.thePlayer.paused)
            {
                currentMouseState = mouse;
                checkAndAddAction(MouseButton.Left, Action.attack);
            }
        }

        /*Called if a new key is being pressed in a tick. Will only call for one tick if it is the same key.
          usefull for input such as opening menus, attacking, jumping, things that only need one key press.*/
        public static void updateSinglePressInput(KeyboardState keyboard)
        {
            if(Input.singleKeyPress(Key.E))
            {
                GameInstance.pauseGame();
            }

            if (Input.singleKeyPress(Key.V))
            {
                GameInstance.get.thePlayer.toggleFlying();
            }
        }

        /*if key is down, adds action to player.*/
        private static void checkAndAddAction(Key key, Action act)
        {
            if(currentKeyboardState.IsKeyDown(key))
                playerActions[(int)act] = true;
        }
        private static void checkAndAddAction(MouseButton button, Action act)//for mouse buttons
        {
            if (currentMouseState.IsButtonDown(button))
                playerActions[(int)act] = true;
        }

        /*can be called to determine if the player (user) is performing a certain action*/
        public static bool getDoingAction(Action act)
        {
           return playerActions[(int)act];
        }

        /*should be called at the end of each tick to reset inputs*/
        public static void resetActions()
        {
            for (int i = 0; i < EntityLiving.actionsCount; i++)
            {
                playerActions[i] = false;
            }
        }
    }
}
