namespace RabbetGameEngine
{
    public class GUIVideoSettings : GUI
    {
        GUIButton backButton;
        GUIButton applyButton;
        GUIDropDownButton t;
        public GUIVideoSettings() : base("videoSettings", "arial")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F, 1));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 1.5F, 1.0F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER, 1));
            addGuiComponent("title", new GUITextPanel(0, 0.15F, guiFont, ComponentAnchor.CENTER, 2).addLine("Video Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(-0.1F, -0.35F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER, 2);
            backButton.addClickListener(onBackButtonClick);
            backButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            backButton.addHoverExitListener(defaultOnButtonHoverExit);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);

            applyButton = new GUIButton(0.1F, -0.35F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Apply", guiFont, ComponentAnchor.CENTER, 2);
            if (!GameSettings.audioSettingsChanged) applyButton.disable();
            applyButton.addClickListener(onBackButtonClick);
            applyButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            applyButton.addHoverExitListener(defaultOnButtonHoverExit);
            applyButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("applyButton", applyButton);

            t = new GUIDropDownButton(this, 0, 0, 0.25F, 0.05F, Color.grey.setAlphaF(0.7F), "test", new[] { "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus", "cactus momentum" }, guiFont, ComponentAnchor.CENTER, 2);
            addGuiComponent("test", t);
        }
        private void onBackButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
    }
}
