using FredrickTechDemo.GUI.Text;
using FredrickTechDemo.SubRendering.GUI;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.GUI
{
    /*This file will handle all gui related logic and requests*/
    public static class GUIHandler
    {
        private static Dictionary<String, GUIScreen> allGUIs = new Dictionary<String, GUIScreen>();
        private static GUIScreen currentDisplayedScreen;

        public static String getCurrentGUIScreenName()
        {
            if (currentDisplayedScreen != null)
            {
                return currentDisplayedScreen.screenName;
            }
            else return "none";
        }

        public static void addGUIComponentToCurrentGUI(String componentName, GUIScreenComponent component)
        {
            if (currentDisplayedScreen != null)
            {
                currentDisplayedScreen.addGuiComponent(componentName, component);
            }
        }

        public static void addGUIComponentToGUI(String GUIName, String componentName, GUIScreenComponent component)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                currentDisplayedScreen.addGuiComponent(componentName, component);
            }
        }

        public static void addTextPanelToCurrentGUI(String panelName, GUITextPanel panel)
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.addTextPanel(panelName, panel);
            }
        }
        public static void addTextPanelToGUI(String GUIName, String panelName, GUITextPanel panel)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                currentDisplayedScreen.addTextPanel(panelName, panel);
            }
        }

        /*returns the format from the text panel with the name provided from the gui name provided, Use this to change the formats of text panels.*/
        public static TextFormat getTextPanelFormatFromGUI(String GUIName, String panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if(screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    return panel.format;
                }
                else
                {
                    Application.error("GUIHandler.getTextPanelToFormatFromGui() could not get the text panel from name provided: " + panelName + ", returning default textformat.");
                    return new TextFormat();
                }
            }
            else
            {
                Application.error("GUIHandler.getTextPanelToFormatFromGui() could not get the GUIScren from name provided: " + GUIName + ", returning default textformat.");
                return new TextFormat();
            }
        }

        /*returns the format from the text panel with the name provided from the current gui, Use this to change the formats of text panels.*/
        public static TextFormat getTextPanelFormatFromCurrentGUI(String panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    return panel.format;
                }
                else
                {
                    Application.error("GUIHandler.getTextPanelToFormatFromCurrentGui() could not get the text panel from name provided: " + panelName + ", returning default textformat.");
                    return new TextFormat();
                }
            }
            else
            {
                Application.error("GUIHandler.getTextPanelToFormatFromCurrentGui() was called but the current gui screen is null, returning default textformat.");
                return new TextFormat();
            }
        }

        public static void unHideTextPanelInGUI(String GUIName, String panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if (screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.unHide();
                }
                else
                {
                    Application.error("GUIHandler.unHideTextPanelInGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIHandler.unHideTextPanelInGUI() could not get the GUIScren from name provided: " + GUIName);
            }
        }
        public static void hideTextPanelInGUI(String GUIName, String panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUIScreen screen))
            {
                if (screen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.hide();
                }
                else
                {
                    Application.error("GUIHandler.hideTextPanelInGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIHandler.hideTextPanelInGUI() could not get the GUIScren from name provided: " + GUIName);
            }
        }

        public static void unHideTextPanelInCurrentGUI(String panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.unHide();
                }
                else
                {
                    Application.error("GUIHandler.unHideTextPanelInCurrentGUI() could not get the text panel from name provided: " + panelName );
                }
            }
            else
            {
                Application.error("GUIHandler.unHideTextPanelInCurrentGUI() was called but the current gui screen is null.");
            }
        }
        public static void hideTextPanelInCurrentGUI(String panelName)
        {
            if (currentDisplayedScreen != null)
            {
                if (currentDisplayedScreen.getTextPanel(panelName, out GUITextPanel panel))
                {
                    panel.hide();
                }
                else
                {
                    Application.error("GUIHandler.hideTextPanelInCurrentGUI() could not get the text panel from name provided: " + panelName);
                }
            }
            else
            {
                Application.error("GUIHandler.hideTextPanelInCurrentGUI() was called but the current gui screen is null.");
            }
        }

        public static void rebuildTextInGUI(String guiName)
        {
            if (allGUIs.TryGetValue(guiName, out GUIScreen screen))
            {
                screen.buildScreenTextModel();
            }
            else
            {
                Application.error("GUIHandler.rebuildTextInGUI() could not get the GUIScren from name provided: " + guiName);
            }
        }

        public static void rebuildTextInCurrentGUI()
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.buildScreenTextModel();
            }
        }

            /*Trys to display the guiscreen with the provided name and hides all the other ones.*/
            public static void displayGUIScreen(String name)
        {
            if(allGUIs.TryGetValue(name, out GUIScreen screen))
            {
                hideAllGUIScreens();
                screen.unHideWholeGUIScreen();
                currentDisplayedScreen = screen;
            }
            else
            {
                Application.error("GUIHandler could not show requested GUIScreen named: " + name);
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

        public static void addNewGUIScreen(String name, String font, UInt32 maxCharCount = 1024)
        {
            GUIScreen screenToAdd = new GUIScreen(name, font, maxCharCount);
            if(screenToAdd.isFontNull())
            {
                Application.error("GUIHandler could not add requested GUIScreen, font name could not be successfully loaded.");
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

        public static void drawCurrentGUIScreen()
        {
            if(currentDisplayedScreen != null)
            {
                currentDisplayedScreen.drawAll();
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
