namespace RabbetGameEngine
{
    public class GUIPauseMenu : GUI
    {
        GUIButton quitButton;
        GUIButton settingsButton;
        GUIButton resumeButton;
        public GUIPauseMenu() : base("pauseMenu", "arial")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 0.5F, 0.5F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("title", new GUITextPanel(0, 0.15F, guiFont, ComponentAnchor.CENTER, 1).addLine("Game Paused").setFontSize(0.4F).setDefaultLineColor(Color.white));


            resumeButton = new GUIButton(0, 0, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Resume Game", guiFont, ComponentAnchor.CENTER, 1);
            resumeButton.addClickListener(onResumeButtonClick);
            resumeButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("resumeButton", resumeButton);

            settingsButton = new GUIButton(0.0F, -0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Settings", guiFont, ComponentAnchor.CENTER, 1);
            settingsButton.addClickListener(onSettingsButtonClick);
            settingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("settingsButton", settingsButton);

            quitButton = new GUIButton(0.0F, -0.1F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Quit Game", guiFont, ComponentAnchor.CENTER, 1);
            quitButton.addClickListener(onQuitButtonClick);
            quitButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("quitButton", quitButton);
        }

        private void onQuitButtonClick(GUIButton g)
        {
            GameInstance.get.Close();
        }

        private void onResumeButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
        private void onSettingsButtonClick(GUIButton g)
        {
            GUIManager.openGUI(new GUISettingsMenu());
        }
    }
}
