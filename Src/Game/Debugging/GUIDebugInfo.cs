using RabbetGameEngine.Debugging;

namespace RabbetGameEngine
{
    //TODO: Implement profile averages only for tick times instead of frame times
    //TODO: Add info about player pos, rot and vel on right size above mem usage
    public class GUIDebugInfo : GUI
    {
        GUITextPanel profileAveragesText;
        GUITextPanel infoText;
        public GUIDebugInfo() : base("debugInfo", "Consolas_Shadow")
        {
            profileAveragesText = new GUITextPanel(0, -0.025F, guiFont, ComponentAnchor.TOP_LEFT).setPanelColor(Color.white);
            profileAveragesText.setFontSize(0.15F);
            addGuiComponent("profileAverages", profileAveragesText);

            infoText = new GUITextPanel(0, -0.05F, guiFont, ComponentAnchor.TOP_RIGHT).setPanelColor(Color.white);
            infoText.setFontSize(0.15F);
            addGuiComponent("info", infoText);
        }
        
        public override void onUpdate()
        {
            base.onUpdate();
            if (GameInstance.get.thePlayer != null && GameInstance.get.currentPlanet != null)
            {
                Profiler.getFrameProfilingData(profileAveragesText.lines);
                profileAveragesText.build();

                infoText.clear();
                infoText.addLine("Memory Usage: " + Application.ramUsageInBytes / 1000000L + " MB");
                infoText.build();
            }
        }
    }
}
