using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    class PlayerController
    {
        private EntityPlayer thePlayer;
        public PlayerController(EntityPlayer thePlayer)
        {
            this.thePlayer = thePlayer;
        }

        /*Called every tick from Input.cs if a key is being pressed. for detecting player input*/
        public void updateInput(KeyboardState keyboard)
        {
            if(keyboard.IsKeyDown(Key.W))
            {
                thePlayer.walkFowards();
            }
            if (keyboard.IsKeyDown(Key.S))
            {
                thePlayer.walkBackwards();
            }
            if (keyboard.IsKeyDown(Key.A))
            {
                thePlayer.strafeLeft();
            }
            if (keyboard.IsKeyDown(Key.D))
            {
                thePlayer.strafeRight();
            }
        }

        /*Called if a new key is being pressed in a tick. Will only call for one tick if it is the same key.
          usefull for input such as opening menus, attacking, jumping, things that only need one key press.*/
        public void updateSinglePressInput(KeyboardState keyboard)
        {
            if(keyboard.IsKeyDown(Key.I))
            {
                Input.centerMouse(); // center the mouse cursor when closing or opening menu
                Input.toggleHideMouse();
                thePlayer.toggleOpenMenu();
            }
        }
    }
}
