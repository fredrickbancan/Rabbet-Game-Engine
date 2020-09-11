using OpenTK.Input;

namespace Coictus
{

    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    public static class PlayerController//TODO: impliment custom bindings in gamesettings
    {
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

            if (Input.singleKeyPress(Key.F))
            {
                GameInstance.get.thePlayer.interact();
            }
        }

        /*if key is down, adds action to player.*/
        private static void checkAndAddAction(Key key, Action act)
        {
            if(currentKeyboardState.IsKeyDown(key))
            GameInstance.get.thePlayer.addAction(act);
        }
        private static void checkAndAddAction(MouseButton button, Action act)//for mouse buttons
        {
            if (currentMouseState.IsButtonDown(button))
                GameInstance.get.thePlayer.addAction(act);
        }
    }
}
