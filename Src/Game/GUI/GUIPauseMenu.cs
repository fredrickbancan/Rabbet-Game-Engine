namespace RabbetGameEngine
{
    public class GUIPauseMenu : GUI
    {
        public GUIPauseMenu() : base("pauseMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(CustomColor.black, 0.5F));
        }
    }
}
