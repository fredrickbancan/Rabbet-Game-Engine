using RabbetGameEngine.GUI.Text;
using RabbetGameEngine.SubRendering.GUI;
using System.Collections.Generic;

namespace RabbetGameEngine.GUI
{
    /*This file will handle all gui related logic and requests*/
    public static class GUIManager
    {
        private static Dictionary<string, GUIScreen> allGUIs = new Dictionary<string, GUIScreen>();
        private static GUIScreen currentDisplayedScreen;

        public static string getCurrentGUIScreenName()
        {
            if (currentDisplayedScreen != null)
            {
                return currentDisplayedScreen.screenName;
            }
            else return "none";
        }

        public static void addGUIComponentToCurrentGUI(string componentName, GUIScreenComponent component)
        {
            if (currentDisplayedScreen != null)
            {
                currentDisplayedScreen.addGuiComponent(componentName, component);
            }
        }

        public static void addGUIComponentToGUI(string GUIName, string componentName, GUIScreenComponent component)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                currentDisplayedScreen.addGuiComponent(componentName, component);
            }
        }

        public static void addTextPanelToCurrentGUI(string panelName, GUITextPanel panel)
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.addTextPanel(panelName, panel);
            }
        }
        public static void addTextPanelToGUI(string GUIName, string panelName, GUITextPanel panel)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                currentDisplayedScreen.addTextPanel(panelName, panel);
            }
        }

        /*returns the format from the text panel with the name provided from the gui name provided, Use this to change the formats of text panels.*/
        public static TextFormat getTextPanelFormatFromGUI(string GUIName, string panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if(screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    return panel.format;
                }
                else
                {
                    Application.error("GUIManager.getTextPanelToFormatFromGui() could not get the text panel from name provided: " + panelName + ", returning default textformat.");
                    return new TextFormat();
                }
            }
            else
            {
                Application.error("GUIManager.getTextPanelToFormatFromGui() could not get the GUIScren from name provided: " + GUIName + ", returning default textformat.");
                return new TextFormat();
            }
        }

        /*returns the format from the text panel with the name provided from the current gui, Use this to change the formats of text panels.*/
        public static TextFormat getTextPanelFormatFromCurrentGUI(string panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    return panel.format;
                }
                else
                {
                    Application.error("GUIManager.getTextPanelToFormatFromCurrentGui() could not get the text panel from name provided: " + panelName + ", returning default textformat.");
                    return new TextFormat();
                }
            }
            else
            {
                Application.error("GUIManager.getTextPanelToFormatFromCurrentGui() was called but the current gui screen is null, returning default textformat.");
                return new TextFormat();
            }
        }

        public static void unHideTextPanelInGUI(string GUIName, string panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if (screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.unHide();
                }
                else
                {
                    Application.error("GUIManager.unHideTextPanelInGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIManager.unHideTextPanelInGUI() could not get the GUIScren from name provided: " + GUIName);
            }
        }
        public static void hideTextPanelInGUI(string GUIName, string panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if (screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.hide();
                }
                else
                {
                    Application.error("GUIManager.hideTextPanelInGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIManager.hideTextPanelInGUI() could not get the GUIScren from name provided: " + GUIName);
            }
        }

        public static void unHideTextPanelInCurrentGUI(string panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.unHide();
                }
                else
                {
                    Application.error("GUIManager.unHideTextPanelInCurrentGUI() could not get the text panel from name provided: " + panelName );
                }
            }
            else
            {
                Application.error("GUIManager.unHideTextPanelInCurrentGUI() was called but the current gui screen is null.");
            }
        }

        public static void hideTextPanelInCurrentGUI(string panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.hide();
                }
                else
                {
                    Application.error("GUIManager.hideTextPanelInCurrentGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIManager.hideTextPanelInCurrentGUI() was called but the current gui screen is null.");
            }
        }

        public static void rebuildTextInGUI(string guiName)
        {
            if (allGUIs.TryGetValue(guiName, out GUIScreen screen))
            {
                screen.buildText();
            }
            else
            {
                Application.error("GUIManager.rebuildTextInGUI() could not get the GUIScren from name provided: " + guiName);
            }
        }

        public static void rebuildTextInCurrentGUI()
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.buildText();
            }
        }

            /*Trys to display the guiscreen with the provided name and hides all the other ones.*/
            public static void displayGUIScreen(string name)
        {
            if(allGUIs.TryGetValue(name, out GUIScreen screen))
            {
                hideAllGUIScreens();
                screen.unHideWholeGUIScreen();
                currentDisplayedScreen = screen;
            }
            else
            {
                Application.error("GUIManager could not show requested GUIScreen named: " + name);
            }
        }

        public static void hideAllGUIScreens()
        {
            foreach(GUIScreen screen in allGUIs.Values)
            {
                screen.hideWholeGUIScreen();
                currentDisplayedScreen = null;
            }
        }

        public static void addNewGUIScreen(string name, string font, uint maxCharCount = 1024)
        {
            GUIScreen screenToAdd = new GUIScreen(name, font, maxCharCount);
            if(screenToAdd.isFontNull())
            {
                Application.error("GUIManager could not add requested GUIScreen, font name could not be successfully loaded.");
            }
            else
            {
                allGUIs.Add(name, screenToAdd);

                if(currentDisplayedScreen == null)//if there is no gui being currently displayed, display the recently added guiscreen
                {
                    currentDisplayedScreen = screenToAdd;
                }
            }
        }

        /*Called once every tick.*/
        public static void onTick()
        {
            foreach(GUIScreen screen in allGUIs.Values)
            {
                screen.onTick();
            }
        }

        public static void onWindowResize()
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.onWindowResize();
            }
        }
    }
}
