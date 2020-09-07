using Coictus.Debugging;
using Coictus.GUI;
using Coictus.GUI.Text;
using System;
using System.Drawing;

namespace Coictus
{
    /*Abstraction of the main gui for Coictus*/
    public static class MainGUI
    {
        public static readonly String mainGUIName = "mainGUI";
        public static readonly String fpsPanelName = "fpsPanel";

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
            GUIHandler.addTextPanelToGUI(mainGUIName, "flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(Color.darkRed)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "label", new GUITextPanel(new TextFormat(0, 0.97F).setLine("Coictus Version " + Application.version).setPanelColor(Color.black)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "help", new GUITextPanel(new TextFormat(0.5F, 0).setAlign(TextAlign.CENTER).setLines(new string[] { "Press 'W,A,S,D and SPACE' to move. Move mouse to look around.", "Tap 'V' to toggle flying. Tap 'E' to release mouse.", "Walk up to tank and press F to drive, Left click to fire.", "Press 'ESC' to close game.", "Press F3 to show/hide debug menu.", "press F4 to enable/disable drawing hitboxes." }).setFontSize(0.015F).setPanelColor(Color.black)));
            GUIHandler.addGUIComponentToGUI(mainGUIName, "crossHair", new GUICrosshair());

            /*TEMPORARY, just for arcade stuff*/
            GUIHandler.addTextPanelToGUI(mainGUIName, "directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(Color.flame)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "directHit");

            GUIHandler.addTextPanelToGUI(mainGUIName, "airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(Color.red)));
            GUIHandler.hideTextPanelInGUI(mainGUIName, "airShot");

            GUIHandler.addTextPanelToGUI(mainGUIName, "directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + 0).setPanelColor(Color.flame)));
            GUIHandler.addTextPanelToGUI(mainGUIName, "airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + 0).setPanelColor(Color.red)));

            GUIHandler.addTextPanelToGUI(MainGUI.mainGUIName, fpsPanelName, new GUITextPanel(new TextFormat().setLine("0")));

            /*Temp arcade stuff*/
            maxPopupTicks = (int)TicksAndFps.getNumOfTicksForSeconds(1.5F);
        }

        /*Called every tick*/
        public static void onTick()
        {
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
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(Color.darkGreen).setLine("Flying: ON");
            }
            else
            {
                GUIHandler.getTextPanelFormatFromGUI(mainGUIName, "flying").setPanelColor(Color.darkRed).setLine("Flying: OFF");
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
        public static void displayFps(int fps)
        {
            if (GameSettings.displayFps)
            {
                GUIHandler.unHideTextPanelInGUI(mainGUIName, fpsPanelName);
                String fpsString = fps.ToString();
                if (fps < 75)
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsString).setPanelColor(Color.red);
                }
                else if (fps < 120)
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsString).setPanelColor(Color.yellow);
                }
                else
                {
                    GUIHandler.getTextPanelFormatFromGUI(mainGUIName, fpsPanelName).setLine(fpsString).setPanelColor(Color.green);
                }
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(mainGUIName, fpsPanelName);
            }
        }
    }
}
