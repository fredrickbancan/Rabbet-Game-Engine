namespace RabbetGameEngine
{
    public class GUIPauseMenu : GUI
    {
        GUIButton quitButton;
        GUIButton settingsButton;
        GUIButton resumeButton;
        public GUIPauseMenu() : base("pauseMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 0.5F, 0.5F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("title", new GUITextPanel(0, 0.15F, guiFont, ComponentAnchor.CENTER, 1).addLine("Game Paused").setFontSize(0.4F).setPanelColor(Color.white));


            resumeButton = new GUIButton(0, 0, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Resume Game", guiFont, ComponentAnchor.CENTER, 1);
            resumeButton.addClickListener(onResumeButtonClick);
            resumeButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            resumeButton.addHoverExitListener(defaultOnButtonHoverExit);
            resumeButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("resumeButton", resumeButton);

            settingsButton = new GUIButton(0.0F, -0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Settings", guiFont, ComponentAnchor.CENTER, 1);
            settingsButton.addClickListener(onSettingsButtonClick);
            settingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            settingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            settingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("settingsButton", settingsButton);

            quitButton = new GUIButton(0.0F, -0.1F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Quit Game", guiFont, ComponentAnchor.CENTER, 1);
            quitButton.addClickListener(onQuitButtonClick);
            quitButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            quitButton.addHoverExitListener(defaultOnButtonHoverExit);
            quitButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("quitButton", quitButton);
        }

        private void onQuitButtonClick()
        {
            defaultOnButtonClick();
            GameInstance.get.Close();
        }

        private void onResumeButtonClick()
        {
            GUIManager.closeCurrentGUI();
        }
        private void onSettingsButtonClick()
        {
            defaultOnButtonClick();
            GUIManager.openGUI(new GUISettingsMenu());
        }
    }
}
