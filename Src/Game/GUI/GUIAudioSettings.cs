using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class GUIAudioSettings : GUI
    {
        GUIButton backButton;
        GUIButton applyButton; 
        GUIValueSlider s;
        public GUIAudioSettings() : base("audioSettings", "Arial_Shadow")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("titleBack", new GUITransparentRecangle(new Vector2(0, 0.5F), new Vector2(0.7F, 0.8F), Color.black.setAlphaF(0.7F), ComponentAlignment.CENTER, 1, false));
            addTextPanel("title", new GUITextPanel(new Vector2(0, 0.15F), ComponentAlignment.CENTER, 1).addLine("Audio Settings").setFontSize(0.4F).setPanelColor(Color.white));
            
            backButton = new GUIButton(new Vector2(-0.1F, 0.85F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Back", screenFont, ComponentAlignment.CENTER, 1);
            backButton.addClickListener(onBackButtonClick);
            backButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            backButton.addHoverExitListener(defaultOnButtonHoverExit);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);
           
            applyButton = new GUIButton(new Vector2(0.1F, 0.85F), new Vector2(0.2F, 0.05F), Color.grey.setAlphaF(0.7F), "Apply", screenFont, ComponentAlignment.CENTER, 1);
            if (!GameSettings.audioSettingsChanged) applyButton.disable();
            applyButton.addClickListener(onBackButtonClick);
            applyButton.addHoverEnterListener(defaultOnButtonHoverEnter);
            applyButton.addHoverExitListener(defaultOnButtonHoverExit);
            applyButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("applyButton", applyButton);

            s = new GUIValueSlider(new Vector2(0, 0.5F), new Vector2(0.2F, 0.05F), "test for volume", screenFont, true, 1).setRange(0.0F, 1.0F).setMaxDisplayVal(100).setMinDisplayVal(0).setSliderPos(GameSettings.masterVolume.normalizedFloatValue);
            s.addSlideMoveListener(h);
            addGuiComponent("s", s);
            s.updateRenderData();
        }

        private void h()
        {
            GameSettings.masterVolume.floatValue = s.getFloatValue();
        }
        private void onBackButtonClick()
        {
            GUIManager.closeCurrentGUI();
        }
    }
}
