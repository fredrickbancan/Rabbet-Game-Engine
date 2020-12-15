namespace RabbetGameEngine
{
    public class GUISettingsMenu : GUI
    {
        GUIButton backButton;
        GUIButton audioSettingsButton;
        GUIButton graphicalSettingsButton;
        GUIButton controlsSettingsButton;
        public GUISettingsMenu() : base("settingsMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 1.5F, 1F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("title", new GUITextPanel(0, 0.15F, guiFont, ComponentAnchor.CENTER, 1).addLine("Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(0, -0.35F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER, 1);
            backButton.addClickListener(onBackButtonClick);
            backButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            backButton.addHoverExitListener(defaultOnButtonHoverExit);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);

            audioSettingsButton = new GUIButton(-0.2F, 0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Audio Settings", guiFont, ComponentAnchor.CENTER, 1);
            audioSettingsButton.addClickListener(onAudioSettingsButtonClick);
            audioSettingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            audioSettingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            audioSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("audioSettingsButton", audioSettingsButton);

            graphicalSettingsButton = new GUIButton(0.0F, 0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Graphics Settings", guiFont, ComponentAnchor.CENTER, 1);
            graphicalSettingsButton.addClickListener(onGraphicsSettingsButtonClick);
            graphicalSettingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            graphicalSettingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            graphicalSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("graphicalSettingsButton", graphicalSettingsButton);

            controlsSettingsButton = new GUIButton(0.2F, 0.0F, 0.2F, 0.1F, Color.grey.setAlphaF(0.7F), "Controls Settings", guiFont, ComponentAnchor.CENTER, 1);
            controlsSettingsButton.addClickListener(onControlsButtonClick);
            controlsSettingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            controlsSettingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            controlsSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("controlsSettingsButton", controlsSettingsButton);
        }

        private void onBackButtonClick()
        {
            GUIManager.closeCurrentGUI();
        }
        private void onAudioSettingsButtonClick()
        {
            defaultOnButtonClick();
            GUIManager.openGUI(new GUIAudioSettings());
        }

        private void onGraphicsSettingsButtonClick()
        {
            defaultOnButtonClick();
        }

        private void onControlsButtonClick()
        {
            defaultOnButtonClick();
        }
    }
}
