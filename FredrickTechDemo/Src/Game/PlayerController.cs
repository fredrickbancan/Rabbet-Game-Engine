using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    public static class PlayerController
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
                Input.centerMouse(); // center the mouse cursor when closing or opening menu
                Input.toggleHideMouse();
                GameInstance.get.thePlayer.togglePause();
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
