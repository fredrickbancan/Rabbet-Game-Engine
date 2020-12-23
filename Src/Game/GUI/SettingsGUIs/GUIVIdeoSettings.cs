namespace RabbetGameEngine
{
    public class GUIVideoSettings : GUI
    {
        GUIButton backButton;
        GUIButton applyButton;
        bool applyButtonNeedsUpdate = false;
        public GUIVideoSettings() : base("videoSettings", "arial")
        {
            addGuiComponent("background", new GUITransparentOverlay(Color.black, 0.7F, 1));
            addGuiComponent("titleBack", new GUITransparentRectangle(0, 0, 1.5F, 1.0F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER, 1));
            addGuiComponent("title", new GUITextPanel(0, 0, guiFont, ComponentAnchor.CENTER_TOP, 2).addLine("Video Settings").setFontSize(0.4F).setPanelColor(Color.white));

            backButton = new GUIButton(-0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER_BOTTOM, 2).clearClickListeners();
            backButton.addClickListener(onBackButtonClick);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("backButton", backButton);

            applyButton = new GUIButton(0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Apply", guiFont, ComponentAnchor.CENTER_BOTTOM, 2);
            if (!GameSettings.audioSettingsChanged) applyButton.disable();
            applyButton.addClickListener(onApplyButtonClick);
            applyButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("applyButton", applyButton);

            GUIUtil.addVideoSettingsChangersToGUI(this);
        }

        public override void onComponentValueChanged()
        {
            base.onComponentValueChanged();
            GameSettings.videoSettingsChanged = true;
        }

        public override void onUpdate()
        {
            if (!GameSettings.videoSettingsChanged) applyButton.disable();
            else applyButton.enable();
            applyButtonNeedsUpdate = applyButtonNeedsUpdate != GameSettings.videoSettingsChanged;

            if(applyButtonNeedsUpdate)
            {
                applyButton.updateRenderData();
            }
            applyButtonNeedsUpdate = GameSettings.videoSettingsChanged;
            base.onUpdate();
        }

        private void onApplyButtonClick(GUIButton g)
        {
            GameSettings.applyVideoSettings();
        }

        private void onBackButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
    }
}
