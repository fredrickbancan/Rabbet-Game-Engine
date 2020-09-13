using Coictus.GUI;
using Coictus.GUI.Text;
using System;

namespace Coictus.Debugging
{
    /*A class for abstracting the process of displaying debug information on the screen when active.*/
    public static class DebugInfo
    {
        public static readonly string debugInfoPanelName = "debugInfo";

        /*Initialize the text panel for the debug info, can only be done if the mainGUI panel is created first*/
        public static void init()
        {
            GUIHandler.addTextPanelToGUI(MainGUI.mainGUIName, debugInfoPanelName, new GUITextPanel(new TextFormat(0.0F, 0.05F)
                .setLines(new string[]
                        {
                        ("press F3 to hide debug screen.")
                        }
                       ).setPanelColor(CustomColor.grey)));
        }

        /*Shows and updates the debug info on the screen, Can be called every tick (Do not call every frame, too expensive)*/
        public static void displayOrClearDebugInfo()
        {
            if (GameSettings.debugScreen && GameInstance.get.thePlayer != null)
            {
                GUIHandler.unHideTextPanelInGUI(MainGUI.mainGUIName, debugInfoPanelName);
                GUIHandler.getTextPanelFormatFromGUI(MainGUI.mainGUIName, debugInfoPanelName).setLines(
                       new string[]
                       {
                        ("Player Name: " + GameInstance.get.thePlayer.getName()),
                        ("X: " + GameInstance.get.thePlayer.getPosition().X.ToString("0.##")),
                        ("Y: " + GameInstance.get.thePlayer.getPosition().Y.ToString("0.##")),
                        ("Z: " + GameInstance.get.thePlayer.getPosition().Z.ToString("0.##")),
                        ("Velocity X: " + GameInstance.get.thePlayer.getVelocity().X.ToString("0.##")),
                        ("Velocity Y: " + GameInstance.get.thePlayer.getVelocity().Y.ToString("0.##")),
                        ("Velocity Z: " + GameInstance.get.thePlayer.getVelocity().Z.ToString("0.##")),
                        ("Head Pitch: " + GameInstance.get.thePlayer.getHeadPitch().ToString("0.##")),
                        ("Yaw: " + GameInstance.get.thePlayer.getYaw().ToString("0.##")),
                        ("Profiler ms averages: "),
                        ("Render: " + Profiler.getAveragesForProfile(Profiler.renderingName).ToString("0.##") + " ms."),
                        ("Game loop: " + Profiler.getAveragesForProfile(Profiler.gameLoopName).ToString("0.##") + " ms."),
                        ("Collisions: " + Profiler.getAveragesForProfile(Profiler.collisionsName).ToString("0.##") + " ms."),
                        ("Entities: " + GameInstance.get.currentPlanet.getEntityCount())
                       }); 
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(MainGUI.mainGUIName, debugInfoPanelName);
            }
        }
    }
}
