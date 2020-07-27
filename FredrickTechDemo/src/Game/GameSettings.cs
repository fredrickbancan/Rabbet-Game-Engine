using OpenTK;

namespace FredrickTechDemo
{
    static class GameSettings
    {
        public static float fov = 80; //fov of player camera
        public static bool vsync = false;// DO NOT set to true for now, causes game loop speed to be limited by screen refresh rate.

        public static void loadSettings(GameInstance game)
        {
            if(vsync)
            {
                game.VSync = VSyncMode.Adaptive;
            }
            else
            {
                game.VSync = VSyncMode.Off;
            }
        }
    }
}
