using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.Text;

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
            GUIManager.addNewGUIScreen(mainGUIName, "Arial_Shadow");
            GUIManager.addTextPanelToGUI(mainGUIName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(CustomColor.darkRed)));
            GUIManager.addTextPanelToGUI(mainGUIName, "noclip", new GUITextPanel(new TextFormat(0, 0.03F).setAlign(TextAlign.RIGHT).setLine("Noclip: OFF").setPanelColor(CustomColor.darkRed)));
            GUIManager.addTextPanelToGUI(mainGUIName, "label", new GUITextPanel(new TextFormat(0, 0.97F).setLine(Application.applicationName).setPanelColor(CustomColor.black)));
            GUIManager.addGUIComponentToGUI(mainGUIName, "crossHair", new GUICrosshair());

            /*TEMPORARY, just for arcade stuff*/
            GUIManager.addTextPanelToGUI(mainGUIName, "directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(CustomColor.flame)));
            GUIManager.hideTextPanelInGUI(mainGUIName, "directHit");

            GUIManager.addTextPanelToGUI(mainGUIName, "airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(CustomColor.red)));
            GUIManager.hideTextPanelInGUI(mainGUIName, "airShot");

            GUIManager.addTextPanelToGUI(mainGUIName, "directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + 0).setPanelColor(CustomColor.flame)));
            GUIManager.addTextPanelToGUI(mainGUIName, "airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + 0).setPanelColor(CustomColor.red)));

            GUIManager.addTextPanelToGUI(MainGUI.mainGUIName, fpsPanelName, new GUITextPanel(new TextFormat().setLine("0")));

            /*Temp arcade stuff*/
            maxPopupTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(1.5F);
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
                GUIManager.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(CustomColor.green).setLine("Flying: ON");
            }
            else
            {
                GUIManager.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(CustomColor.darkRed).setLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                GUIManager.getTextPanelFormatFromGUI(mainGUIName, "noclip").setPanelColor(CustomColor.green).setLine("Noclip: ON");
            }
            else
            {
                GUIManager.getTextPanelFormatFromGUI(mainGUIName, "noclip").setPanelColor(CustomColor.darkRed).setLine("Noclip: OFF");
            }

            GUIManager.getTextPanelFormatFromGUI(mainGUIName, "directHitCount").setLine("Direct Hits: " + directHitCounter);
            GUIManager.getTextPanelFormatFromGUI(mainGUIName, "airShotCount").setLine("AirShots: " + airShotCounter);

            if (showingDirectHitPopup)
            {
                GUIManager.unHideTextPanelInGUI(mainGUIName, "directHit");
            }
            else
            {
                GUIManager.hideTextPanelInGUI(mainGUIName, "directHit");
            }
            if (showingAirShotPopup)
            {
                GUIManager.unHideTextPanelInGUI(mainGUIName, "airShot");
            }
            else
            {
                GUIManager.hideTextPanelInGUI(mainGUIName, "airShot");
            }
            DebugInfo.displayOrClearDebugInfo();
            GUIManager.rebuildTextInGUI(mainGUIName);//do last, applies any changes to the text on screen.
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
                GUIManager.unHideTextPanelInGUI(mainGUIName, fpsPanelName);
                string fpsstring = TicksAndFrames.fps.ToString();
                if (TicksAndFrames.fps < 75)
                {
                    GUIManager.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    GUIManager.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.yellow);
                }
                else
                {
                    GUIManager.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.green);
                }
            }
            else
            {
                GUIManager.hideTextPanelInGUI(mainGUIName, fpsPanelName);
            }
        }
    }
}
