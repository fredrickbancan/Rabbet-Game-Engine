using OpenTK.Mathematics;

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
            addGuiComponent("titleBack", new GUITransparentRecangle(new Vector2(0, 0.5F), new Vector2(0.4F, 0.4F), Color.black.setAlphaF(0.7F), ComponentAlignment.CENTER, 1, false));
            addTextPanel("title", new GUITextPanel(new Vector2(0, 0.35F), ComponentAlignment.CENTER, 1).addLine("Game Paused").setFontSize(0.4F).setPanelColor(Color.white));


            resumeButton = new GUIButton(new Vector2(0.0F, 0.5F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Resume Game", screenFont, ComponentAlignment.CENTER, 1);
            resumeButton.addClickListener(onResumeButtonClick);
            resumeButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            resumeButton.addHoverExitListener(defaultOnButtonHoverExit);
            resumeButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("resumeButton", resumeButton);

            settingsButton = new GUIButton(new Vector2(0.0F, 0.55F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Settings", screenFont, ComponentAlignment.CENTER, 1);
            settingsButton.addClickListener(onSettingsButtonClick);
            settingsButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            settingsButton.addHoverExitListener(defaultOnButtonHoverExit);
            settingsButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("settingsButton", settingsButton);

            quitButton = new GUIButton(new Vector2(0.0F, 0.6F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Quit Game", screenFont, ComponentAlignment.CENTER, 1);
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
