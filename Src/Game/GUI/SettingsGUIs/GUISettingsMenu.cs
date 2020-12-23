namespace RabbetGameEngine
{
    public class GUISettingsMenu : GUI
    {
        GUIButton backButton;
        GUIButton audioSettingsButton;
        GUIButton graphicalSettingsButton;
        GUIButton controlsSettingsButton;
        public GUISettingsMenu() : base("settingsMenu", "arial")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 1.5F, 1F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("title", new GUITextPanel(0, 0.15F, guiFont, ComponentAnchor.CENTER, 1).addLine("Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(0, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER_BOTTOM, 1).clearClickListeners();
            backButton.addClickListener(onBackButtonClick);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);

            audioSettingsButton = new GUIButton(-0.2F, 0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Audio Settings", guiFont, ComponentAnchor.CENTER, 1);
            audioSettingsButton.addClickListener(onAudioSettingsButtonClick);
            audioSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("audioSettingsButton", audioSettingsButton);

            graphicalSettingsButton = new GUIButton(0.0F, 0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Video Settings", guiFont, ComponentAnchor.CENTER, 1);
            graphicalSettingsButton.addClickListener(onGraphicsSettingsButtonClick);
            graphicalSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("graphicalSettingsButton", graphicalSettingsButton);

            controlsSettingsButton = new GUIButton(0.2F, 0.0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Controls Settings", guiFont, ComponentAnchor.CENTER, 1);
            controlsSettingsButton.addClickListener(onControlsButtonClick);
            controlsSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("controlsSettingsButton", controlsSettingsButton);
        }

        private void onBackButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
        private void onAudioSettingsButtonClick(GUIButton g)
        {
            GUIManager.openGUI(new GUIAudioSettings());
        }

        private void onGraphicsSettingsButtonClick(GUIButton g)
        {
            GUIManager.openGUI(new GUIVideoSettings());
        }

        private void onControlsButtonClick(GUIButton g)
        {

        }
    }
}
