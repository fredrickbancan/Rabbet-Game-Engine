using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.GUI.Text;

namespace RabbetGameEngine
{
    /*Abstraction of the main gui for RabbetGameEngine*/
    public static class MainGUI
    {
        public static readonly string mainGUIName = "mainGUI";
        public static readonly string fpsPanelName = "fpsPanel";

        /*Temporary arcade vars*/
        private static int directHitCounter = 0;
        private static int airShotCounter = 0;
        private static bool showingDirectHitPopup = false;
        private static bool showingAirShotPopup = false;
        private static int maxPopupTicks = 0;
        private static int directHitPopupTicks = 0;
        private static int airShotPopupTicks = 0;

        /*Initialize this gui*/
        public static void init()
        {
            GUIHandler.addNewGUIScreen(mainGUIName, "Trebuchet");
            GUIHandler.addTextPanelToGUI(mainGUIName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(CustomColor.darkRed)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "noclip", new GUITextPanel(new TextFormat(0, 0.03F).setAlign(TextAlign.RIGHT).setLine("Noclip: OFF").setPanelColor(CustomColor.darkRed)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "label", new GUITextPanel(new TextFormat(0, 0.97F).setLine("Rabbet Game Engine Version " + Application.version).setPanelColor(CustomColor.black)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "help", new GUITextPanel(new TextFormat(0.5F, 0).setAlign(TextAlign.CENTER).setLines(new string[] { "Press 'W,A,S,D and SPACE' to move. Move mouse to look around.", "Tap 'V' to toggle flying. Tap 'E' to release mouse.", "Walk up to tank and press F to drive, Left click to fire.", "Press 'ESC' to close game.", "Press F3 to show/hide debug menu.", "press F4 to enable/disable drawing hitboxes.", "press F5 to enable/disable noclip.", "press F12 to enable fullscreen." }).setFontSize(0.015F).setPanelColor(CustomColor.black)));
            GUIHandler.addGUIComponentToGUI(mainGUIName, "crossHair", new GUICrosshair());

            /*TEMPORARY, just for arcade stuff*/
            GUIHandler.addTextPanelToGUI(mainGUIName, "directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(CustomColor.flame)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "directHit");

            GUIHandler.addTextPanelToGUI(mainGUIName, "airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(CustomColor.red)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "airShot");

            GUIHandler.addTextPanelToGUI(mainGUIName, "directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + 0).setPanelColor(CustomColor.flame)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + 0).setPanelColor(CustomColor.red)));

            GUIHandler.addTextPanelToGUI(MainGUI.mainGUIName, fpsPanelName, new GUITextPanel(new TextFormat().setLine("0")));

            /*Temp arcade stuff*/
            maxPopupTicks = (int)TicksAndFps.getNumOfTicksForSeconds(1.5F);
        }

        /*Called every tick*/
        public static void onTick()
        {
            displayFps();
            /*Temporary, for arcade popups.*/
            if (showingDirectHitPopup)
            {
                directHitPopupTicks++;
                if (directHitPopupTicks >= maxPopupTicks)
                {
                    showingDirectHitPopup = false;
                    directHitPopupTicks = 0;
                }
            }

            if (showingAirShotPopup)
            {
                airShotPopupTicks++;
                if (airShotPopupTicks >= maxPopupTicks)
                {
                    showingAirShotPopup = false;
                    airShotPopupTicks = 0;
                }
            }
            if (GameInstance.get.thePlayer.getIsFlying())
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(CustomColor.green).setLine("Flying: ON");
            }
            else
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(CustomColor.darkRed).setLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "noclip").setPanelColor(CustomColor.green).setLine("Noclip: ON");
            }
            else
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "noclip").setPanelColor(CustomColor.darkRed).setLine("Noclip: OFF");
            }

            GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "directHitCount").setLine("Direct Hits: " + directHitCounter);
            GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "airShotCount").setLine("AirShots: " + airShotCounter);

            if (showingDirectHitPopup)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, "directHit");
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(mainGUIName, "directHit");
            }
            if (showingAirShotPopup)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, "airShot");
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(mainGUIName, "airShot");
            }

            DebugInfo.displayOrClearDebugInfo();
            GUIHandler.rebuildTextInGUI(mainGUIName);//do last, applies any changes to the text on screen.
        }

        /*Called when player lands direct hit on a cactus, TEMPORARY!*/
        public static void onDirectHit()
        {
            directHitCounter++;
            directHitPopupTicks = 0;
            showingDirectHitPopup = true;
        }

        /*Called when player lands air shot on a cactus, TEMPORARY!*/
        public static void onAirShot()
        {
            airShotCounter++;
            airShotPopupTicks = 0;
            showingAirShotPopup = true;
        }

        /*Called every second by the TicksAndFps class to display new fps if setting is on*/
        public static void displayFps()
        {
            if (GameSettings.displayFps)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, fpsPanelName);
                string fpsstring = TicksAndFps.fps.ToString();
                if (TicksAndFps.fps < 75)
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.red);
                }
                else if (TicksAndFps.fps < 120)
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.yellow);
                }
                else
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.green);
                }
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(mainGUIName, fpsPanelName);
            }
        }
    }
}
