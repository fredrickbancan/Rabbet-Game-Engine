using OpenTK.Input;

namespace FredrickTechDemo
{
    class PlayerController
    {
        private EntityPlayer thePlayer;
        public PlayerController(EntityPlayer thePlayer)
        {
            this.thePlayer = thePlayer;
        }
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
        public void updateSinglePressInput(KeyboardState keyboard)
        {

        }
    }
}
