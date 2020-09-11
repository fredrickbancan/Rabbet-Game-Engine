using OpenTK.Input;

namespace Coictus
{
    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    public static class PlayerController//TODO: impliment an actions list. An array of actions which have been requested by input. The list should consist of a set size array of action enums, an action enum needs to be created with different actions. e.g: use, attack, duck, fowards, backwards etc. And then an onTick() function must be added to playercontroller.cs to act on this list of actions and reset it.
    {
        /*Called every tick from Input.cs if a key is being pressed. for detecting player input*/
        public static void updateInput(KeyboardState keyboard)
        {
            if (!GameInstance.get.thePlayer.paused)
            {
                if (keyboard.IsKeyDown(Key.W))
                {
                    GameInstance.get.thePlayer.walkFowards();
                }
                if (keyboard.IsKeyDown(Key.S))
                {
                    GameInstance.get.thePlayer.walkBackwards();
                }
                if (keyboard.IsKeyDown(Key.A))
                {
                    GameInstance.get.thePlayer.strafeLeft();
                }
                if (keyboard.IsKeyDown(Key.D))
                {
                    GameInstance.get.thePlayer.strafeRight();
                }
            }
        }

        public static void updateSingleMousePressInput(MouseState mouse)
        {
            if(Input.mouseSingleKeyPress(MouseButton.Left))//left mouse click
            {
                GameInstance.get.thePlayer.onLeftClick();
            }
        }

        /*Called if a new key is being pressed in a tick. Will only call for one tick if it is the same key.
          usefull for input such as opening menus, attacking, jumping, things that only need one key press.*/
        public static void updateSinglePressInput(KeyboardState keyboard)
        {
            if(Input.keySinglePress(Key.E))
            {
                GameInstance.pauseGame();
            }

            if (Input.keySinglePress(Key.V))
            {
                GameInstance.get.thePlayer.toggleFlying();
            }

            if (Input.keySinglePress(Key.F))
            {
                GameInstance.get.thePlayer.interact();
            }

            if (Input.keySinglePress(Key.Space))
            {
                GameInstance.get.thePlayer.jump();
            }
        }
    }
}
