using OpenTK.Mathematics;


namespace RabbetGameEngine
{
    public class GUIDebugInfo : GUI
    {
        GUITextPanel profileAveragesText;
        GUITextPanel infoText;
        GUIDebugFrameTimeChart timeChart;
        public GUIDebugInfo() : base("debugInfo", "consolas")
        {
            timeChart = new GUIDebugFrameTimeChart(0, 0,  ComponentAnchor.BOTTOM_RIGHT, 1);
            addGuiComponent("frameTimeChart", timeChart);
            profileAveragesText = new GUITextPanel(0, -0.025F, guiFont, ComponentAnchor.TOP_LEFT).setDefaultLineColor(Color.white);
            profileAveragesText.setFontSize(0.15F);
            addGuiComponent("profileAverages", profileAveragesText);

            infoText = new GUITextPanel(0, -0.05F, guiFont, ComponentAnchor.TOP_RIGHT).setDefaultLineColor(Color.white);
            infoText.setFontSize(0.15F);
            addGuiComponent("info", infoText);
        }

        public override void onUpdate(bool isFrameUpdate)
        {
            base.onUpdate(isFrameUpdate);
            if (!isFrameUpdate && GameInstance.get.currentWorld != null)
            {
                profileAveragesText.clear();
                Profiler.getFrameProfilingData(profileAveragesText.lines, profileAveragesText.lineColors);
                profileAveragesText.addLine("");
                Profiler.getTickProfilingData(profileAveragesText.lines, profileAveragesText.lineColors);
                profileAveragesText.build();
                Vector3 pPos = Renderer.camPos;
                infoText.clear();
                infoText.addLine("X: " + pPos.X.ToString("0.0 m"));
                infoText.addLine("Y: " + pPos.Y.ToString("0.0 m"));
                infoText.addLine("Z: " + pPos.Z.ToString("0.0 m"));

               //infoText.addLine("Chunk Render Updates: " + TerrainRenderer.chunkUpdates);

                infoText.addLine("Batches: " + BatchManager.batchCount);
                infoText.addLine("GUI Batches: " + BatchManager.guiBatchCount);
                infoText.addLine("Draw calls: " + Renderer.totalDraws);
                infoText.addLine("Chunk Draw calls: " + TerrainRenderer.NUM_DRAWCALLS);
                infoText.addLine("FBO Draw calls: " + Renderer.totalFBODraws);
                infoText.addLine("Resolution: " + Renderer.viewPortSize.X + " X " + Renderer.viewPortSize.Y);
                infoText.addLine("Sounds: " + SoundManager.getPlayingSoundsCount());
                infoText.addLine("Memory Usage: " + Application.ramUsageInBytes / 1000000L + " MB");
                infoText.build();
            }
        }
    }
}
