namespace RabbetGameEngine
{
    public class GUIHud : GUI
    {
        GUITextPanel fpsPanel;
        GUITextPanel timePanel;
        public GUIHud() : base("hud", "arial")
        {
            fpsPanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.TOP_LEFT).addLine("0");
            timePanel = new GUITextPanel(0, 0, guiFont, ComponentAnchor.CENTER_TOP).addLine("00:00").setDefaultLineColor(Color.grey);
            //addGuiComponent("label", new GUITextPanel(0, 0.0F, guiFont, ComponentAnchor.BOTTOM_LEFT).addLine(Application.applicationName).setDefaultLineColor(Color.black));
            addGuiComponent("fps", fpsPanel);
            addGuiComponent("time", timePanel);
            //addGuiComponent("crosshair", new GUICrosshair());
        }

        public override void onUpdate(bool isFrameUpdate)
        {
            base.onUpdate(isFrameUpdate);
            if (isFrameUpdate) return;
            displayFps();
            timePanel.clear().addLine(GameInstance.get.currentWorld.theSky.get12HourTimeString());
            timePanel.updateRenderData();
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
