using RabbetGameEngine.Debugging;
using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        public static readonly string guiHudName = "hudGUI";
        public static readonly string fpsPanelName = "fpsPanel";

        /*Temporary arcade vars*/
        private static int directHitCounter = 0;
        private static int airShotCounter = 0;
        private static bool showingDirectHitPopup = false;
        private static bool showingAirShotPopup = false;
        private static int maxPopupTicks = 0;
        private static int directHitPopupTicks = 0;
        private static int airShotPopupTicks = 0;
        public static void init()
        {
            GUIManager.addNewGUIScreen(guiHudName, "Arial_Shadow");
            GUIManager.addTextPanelToGUI(guiHudName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(CustomColor.darkRed)));
            GUIManager.addTextPanelToGUI(guiHudName, "noclip", new GUITextPanel(new TextFormat(0, 0.03F).setAlign(TextAlign.RIGHT).setLine("Noclip: OFF").setPanelColor(CustomColor.darkRed)));
            GUIManager.addTextPanelToGUI(guiHudName, "label", new GUITextPanel(new TextFormat(0, 0.97F).setLine(Application.applicationName).setPanelColor(CustomColor.black)));
            GUIManager.addGUIComponentToGUI(guiHudName, "crossHair", new GUICrosshair());

            /*TEMPORARY, just for arcade stuff*/
            GUIManager.addTextPanelToGUI(guiHudName, "directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(CustomColor.flame)));
            GUIManager.hideTextPanelInGUI(guiHudName, "directHit");

            GUIManager.addTextPanelToGUI(guiHudName, "airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(CustomColor.red)));
            GUIManager.hideTextPanelInGUI(guiHudName, "airShot");

            GUIManager.addTextPanelToGUI(guiHudName, "directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + 0).setPanelColor(CustomColor.flame)));
            GUIManager.addTextPanelToGUI(guiHudName, "airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + 0).setPanelColor(CustomColor.red)));

            GUIManager.addTextPanelToGUI(guiHudName, fpsPanelName, new GUITextPanel(new TextFormat().setLine("0")));

            /*Temp arcade stuff*/
            maxPopupTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(1.5F);
        }

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
                GUIManager.getTextPanelFormatFromGUI(guiHudName, "flying").setPanelColor(CustomColor.green).setLine("Flying: ON");
            }
            else
            {
                GUIManager.getTextPanelFormatFromGUI(guiHudName, "flying").setPanelColor(CustomColor.darkRed).setLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                GUIManager.getTextPanelFormatFromGUI(guiHudName, "noclip").setPanelColor(CustomColor.green).setLine("Noclip: ON");
            }
            else
            {
                GUIManager.getTextPanelFormatFromGUI(guiHudName, "noclip").setPanelColor(CustomColor.darkRed).setLine("Noclip: OFF");
            }

            GUIManager.getTextPanelFormatFromGUI(guiHudName, "directHitCount").setLine("Direct Hits: " + directHitCounter);
            GUIManager.getTextPanelFormatFromGUI(guiHudName, "airShotCount").setLine("AirShots: " + airShotCounter);

            if (showingDirectHitPopup)
            {
                GUIManager.unHideTextPanelInGUI(guiHudName, "directHit");
            }
            else
            {
                GUIManager.hideTextPanelInGUI(guiHudName, "directHit");
            }
            if (showingAirShotPopup)
            {
                GUIManager.unHideTextPanelInGUI(guiHudName, "airShot");
            }
            else
            {
                GUIManager.hideTextPanelInGUI(guiHudName, "airShot");
            }
            DebugInfo.displayOrClearDebugInfo();
            GUIManager.rebuildTextInGUI(guiHudName);//do last, applies any changes to the text on screen.
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
                GUIManager.unHideTextPanelInGUI(guiHudName, fpsPanelName);
                string fpsstring = TicksAndFrames.fps.ToString();
                if (TicksAndFrames.fps < 75)
                {
                    GUIManager.getTextPanelFormatFromGUI(guiHudName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    GUIManager.getTextPanelFormatFromGUI(guiHudName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.yellow);
                }
                else
                {
                    GUIManager.getTextPanelFormatFromGUI(guiHudName, fpsPanelName).setLine(fpsstring).setPanelColor(CustomColor.green);
                }
            }
            else
            {
                GUIManager.hideTextPanelInGUI(guiHudName, fpsPanelName);
            }
        }
    }
}
