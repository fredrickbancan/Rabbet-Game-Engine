using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;

namespace RabbetGameEngine
{
    public class GUIDebugInfo : GUI
    {
        GUITextPanel profileAveragesText;
        GUITextPanel infoText;
        GUIDebugFrameTimeChart timeChart;
        public GUIDebugInfo() : base("debugInfo", "consolas")
        {
            timeChart = new GUIDebugFrameTimeChart(0, 0, ComponentAnchor.BOTTOM_RIGHT);
            addGuiComponent("frameTimeChart", timeChart);
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
                profileAveragesText.clear();
                Profiler.getFrameProfilingData(profileAveragesText.lines);
                profileAveragesText.addLine("");
                Profiler.getTickProfilingData(profileAveragesText.lines);
                profileAveragesText.build();
                Vector3 pPos = GameInstance.get.thePlayer.getEyePosition();
                Vector3 pVel = GameInstance.get.thePlayer.getVelocity();
                infoText.clear();
                infoText.addLine("X: " + pPos.X.ToString("0.0 m"));
                infoText.addLine("Y: " + pPos.Y.ToString("0.0 m"));
                infoText.addLine("Z: " + pPos.Z.ToString("0.0 m"));
                infoText.addLine("X vel: " + pVel.X.ToString("0.0 m/t"));
                infoText.addLine("Y vel: " + pVel.Y.ToString("0.0 m/t"));
                infoText.addLine("Z vel: " + pVel.Z.ToString("0.0 m/t"));
                infoText.addLine("Entities: " + GameInstance.get.currentPlanet.getEntityCount());
                infoText.addLine("Projectiles: " + GameInstance.get.currentPlanet.getProjectileCount());
                infoText.addLine("Draw calls: " + Renderer.totalDraws);
                infoText.addLine("Resolution: " + Renderer.viewPortSize.X + " X " + Renderer.viewPortSize.Y);
                infoText.addLine("Memory Usage: " + Application.ramUsageInBytes / 1000000L + " MB");
                infoText.build();
            }
        }
    }
}
