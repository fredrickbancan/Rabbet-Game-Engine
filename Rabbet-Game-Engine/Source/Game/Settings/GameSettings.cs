using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    //TODO: Add descriptions and working drop down panels to show them.

    public static class GameSettings
    {
        public static bool videoSettingsChanged = false;
        public static bool audioSettingsChanged = false;
        public static bool controlsChanged = false;
        public static bool bindingsChanged = false;
        public static readonly float defaultFov = 80; //fov of player camera
        public static readonly float defaultBarrelDistortionStrength = 0;
        public static readonly float barrelDistortionCylRatio = 2.0F;

        public static readonly float defaultBrightness = 1.0F;
        public static readonly float defaultMouseSensitivity = 0.025F;

        public static readonly float defaultMasterVolume = 0.2F;

        public static readonly string defaultFont = "arial";

        public static readonly bool defaultVsync = false;
        public static readonly float defaultMaxDrawDistance = 128;
        public static bool displayFps = true;
        public static bool debugScreen = false;
        public static bool entityLabels = false;
        public static bool drawHitboxes = false;
        public static bool fullscreen = false;
        public static bool noclip = false;

        public static readonly int defaultRenderScaleIndex = 2;

        private static readonly string[] renderScaleTitles = new string[]
        { 
            "0.25x",
            "0.5x",
            "1x",
            "1.5x",
            "2x",
            "2.5x",
            "3x",
            "3.5x",
            "4x"
        };
        private static readonly float[] superSampleFloats = new float[]
        {
            0.25F,
            0.5F,
            1.0F,
            1.5F,
            2.0F,
            2.5F,
            3.0F,
            3.5F,
            4.0F
        };

        public static List<ControlBinding> bindings = new List<ControlBinding>();
        public static List<Setting> controlsSettings = new List<Setting>();
        public static List<Setting> videoSettings = new List<Setting>();
        public static List<Setting> audioSettings = new List<Setting>();

        public static Setting fov = new Setting("Field of View", SettingType.FLOAT, videoSettings).setRange(60.0F, 120.0F).setDisplayRange(60.0F, 120.0F).setFloatValue(defaultFov);
        public static Setting maxDrawDistance = new Setting("Draw Distance", SettingType.FLOAT, videoSettings).setRange(128.0F, 2048.0F).setDisplayRange(128.0F, 2048.0F).setFloatValue(defaultMaxDrawDistance);
        public static Setting brightness = new Setting("Brightness", SettingType.FLOAT, videoSettings).setRange(0.5F, 1.1F).setDisplayRange(50.0F, 110.0F).setFloatValue(defaultBrightness);
        public static Setting barrelDistortion = new Setting("Barrel Distortion Strength", SettingType.FLOAT, videoSettings).setRange(0.0F, 5.0F).setDisplayRange(0.0F, 500.0F).setFloatValue(defaultBarrelDistortionStrength);//when changed, value should be set to fov * 0.01F * value. Needs to be updated with FOV
        public static Setting renderScale = new Setting("Render Scale", SettingType.LIST_FLOAT, videoSettings).setListTitles(renderScaleTitles).setListFloats(superSampleFloats).setListIndex(defaultRenderScaleIndex);
        public static Setting vsync = new Setting("Vertical Sync", SettingType.BOOL, videoSettings).setBoolValue(defaultVsync);
       
        public static Setting masterVolume = new Setting("Master Volume", SettingType.FLOAT, audioSettings).setRange(0.0F, 1.0F).setDisplayRange(0.0F, 100.0F).setFloatValue(defaultMasterVolume);

        public static Setting mouseSensetivity = new Setting("Mouse Sensitivity", SettingType.FLOAT, controlsSettings).setRange(0.01F, 1.0F).setDisplayRange(1.0F, 100.0F).setFloatValue(defaultMouseSensitivity);

        public static ControlBinding walkFowardsBind = new ControlBinding(EntityAction.fowards, Keys.W);
        public static ControlBinding strafeLeftBind = new ControlBinding(EntityAction.strafeLeft, Keys.A);
        public static ControlBinding strafeRightBind = new ControlBinding(EntityAction.strafeRight, Keys.D);
        public static ControlBinding walkBackwardsBind = new ControlBinding(EntityAction.backwards, Keys.S);
        public static ControlBinding jumpBind = new ControlBinding(EntityAction.jump, Keys.Space);
        public static ControlBinding interactBind = new ControlBinding(EntityAction.interact, Keys.F);
        public static ControlBinding duckBind = new ControlBinding(EntityAction.duck, Keys.LeftControl);
        public static ControlBinding sprintBind = new ControlBinding(EntityAction.sprint, Keys.LeftShift);
        public static ControlBinding attackBind = new ControlBinding(EntityAction.attack, MouseButton.Left); 

        public static void loadSettings()
        {
            //load settings from file here

            GameInstance.get.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
        }

        public static void applyVideoSettings()
        {
            videoSettingsChanged = false;
            GameInstance.get.onVideoSettingsChanged();
            GameInstance.get.VSync =vsync.boolValue ? OpenTK.Windowing.Common.VSyncMode.On : OpenTK.Windowing.Common.VSyncMode.Off;
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
