using OpenTK.Mathematics;
using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        public GUIHud() : base("hud", "Arial_Shadow")
        {
            addTextPanel("flying", new GUITextPanel(new Vector2(0,0.04F), ComponentAlignment.RIGHT).addLine("Flying: OFF").setPanelColor(Color.darkRed));
            addTextPanel("noclip", new GUITextPanel(new Vector2(0, 0.07F), ComponentAlignment.RIGHT).addLine("Noclip: OFF").setPanelColor(Color.darkRed));
            addTextPanel("label", new GUITextPanel(new Vector2(0, 1.0115F), ComponentAlignment.LEFT).addLine(Application.applicationName).setPanelColor(Color.black));
            addTextPanel("fps", new GUITextPanel(new Vector2(0, 0.04F), ComponentAlignment.LEFT).addLine("0"));
            addGuiComponent("crosshair", new GUICrosshair());
        }

        public override void onUpdate()
        {
            base.onUpdate();
            displayFps();
            if (GameInstance.get.thePlayer.getIsFlying())
            {
               getTextPanel("flying").setPanelColor(Color.green).clear().addLine("Flying: ON");
            }
            else
            {
                getTextPanel("flying").setPanelColor(Color.darkRed).clear().addLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                getTextPanel("noclip").setPanelColor(Color.green).clear().addLine("Noclip: ON");
            }
            else
            {
                getTextPanel("noclip").setPanelColor(Color.darkRed).clear().addLine("Noclip: OFF");
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
                    getTextPanel("fps").clear().addLine(fpsstring).setPanelColor(Color.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    getTextPanel("fps").clear().addLine(fpsstring).setPanelColor(Color.yellow);
                }
                else
                {
                    getTextPanel("fps").clear().addLine(fpsstring).setPanelColor(Color.green);
                }
            }
            else
            {
                hideTextPanel("fps");
            }
        }
    }
}
