using OpenTK.Windowing.Common;

namespace RabbetGameEngine
{
    /*Class for loading settings from file, applying settings and containing settings.*/
    static class GameSettings
    {
        public static float fov = 80; //fov of player camera
        public static float mouseSensitivity = 0.08F;

        /// <summary>
        /// This setting MUST be between 0 and 1
        /// </summary>
        public static float masterVolume = 0.05F;

        public static string defaultFont = "Arial_Shadow";

        public static bool vsync = false;
        public static float maxDrawDistance = 1000.0F;
        public static bool displayFps = true;
        public static bool debugScreen = false;
        public static bool entityLabels = false;
        public static bool drawHitboxes = false;
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
