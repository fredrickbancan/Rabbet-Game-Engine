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
        public static float barrelDistortionStrength = defaultFov * 0.01F;
        public static float barrelDistortionCylRatio = 2.0F;

        public static float defaultBrightness = 1.0F;
        public static float ditherScale = 1.0F;
        public static float defaultMouseSensitivity = 0.08F;

        /// <summary>
        /// This setting MUST be between 0 and 1
        /// </summary>
        public static float defaultMasterVolume = 0.2F;

        public static string defaultFont = "arial";

        public static bool defaultVsync = false;
        public static float defaultMaxDrawDistance = 1024.0F;
        public static bool displayFps = true;
        public static bool debugScreen = false;
        public static bool entityLabels = false;
        public static bool drawHitboxes = false;
        public static bool fullscreen = false;
        public static bool noclip = false;

        public static int defaultSuperSampleIndex = 0;

        private static string[] superSampleTitles = new string[]
        {
            "1x",
            "1.5x",
            "2x",
            "2.5x",
            "3x",
            "3.5x",
            "4x"
        };
        private static float[] superSampleFloats = new float[]
        {
            1.0F,
            1.5F,
            2.0F,
            2.5F,
            3.0F,
            3.5F,
            4.0F
        };
        public static List<Setting> videoSettings = new List<Setting>();
        public static List<Setting> advVideoSettings = new List<Setting>();
        public static List<Setting> audioSettings = new List<Setting>();
        public static List<Setting> controls = new List<Setting>();
        public static List<Setting> bindings = new List<Setting>();

        public static Setting fov = new Setting("Field of View", SettingType.FLOAT, videoSettings).setRange(60.0F, 120.0F).setDisplayRange(60.0F, 120.0F).setFloatValue(defaultFov);
        public static Setting maxDrawDistance = new Setting("Draw Distance", SettingType.FLOAT, videoSettings).setRange(128.0F, 2048.0F).setDisplayRange(128.0F, 2048.0F).setFloatValue(defaultMaxDrawDistance);
        public static Setting brightness = new Setting("Brightness", SettingType.FLOAT, videoSettings).setRange(0.5F, 1.1F).setDisplayRange(50.0F, 110.0F).setFloatValue(defaultBrightness);
        public static Setting barrelDistortion = new Setting("Barrel Distortion Strength", SettingType.FLOAT, videoSettings).setRange(0.0F, 5.0F).setDisplayRange(0.0F, 500.0F).setFloatValue(defaultFov * 0.01F);//when changed, value should be set to fov * 0.01F * value. Needs to be updated with FOV
        public static Setting superSample = new Setting("Super Sampling", SettingType.LIST_FLOAT, videoSettings).setListTitles(superSampleTitles).setListFloats(superSampleFloats).setListIndex(defaultSuperSampleIndex);
        public static Setting vsync = new Setting("Vertical Sync", SettingType.BOOL, videoSettings).setBoolValue(defaultVsync);
       
        public static Setting masterVolume = new Setting("Volume", SettingType.FLOAT, audioSettings).setRange(0.0F, 1.0F).setDisplayRange(0.0F, 100.0F).setFloatValue(defaultMasterVolume);

        public static Setting mouseSensetivity = new Setting("Mouse Sensitivity", SettingType.FLOAT, controls).setRange(0.01F, 10.0F).setDisplayRange(1.0F, 1000.0F).setFloatValue(defaultMouseSensitivity);

        public static void loadSettings()
        {
            //load settings from file here

            GameInstance.get.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
        }

        public static void applyVideoSettings()
        {
            //TODO: Update all dependant uniforms and other values
            videoSettingsChanged = false;
        }

        public static void applyAudioSettings()
        {
            audioSettingsChanged = false;
        }

        public static void applyControllerSettings()
        {
            controlsChanged = false;
        }

        public static void applyBindingSettings()
        {
            bindingsChanged = false;
        }
    }
}
