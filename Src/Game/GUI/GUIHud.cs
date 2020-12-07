using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        public GUIHud() : base("hud", "Arial_Shadow")
        {
            addTextPanel("flying", new GUITextPanel(new TextFormat().setAlign(TextAlign.RIGHT).setLine("Flying: OFF").setPanelColor(CustomColor.darkRed)));
            addTextPanel("noclip", new GUITextPanel(new TextFormat(0, 0.03F).setAlign(TextAlign.RIGHT).setLine("Noclip: OFF").setPanelColor(CustomColor.darkRed)));
            addTextPanel("label", new GUITextPanel(new TextFormat(0, 0.97F).setLine(Application.applicationName).setPanelColor(CustomColor.black)));
            addTextPanel("fps", new GUITextPanel(new TextFormat().setLine("0")));
            addGuiComponent("crosshair", new GUICrosshair());
        }

        public override void onUpdate()
        {
            base.onUpdate();
            displayFps();
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
            buildAllText();//do last, applies any changes to the text on screen.
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
