using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    /*A class for abstracting the process of displaying debug information on the screen when active.*/
    public static class DebugScreen
    {
        private static String playerInfoName = "DebugPlayerInfo";
        /*Shows and updates the debug info on the screen, Can be called every tick (Do not call every frame, too expensive)*/
        public static void displayOrClearDebugInfo(GameInstance game)
        {
            if (GameSettings.debugScreen)
            {
                Renderer.textRenderer2D.addNewTextPanel(playerInfoName,
                       new String[]
                       {
                        ("Player Name: " + game.thePlayer.getName()),
                        ("X: " + game.thePlayer.getPosition().x.ToString("0.##")),
                        ("Y: " + game.thePlayer.getPosition().y.ToString("0.##")),
                        ("Z: " + game.thePlayer.getPosition().z.ToString("0.##")),
                        ("Velocity X: " + game.thePlayer.getVelocity().x.ToString("0.##")),
                        ("Velocity Y: " + game.thePlayer.getVelocity().y.ToString("0.##")),
                        ("Velocity Z: " + game.thePlayer.getVelocity().z.ToString("0.##")),
                        ("Head Pitch: " + game.thePlayer.getHeadPitch().ToString("0.##")),
                        ("Yaw: " + game.thePlayer.getYaw().ToString("0.##")),
                        ("Profiler ms averages: "),
                        ("Game loop: " + Profiler.getAveragesForProfile(Profiler.gameLoopName).ToString("0.##") + " ms."),
                        ("Render: " + Profiler.getAveragesForProfile(Profiler.renderingName).ToString("0.##") + " ms."),
                        ("Text Renderer 2D: " + Profiler.getAveragesForProfile(Profiler.textRender2DBuildingName).ToString("0.##") + " ms."),
                        ("press F3 to hide debug screen."),
                       }, new Vector2F(0.0F, 0.05F), ColourF.grey); 
            }
            else
            {
                Renderer.textRenderer2D.removeTextPanel(playerInfoName);
            }
        }
    }
}
