using OpenTK;

namespace FredrickTechDemo
{
    /*Class for loading settings from file, applying settings and containing settings.*/
    static class GameSettings
    {
        public static float fov = 80; //fov of player camera
        public static float mouseSensitivity = 0.25F;
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
