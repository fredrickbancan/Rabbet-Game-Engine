using RabbetGameEngine.GUI;
using RabbetGameEngine.GUI.Text;

namespace RabbetGameEngine.Debugging
{
    /*A class for abstracting the process of displaying debug information on the screen when active.*/
    public static class DebugInfo
    {
        public static readonly string debugInfoTextPanelName = "debugInfo";
        /*Initialize the text panel for the debug info, can only be done if the mainGUI panel is created first*/
        public static void init()
        {
            GUIHandler.addTextPanelToGUI(MainGUI.mainGUIName, debugInfoTextPanelName, new GUITextPanel(new TextFormat(0.0F, 0.05F)
                .setLines(new string[]
                        {
                        ("press F3 to hide debug screen.")
                        }
                       ).setPanelColor(CustomColor.lightGrey)));
        }

        /*Shows and updates the debug info on the screen, Can be called every tick (Do not call every frame, too expensive)*/
        public static void displayOrClearDebugInfo()
        {
            if (GameSettings.debugScreen && GameInstance.get.thePlayer != null)
            {
                float entColAverage = Profiler.getAverageForProfile("EntCollisions");
                float colAverage = Profiler.getAverageForProfile("Collisions");
                float gameLoopAverage = Profiler.getAverageForProfile("Loop");
                GUIHandler.unHideTextPanelInGUI(MainGUI.mainGUIName, debugInfoTextPanelName);
                GUIHandler.getTextPanelFormatFromGUI(MainGUI.mainGUIName, debugInfoTextPanelName).setLines(
                       new string[]
                       {
                        ("Player Name: " + GameInstance.get.thePlayer.getName()),
                        ("X: " + GameInstance.get.thePlayer.getPosition().X.ToString("0.00")),
                        ("Y: " + GameInstance.get.thePlayer.getPosition().Y.ToString("0.00")),
                        ("Z: " + GameInstance.get.thePlayer.getPosition().Z.ToString("0.00")),
                        ("Velocity X: " + GameInstance.get.thePlayer.getVelocity().X.ToString("0.00")),
                        ("Velocity Y: " + GameInstance.get.thePlayer.getVelocity().Y.ToString("0.00 ")),
                        ("Velocity Z: " + GameInstance.get.thePlayer.getVelocity().Z.ToString("0.00")),
                       ("Head Pitch: " + GameInstance.get.thePlayer.getHeadPitch().ToString("0.00")),
                       ("Yaw: " + GameInstance.get.thePlayer.getYaw().ToString("0.00")),
                       ("Profiler Averages (MS)" ),
                       ("{" ),
                       ("   GameLoop: " + gameLoopAverage.ToString("0.00") + "ms." ),
                       ("   {" ),
                       ("       Entity Collisions: " + entColAverage.ToString("0.00") + "ms." ),
                       ("       World Collisions: " + colAverage.ToString("0.00") + "ms." ),
                       ("   }Residual: " + (gameLoopAverage - (entColAverage + colAverage)).ToString("0.00") + "ms." ),
                       ("}" ),
                        ("Entities: " + GameInstance.get.currentPlanet.getEntityCount()),
                        ("VFX's: " + GameInstance.get.currentPlanet.getVFXCount()),
                        ("Draw Calls: " + Renderer.getAndResetTotalDrawCount()),
                       }); 
            }
            else
            {
                GUIHandler.hideTextPanelInGUI(MainGUI.mainGUIName, debugInfoTextPanelName);
            }
        }
    }
}
