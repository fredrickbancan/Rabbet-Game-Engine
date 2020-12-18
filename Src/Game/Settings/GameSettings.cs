using System.Collections.Generic;

namespace RabbetGameEngine
{
    //TODO: Make settings able to be itterated through by gui systems with support for info hover panels, sliders, buttons etc.
    //TODO: Add descriptions and working drop down panels to show them.
    static class GameSettings
    {
        public static bool videoSettingsChanged = false;
        public static bool audioSettingsChanged = false;
        public static bool controlsChanged = false;
        public static bool bindingsChanged = false;
        public static float defaultFov = 80; //fov of player camera
        public static float barrelDistortionStrength = defaultFov * 0.00F;
        public static float barrelDistortionCylRatio = 2.0F;

        public static float defaultBrightness = 1.0F;
        public static float ditherScale = 1.0F;
        public static float defaultMouseSensitivity = 0.08F;

        /// <summary>
        /// This setting MUST be between 0 and 1
        /// </summary>
        public static float defaultMasterVolume = 0.2F;

        public static string defaultFont = "Arial_Shadow";

        public static bool defaultVsync = false;
        public static float defaultMaxDrawDistance = 1024.0F;
        public static bool displayFps = true;
        public static bool debugScreen = false;
        public static bool entityLabels = false;
        public static bool drawHitboxes = false;
        public static bool fullscreen = false;
        public static bool noclip = false;

        public static List<Setting> videoSettings = new List<Setting>();
        public static List<Setting> advVideoSettings = new List<Setting>();
        public static List<Setting> audioSettings = new List<Setting>();
        public static List<Setting> controls = new List<Setting>();
        public static List<Setting> bindings = new List<Setting>();

        public static Setting fov = new Setting("Field of View", SettingType.FLOAT, videoSettings).setRange(60.0F, 120.0F).setFloatValue(defaultFov);
        public static Setting drawDistance = new Setting("Draw Distance", SettingType.FLOAT, videoSettings).setRange(256.0F, 2048.0F).setFloatValue(defaultMaxDrawDistance);
        public static Setting vsync = new Setting("Vertical Sync", SettingType.BOOL, videoSettings).setBoolValue(defaultVsync);
        public static Setting brightness = new Setting("Brightness", SettingType.FLOAT, videoSettings).setRange(0.5F, 1.1F).setFloatValue(defaultBrightness);
        public static Setting barrelDistortion = new Setting("Barrel Distortion Strength", SettingType.FLOAT, advVideoSettings).setRange(0.0F, 5.0F).setFloatValue(defaultFov * 0.01F);//when changed, value should be set to fov * 0.01F * value. Needs to be updated with FOV
       
        public static Setting masterVolume = new Setting("Volume", SettingType.FLOAT, audioSettings).setRange(0.0F, 1.0F).setFloatValue(defaultMasterVolume);

        public static Setting mouseSensetivity = new Setting("Mouse Sensitivity", SettingType.FLOAT, controls).setRange(0.01F, 10.0F).setFloatValue(defaultMouseSensitivity);

        public static void loadSettings()
        {
            //load settings from file here

            GameInstance.get.VSync = OpenTK.Windowing.Common.VSyncMode.On;
        }
    }
}
