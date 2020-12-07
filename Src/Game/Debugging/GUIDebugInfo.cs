using RabbetGameEngine.Debugging;
using RabbetGameEngine.Sound;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    public class GUIDebugInfo : GUI
    {
        public GUIDebugInfo() : base("debugInfo", "Arial_Shadow")
        {
            addTextPanel("info", new GUITextPanel(new TextFormat(0.0F, 0.05F).setPanelColor(CustomColor.lightGrey)));
        }
        
        public override void onUpdate()
        {
            base.onUpdate();
            if (GameInstance.get.thePlayer != null && GameInstance.get.currentPlanet != null)
            {
                float entColAverage = Profiler.getAverageForProfile("EntCollisions");
                float colAverage = Profiler.getAverageForProfile("Collisions");
                float guiUpdateAverage = Profiler.getAverageForProfile("guiUpdate");
                float renderUpdateAverage = Profiler.getAverageForProfile("renderUpdate");
                float soundsAverage = Profiler.getAverageForProfile("sounds");
                float gameLoopAverage = Profiler.getAverageForProfile("Loop");
                float aux = Profiler.getAverageForProfile("aux");
                GUITextPanel t = getTextPanel("info");
                t.clear();
                t.addLine("X: " + GameInstance.get.thePlayer.getPosition().X.ToString("0.00"));
                t.addLine("Y: " + GameInstance.get.thePlayer.getPosition().Y.ToString("0.00"));
                t.addLine("Z: " + GameInstance.get.thePlayer.getPosition().Z.ToString("0.00"));
                t.addLine("Velocity X: " + GameInstance.get.thePlayer.getVelocity().X.ToString("0.00"));
                t.addLine("Velocity Y: " + GameInstance.get.thePlayer.getVelocity().Y.ToString("0.00"));
                t.addLine("Velocity Z: " + GameInstance.get.thePlayer.getVelocity().Z.ToString("0.00"));
                t.addLine("Profiler Averages (MS)");
                t.addLine("   GameLoop: " + gameLoopAverage.ToString("0.00 ms"));
                t.addLine("   {");
                t.addLine("       Entity Collisions: " + entColAverage.ToString("0.00 ms"));
                t.addLine("       World Collisions: " + colAverage.ToString("0.00 ms"));
                t.addLine("       Sounds: " + soundsAverage.ToString("0.00 ms"));
                t.addLine("   }Residual: " + (gameLoopAverage - (entColAverage + colAverage + soundsAverage)).ToString("0.00 ms"));
                t.addLine("GUI Update: " + guiUpdateAverage.ToString("0.00 ms"));
                t.addLine("Render Update: " + renderUpdateAverage.ToString("0.00 ms"));
                t.addLine("Aux: " + aux.ToString("0.00 ms"));
                t.addLine("Entities: " + GameInstance.get.currentPlanet.getEntityCount());
                t.addLine("Projectiles: " + GameInstance.get.currentPlanet.getProjectileCount());
                t.addLine("VFX: " + GameInstance.get.currentPlanet.getVFXCount());
                t.addLine("Sounds: " + SoundManager.getPlayingSoundsCount());
                t.addLine("Draw Calls: " + Renderer.totalDraws);
                t.addLine("Batches: " + BatchManager.batchCount);
                t.addLine("Used memory: " + (Application.ramUsageInBytes / 1000000L).ToString() + "MB");
                t.pushLines();
                t.build();

            }
        }
    }
}
