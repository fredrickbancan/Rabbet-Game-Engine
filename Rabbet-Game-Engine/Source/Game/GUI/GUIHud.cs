namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        GUITextPanel flyPanel;
        GUITextPanel noclipPanel;
        GUITextPanel fpsPanel;
        GUITextPanel timePanel;
        public GUIHud() : base("hud", "arial")
        {
            flyPanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.TOP_RIGHT).addLine("Flying: OFF").setDefaultLineColor(Color.darkRed);
            noclipPanel = new GUITextPanel(0, -0.025F, guiFont, ComponentAnchor.TOP_RIGHT).addLine("Noclip: OFF").setDefaultLineColor(Color.darkRed);
            fpsPanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.TOP_LEFT).addLine("0");
            timePanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.CENTER_TOP).addLine("00:00").setDefaultLineColor(Color.grey);
            addGuiComponent("flying",flyPanel);
            addGuiComponent("noclip", noclipPanel);
            addGuiComponent("label", new GUITextPanel(0, 0.0F, guiFont, ComponentAnchor.BOTTOM_LEFT).addLine(Application.applicationName).setDefaultLineColor(Color.black));
            addGuiComponent("fps", fpsPanel);
            addGuiComponent("time", timePanel);
            addGuiComponent("crosshair", new GUICrosshair());
        }

        public override void onUpdate()
        {
            base.onUpdate();
            displayFps();
            if (GameInstance.get.thePlayer.getIsFlying())
            {
               flyPanel.setDefaultLineColor(Color.green).clear().addLine("Flying: ON");
            }
            else
            {
                flyPanel.setDefaultLineColor(Color.darkRed).clear().addLine("Flying: OFF");
            }

            if (GameSettings.noclip)
            {
                noclipPanel.setDefaultLineColor(Color.green).clear().addLine("Noclip: ON");
            }
            else
            {
               noclipPanel.setDefaultLineColor(Color.darkRed).clear().addLine("Noclip: OFF");
            }
            timePanel.clear().addLine(GameInstance.get.currentWorld.get24HourTimeString());
            timePanel.updateRenderData();
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
                    fpsPanel.clear().addLine(fpsstring).setDefaultLineColor(Color.red);
                }
                else if (TicksAndFrames.fps < 120)
                {
                    fpsPanel.clear().addLine(fpsstring).setDefaultLineColor(Color.yellow);
                }
                else
                {
                    fpsPanel.clear().addLine(fpsstring).setDefaultLineColor(Color.green);
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
