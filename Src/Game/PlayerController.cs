using OpenTK.Windowing.GraphicsLibraryFramework;

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
        public static void updateInput(KeyboardState keyboard , MouseState mouse)
        {
            currentKeyboardState = keyboard;
            currentMouseState = mouse;
            for (int i = 0; i < GameSettings.bindings.Count; i++)
            {
                if(GameSettings.bindings[i].isMouseButton)
                {
                    checkAndAddAction((MouseButton)GameSettings.bindings[i].code, GameSettings.bindings[i].act);
                }
                else
                {
                    checkAndAddAction((Keys)GameSettings.bindings[i].code, GameSettings.bindings[i].act);
                }
            }
        }

       

        /*Called if a new key is being pressed in a tick. Will only call for one tick if it is the same key.
          usefull for input such as opening menus, attacking, jumping, things that only need one key press.*/
        public static void updateSinglePressInput(KeyboardState keyboard)
        {

            if (Input.singleKeyPress(Keys.V))
            {
                GameInstance.get.thePlayer.toggleFlying();
            }
        }

        /*if key is down, adds action to player.*/
        private static void checkAndAddAction(Keys key, EntityAction act)
        {
            if(currentKeyboardState.IsKeyDown(key))
                playerActions[(int)act] = true;
        }
        private static void checkAndAddAction(MouseButton button, EntityAction act)//for mouse buttons
        {
            if (currentMouseState.IsButtonDown(button))
                playerActions[(int)act] = true;
        }

        /*can be called to determine if the player (user) is performing a certain action*/
        public static bool getDoingAction(EntityAction act)
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
