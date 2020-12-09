using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class GUIPauseMenu : GUI
    {
        GUIButton quitButton;
        GUIButton resumeButton;
        public GUIPauseMenu() : base("pauseMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(CustomColor.black, 0.5F));

            resumeButton = new GUIButton(new Vector2(0.0F, 0.4F), new Vector2(0.2F, 0.05F), CustomColor.grey.setAlphaF(0.7F), "white", ButtonAlignment.CENTER);
            resumeButton.addClickListener(onResumeButtonClick);
            resumeButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            resumeButton.addHoverExitListener(defaultOnButtonHoverExit);
            resumeButton.setHoverColor(CustomColor.black.setAlphaF(0.5F));
            addGuiComponent("resumeButton", resumeButton);

            quitButton = new GUIButton(new Vector2(0.0F, 0.45F), new Vector2(0.2F, 0.05F), CustomColor.grey.setAlphaF(0.7F), "white", ButtonAlignment.CENTER);
            quitButton.addClickListener(onQuitButtonClick);
            quitButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            quitButton.addHoverExitListener(defaultOnButtonHoverExit);
            quitButton.setHoverColor(CustomColor.black.setAlphaF(0.5F));
            addGuiComponent("quitButton", quitButton);

            //TODO: Make buttons text part of the button class.

            GUITextPanel t = new GUITextPanel(new TextFormat(0.5F, 0.45F - 0.015F).setAlign(Text.TextAlign.CENTER).setLine("Quit Game"), 1);
            addTextPanel("quitButtonText", t);
            GUITextPanel t2 = new GUITextPanel(new TextFormat(0.5F, 0.4F - 0.015F).setAlign(Text.TextAlign.CENTER).setLine("Resume Game"), 1);
            addTextPanel("resumeButtonText", t2);
        }

        private void onQuitButtonClick()
        {
            defaultOnButtonClick();
            GameInstance.get.Close();
        }

        private void onResumeButtonClick()
        {
            defaultOnButtonClick();
            GameInstance.get.unPauseGame();
            GUIManager.closeCurrentGUI();
        }
    }
}
