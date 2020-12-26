namespace RabbetGameEngine
{
    public class GUIControlsSettings : GUI
    {
        GUIButton backButton;
        GUIButton applyButton;
        public GUIControlsSettings() : base("controlsSettings", "arial")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 1.5F, 1.0F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("title", new GUITextPanel(0, 0, guiFont, ComponentAnchor.CENTER_TOP, 1).addLine("Controls Settings").setFontSize(0.4F).setPanelColor(Color.white));
            
            backButton = new GUIButton(-0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER_BOTTOM, 1).clearClickListeners();
            backButton.addClickListener(onBackButtonClick);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);
           
            applyButton = new GUIButton(0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Apply", guiFont, ComponentAnchor.CENTER_BOTTOM, 1);
            if (!GameSettings.audioSettingsChanged) applyButton.disable();
            applyButton.addClickListener(onBackButtonClick);
            applyButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("applyButton", applyButton);

            GUIUtil.addSettingsComponentsToGui(GameSettings.controlsSettings, this);
        }
        private void onBackButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
    }
}
