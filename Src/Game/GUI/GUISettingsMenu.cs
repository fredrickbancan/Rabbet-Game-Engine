using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    //TODO: Implement settings
    public class GUISettingsMenu : GUI
    {
        GUIButton backButton;
        GUIButton audioSettingsButton;
        GUIButton graphicalSettingsButton;
        GUIButton controlsSettingsButton;
        public GUISettingsMenu() : base("settingsMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRecangle(new Vector2(0, 0.5F), new Vector2(0.7F, 0.8F), Color.black.setAlphaF(0.7F), ComponentAlignment.CENTER, 1, false));
            addTextPanel("title", new GUITextPanel(new Vector2(0, 0.15F), ComponentAlignment.CENTER, 1).addLine("Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(new Vector2(0.0F, 0.85F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Back", screenFont, ComponentAlignment.CENTER, 1);
            backButton.addClickListener(onBackButtonClick);
            backButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            backButton.addHoverExitListener(defaultOnButtonHoverExit);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);

            audioSettingsButton = new GUIButton(new Vector2(-0.225F, 0.5F), new Vector2(0.2F, 0.1F), Color.grey.setAlphaF(0.7F), "Audio Settings", screenFont, ComponentAlignment.CENTER, 1);
            audioSettingsButton.addClickListener(onAudioSettingsButtonClick);
            audioSettingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            audioSettingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            audioSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("audioSettingsButton", audioSettingsButton);

            graphicalSettingsButton = new GUIButton(new Vector2(0.0F, 0.5F), new Vector2(0.2F, 0.1F), Color.grey.setAlphaF(0.7F), "Graphics Settings", screenFont, ComponentAlignment.CENTER, 1);
            graphicalSettingsButton.addClickListener(onGraphicsSettingsButtonClick);
            graphicalSettingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            graphicalSettingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            graphicalSettingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("graphicalSettingsButton", graphicalSettingsButton);

            controlsSettingsButton = new GUIButton(new Vector2(0.225F, 0.5F), new Vector2(0.2F, 0.1F), Color.grey.setAlphaF(0.7F), "Controls Settings", screenFont, ComponentAlignment.CENTER, 1);
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
