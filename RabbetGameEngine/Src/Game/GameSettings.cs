using OpenTK;

namespace RabbetGameEngine
{
    /*Class for loading settings from file, applying settings and containing settings.*/
    static class GameSettings
    {
        public static float fov = 80; //fov of player camera
        public static float mouseSensitivity = 0.08F;
        public static bool vsync = false;
        public static bool displayFps = true;
        public static bool debugScreen = true;
        public static bool drawHitboxes = false;
        public static bool superSampling = true;
        public static bool fullscreen = false;
        public static bool noclip = false;//disables player clipping
        public static void loadSettings()
        {
            if(vsync)
            {
                GameInstance.get.VSync = VSyncMode.Adaptive;
            }
            else
            {
                GameInstance.get.VSync = VSyncMode.Off;
            }
        }
    }
}
