namespace RabbetGameEngine
{
    public class GUIBindingsSettings : GUI
    {
        GUIButton backButton;
        GUIButton applyButton;
        public GUIBindingsSettings() : base("bindSet", "arial")
        {
            addGuiComponent("bgr", new GUITransparentOverlay(Color.black, 0.7F));
            addGuiComponent("ttlb", new GUITransparentRectangle(0, 0, 1.5F, 1.0F, Color.black.setAlphaF(0.7F), ComponentAnchor.CENTER));
            addGuiComponent("ttl", new GUITextPanel(0, 0, guiFont, ComponentAnchor.CENTER_TOP, 1).addLine("Bindings Settings").setFontSize(0.4F).setDefaultLineColor(Color.white));

            backButton = new GUIButton(-0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Back", guiFont, ComponentAnchor.CENTER_BOTTOM, 1).clearClickListeners();
            backButton.addClickListener(onBackButtonClick);
            backButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("bBut", backButton);

            applyButton = new GUIButton(0.1F, 0.05F, 0.2F, 0.05F, Color.grey.setAlphaF(0.7F), "Apply", guiFont, ComponentAnchor.CENTER_BOTTOM, 1);
            if (!GameSettings.audioSettingsChanged) applyButton.disable();
            applyButton.setHoverColor(Color.black.setAlphaF(0.5F));
            addGuiComponent("aBut", applyButton);

            GUIUtil.addBindingComponentsToGui(GameSettings.bindings, this);
        } 
        private void onBackButtonClick(GUIButton g)
        {
            GUIManager.closeCurrentGUI();
        }
    }
}
