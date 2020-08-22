using Coictus.FredsMath;
using Coictus.GUI;
using Coictus.GUI.Text;
using System;

namespace Coictus
{
    /*A class for abstracting the process of displaying debug information on the screen when active.*/
    public static class DebugScreen
    {
        public static readonly String debugInfoPanelName = "debugInfo";

        /*Initialize the text panel for the debug info, can only be done if the mainGUI panel is created first*/
        public static void init()
        {
            GUIHandler.addTextPanelToGUI(GameInstance.mainGUIName, debugInfoPanelName, new GUITextPanel(new TextFormat(0.0F, 0.05F).setPanelColor(ColourF.grey)
                .setLines(new String[]
                        {
                        ("press F3 to hide debug screen.")
                        }
                       ).setPanelColor(ColourF.grey)));
        }

        /*Shows and updates the debug info on the screen, Can be called every tick (Do not call every frame, too expensive)*/
        public static void displayOrClearDebugInfo()
        {
            if (GameSettings.debugScreen && GameInstance.get.thePlayer != null)
            {
                GUIHandler.unHideTextPanelInGUI(GameInstance.mainGUIName, debugInfoPanelName);
                GUIHandler.getTextPanelFormatFromGUI(GameInstance.mainGUIName, debugInfoPanelName).setLines(
                       new String[]
                       {
                        ("Player Name: " + GameInstance.get.thePlayer.getName()),
                        ("X: " + GameInstance.get.thePlayer.getPosition().x.ToString("0.##")),
                        ("Y: " + GameInstance.get.thePlayer.getPosition().y.ToString("0.##")),
                        ("Z: " + GameInstance.get.thePlayer.getPosition().z.ToString("0.##")),
                        ("Velocity X: " + GameInstance.get.thePlayer.getVelocity().x.ToString("0.##")),
                        ("Velocity Y: " + GameInstance.get.thePlayer.getVelocity().y.ToString("0.##")),
                        ("Velocity Z: " + GameInstance.get.thePlayer.getVelocity().z.ToString("0.##")),
                        ("Head Pitch: " + GameInstance.get.thePlayer.getHeadPitch().ToString("0.##")),
                        ("Yaw: " + GameInstance.get.thePlayer.getYaw().ToString("0.##")),
                        ("Profiler ms averages: "),
                        ("Render: " + Profiler.getAveragesForProfile(Profiler.renderingName).ToString("0.##") + " ms."),
                        ("Game loop: " + Profiler.getAveragesForProfile(Profiler.gameLoopName).ToString("0.##") + " ms."),
                        ("Collisions: " + Profiler.getAveragesForProfile(Profiler.collisionsName).ToString("0.##") + " ms."),
                        ("press F3 to hide debug screen."),
                       }); 
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(GameInstance.mainGUIName, debugInfoPanelName);
            }
        }
    }
}
