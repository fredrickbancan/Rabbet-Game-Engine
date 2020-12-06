using RabbetGameEngine.Debugging;
using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        /*Temporary arcade vars*/
        private int directHitCounter = 0;
        private int airShotCounter = 0;
        private bool showingDirectHitPopup = false;
        private bool showingAirShotPopup = false;
        private int maxPopupTicks = 0;
        private int directHitPopupTicks = 0;
        private int airShotPopupTicks = 0;

        public GUIHud() : base("hud", "Arial_Shadow")
        {
            addTextPanel("flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(CustomColor.darkRed)));
            addTextPanel("noclip", new GUITextPanel(new TextFormat(0, 0.03F).setAlign(TextAlign.RIGHT).setLine("Noclip: OFF").setPanelColor(CustomColor.darkRed)));
            addTextPanel("label", new GUITextPanel(new TextFormat(0, 0.97F).setLine(Application.applicationName).setPanelColor(CustomColor.black)));
            addTextPanel("directHit", new GUITextPanel(new TextFormat(0.5F, 0.64F).setAlign(TextAlign.CENTER).setLine("Direct Hit!").setPanelColor(CustomColor.flame)).hide());
            addTextPanel("airShot", new GUITextPanel(new TextFormat(0.5F, 0.67F).setAlign(TextAlign.CENTER).setLine("AIR SHOT!").setPanelColor(CustomColor.red)).hide());
            addTextPanel("directHitCount", new GUITextPanel(new TextFormat(0.1F, 0.15F).setAlign(TextAlign.RIGHT).setLine("Direct Hits: " + 0).setPanelColor(CustomColor.flame)));
            addTextPanel("airShotCount", new GUITextPanel(new TextFormat(0.1F, 0.18F).setAlign(TextAlign.RIGHT).setLine("Air Shots: " + 0).setPanelColor(CustomColor.red)));
            addTextPanel("fps", new GUITextPanel(new TextFormat().setLine("0")));
            maxPopupTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(1.5F);
        }

        public override void onTick()
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
               getTextPanel("flying").setPanelColor(CustomColor.green).setLine("Flying: ON");
            }
            else
            {
                getTextPanel("flying").setPanelColor(CustomColor.darkRed).setLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                getTextPanel("noclip").setPanelColor(CustomColor.green).setLine("Noclip: ON");
            }
            else
            {
                getTextPanel("noclip").setPanelColor(CustomColor.darkRed).setLine("Noclip: OFF");
            }

            getTextPanel("directHitCount").setLine("Direct Hits: " + directHitCounter);
            getTextPanel("airShotCount").setLine("AirShots: " + airShotCounter);

            if (showingDirectHitPopup)
            {
                unHideTextPanel("directHit");
            }
            else
            {
               hideTextPanel("directHit");
            }
            if (showingAirShotPopup)
            {
                unHideTextPanel("airShot");
            }
            else
            {
                hideTextPanel("airShot");
            }
            DebugInfo.displayOrClearDebugInfo();
            buildText();//do last, applies any changes to the text on screen.
        }

        /*Called when player lands direct hit on a cactus, TEMPORARY!*/
        public void onDirectHit()
        {
            directHitCounter++;
            directHitPopupTicks = 0;
            showingDirectHitPopup = true;
        }

        /*Called when player lands air shot on a cactus, TEMPORARY!*/
        public void onAirShot()
        {
            airShotCounter++;
            airShotPopupTicks = 0;
            showingAirShotPopup = true;
        }

        private void displayFps()
        {
            if (GameSettings.displayFps)
            {
                unHideTextPanel("fps");
                string fpsstring = TicksAndFrames.fps.ToString();
                if (TicksAndFrames.fps < 75)
                {
                    getTextPanel("fps").setLine(fpsstring).setPanelColor(CustomColor.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    getTextPanel("fps").setLine(fpsstring).setPanelColor(CustomColor.yellow);
                }
                else
                {
                    getTextPanel("fps").setLine(fpsstring).setPanelColor(CustomColor.green);
                }
            }
            else
            {
                hideTextPanel("fps");
            }
        }
    }
}
