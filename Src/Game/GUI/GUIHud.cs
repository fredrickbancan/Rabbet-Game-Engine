namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        GUITextPanel flyPanel;
        GUITextPanel noclipPanel;
        GUITextPanel fpsPanel;
        public GUIHud() : base("hud", "arial")
        {
            flyPanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.TOP_RIGHT).addLine("Flying: OFF").setPanelColor(Color.darkRed);
            noclipPanel = new GUITextPanel(0, -0.025F, guiFont, ComponentAnchor.TOP_RIGHT).addLine("Noclip: OFF").setPanelColor(Color.darkRed);
            fpsPanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.TOP_LEFT).addLine("0");
            addGuiComponent("flying",flyPanel);
            addGuiComponent("noclip", noclipPanel);
            addGuiComponent("label", new GUITextPanel(0, 0.0F, guiFont, ComponentAnchor.BOTTOM_LEFT).addLine(Application.applicationName).setPanelColor(Color.black));
            addGuiComponent("fps", fpsPanel);
            addGuiComponent("crosshair", new GUICrosshair());
        }

        public override void onUpdate()
        {
            base.onUpdate();
            displayFps();
            if (GameInstance.get.thePlayer.getIsFlying())
            {
               flyPanel.setPanelColor(Color.green).clear().addLine("Flying: ON");
            }
            else
            {
                flyPanel.setPanelColor(Color.darkRed).clear().addLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                noclipPanel.setPanelColor(Color.green).clear().addLine("Noclip: ON");
            }
            else
            {
               noclipPanel.setPanelColor(Color.darkRed).clear().addLine("Noclip: OFF");
            }
            flyPanel.updateRenderData();
            noclipPanel.updateRenderData();
        }

        private void displayFps()
        {
            if (GameSettings.displayFps)
            {
                fpsPanel.unHide();
                string fpsstring = TicksAndFrames.fps.ToString();
                if (TicksAndFrames.fps < 75)
                {
                    fpsPanel.clear().addLine(fpsstring).setPanelColor(Color.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    fpsPanel.clear().addLine(fpsstring).setPanelColor(Color.yellow);
                }
                else
                {
                    fpsPanel.clear().addLine(fpsstring).setPanelColor(Color.green);
                }
                fpsPanel.updateRenderData();
            }
            else
            {
                fpsPanel.hide();
            }
        }
    }
}
