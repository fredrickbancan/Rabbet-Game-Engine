using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    //TODO: Implement settings
    public class GUISettingsMenu : GUI
    {
        GUIButton backButton;
        public GUISettingsMenu() : base("settingsMenu", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRecangle(new Vector2(0, 0.5F), new Vector2(1.6F, 0.8F), Color.black.setAlphaF(0.7F), ComponentAlignment.CENTER, 1));
            addTextPanel("title", new GUITextPanel(new Vector2(0, 0.15F), ComponentAlignment.CENTER, 1).addLine("Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(new Vector2(0.0F, 0.85F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Back", screenFont, ComponentAlignment.CENTER, 1);
            backButton.addClickListener(onBackButtonClick);
            backButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            backButton.addHoverExitListener(defaultOnButtonHoverExit);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);
        }

        private void onBackButtonClick()
        {
            defaultOnButtonClick();
            GUIManager.closeCurrentGUI();
        }
    }
}
