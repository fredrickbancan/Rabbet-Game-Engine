using RabbetGameEngine.Debugging;
using RabbetGameEngine.Sound;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    //TODO: Create some sort of automatic debug profile gui system which shows all nested profiles under each section.
    public class GUIDebugInfo : GUI
    {
        GUITextPanel t;
        public GUIDebugInfo() : base("debugInfo", "Arial_Shadow")
        {
            t = new GUITextPanel(0, -0.025F, guiFont, ComponentAnchor.TOP_LEFT).setPanelColor(Color.lightGrey);
            addGuiComponent("info", t);
        }
        
        public override void onUpdate()
        {
            base.onUpdate();
            if (GameInstance.get.thePlayer != null && GameInstance.get.currentPlanet != null)
            {
                float entColAverage = Profiler.getAverageForProfile("EntCollisions");
                float colAverage = Profiler.getAverageForProfile("entWorldCol");
                float projColAverage = Profiler.getAverageForProfile("projCol");
                float vfxColAverage = Profiler.getAverageForProfile("vfxCol");
                float entTickAverage = Profiler.getAverageForProfile("entTick") - colAverage;
                float projTickAverage = Profiler.getAverageForProfile("projTick") - projColAverage;
                float projColEntAverage = Profiler.getAverageForProfile("projColEnt");
                float vfxTickAverage = Profiler.getAverageForProfile("vfxTick") - vfxColAverage;
                float guiUpdateAverage = Profiler.getAverageForProfile("guiUpdate");
                float renderUpdateAverage = Profiler.getAverageForProfile("renderUpdate");
                float soundsAverage = Profiler.getAverageForProfile("sounds");
                float gameLoopAverage = Profiler.getAverageForProfile("Loop");
                float aux = Profiler.getAverageForProfile("aux");
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
                t.addLine("       Entity Tick: " + entTickAverage.ToString("0.00 ms"));
                t.addLine("       Inter-Entity Collisions: " + entColAverage.ToString("0.00 ms"));
                t.addLine("       Projectile Tick: " + projTickAverage.ToString("0.00 ms"));
                t.addLine("       Projectile Entity Collisions: " + projColEntAverage.ToString("0.00 ms"));
                t.addLine("       VFX Tick: " + vfxTickAverage.ToString("0.00 ms"));
                t.addLine("       Entity World Collisions: " + colAverage.ToString("0.00 ms"));
                t.addLine("   }Residual: " + (gameLoopAverage - (vfxTickAverage + projColEntAverage + entTickAverage + projTickAverage + entColAverage + colAverage)).ToString("0.00 ms"));
                t.addLine("Sound Update: " + soundsAverage.ToString("0.00 ms"));
                t.addLine("GUI Update: " + guiUpdateAverage.ToString("0.00 ms"));
                t.addLine("Render Update: " + renderUpdateAverage.ToString("0.00 ms"));
                t.addLine("Aux: " + aux.ToString("0.00 ms"));
                t.addLine("Entities: " + GameInstance.get.currentPlanet.getEntityCount());
                t.addLine("Projectiles: " + GameInstance.get.currentPlanet.getProjectileCount());
                t.addLine("VFX: " + GameInstance.get.currentPlanet.getVFXCount());
                t.addLine("Sounds: " + SoundManager.getPlayingSoundsCount());
                t.addLine("Draw Calls: " + Renderer.totalDraws);
                t.addLine("Batches: " + BatchManager.batchCount);
                t.addLine("Resolution: " + Renderer.viewPortSize.X + "X" + Renderer.viewPortSize.Y);
                t.addLine("Used memory: " + (Application.ramUsageInBytes / 1000000L).ToString() + "MB");
                t.build();

            }
        }
    }
}
