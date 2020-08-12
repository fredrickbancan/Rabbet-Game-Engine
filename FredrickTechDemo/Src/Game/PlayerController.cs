using OpenTK.Input;

namespace FredrickTechDemo
{
    /*This class is responsable for detecting input which changes the players state.
      Such as movement, inventory use, attacking etc*/
    class PlayerController
    {
        private EntityPlayer thePlayer;
        private GameInstance game;
        public PlayerController(GameInstance game)
        {
            this.game = game;
            this.thePlayer = game.thePlayer;
        }

        /*Called every tick from Input.cs if a key is being pressed. for detecting player input*/
        public void updateInput(KeyboardState keyboard)
        {
            if (!thePlayer.paused)
            {
                if (keyboard.IsKeyDown(Key.W))
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
        }

        public void updateSingleMousePressInput(MouseState mouse)
        {
            if(Input.mosueKeySinglePress(MouseButton.Left))//left mouse click
            {
                thePlayer.onLeftClick();
            }
        }

        /*Called if a new key is being pressed in a tick. Will only call for one tick if it is the same key.
          usefull for input such as opening menus, attacking, jumping, things that only need one key press.*/
        public void updateSinglePressInput(KeyboardState keyboard)
        {
            if(Input.keySinglePress(Key.E))
            {
                Input.centerMouse(); // center the mouse cursor when closing or opening menu
                Input.toggleHideMouse();
                thePlayer.togglePause();
            }

            if (Input.keySinglePress(Key.V))
            {
                thePlayer.toggleFlying();
            }

            if (Input.keySinglePress(Key.F))
            {
                thePlayer.interact();
            }

            if (Input.keySinglePress(Key.Space))
            {
                thePlayer.jump();
            }
        }
    }
}
