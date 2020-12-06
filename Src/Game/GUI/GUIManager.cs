using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public static class GUIManager//TODO: Change to handle GUI's as objects.
    {
        private static Dictionary<string, GUI> allGUIs = new Dictionary<string, GUI>();
        private static GUI currentDisplayedScreen;

        public static void addGUIComponentToGUI(string GUIName, string componentName, GUIComponent component)
        {
            if (allGUIs.TryGetValue(GUIName, out GUI screen))
            {
                currentDisplayedScreen.addGuiComponent(componentName, component);
            }
        }
        public static void addTextPanelToGUI(string GUIName, string panelName, GUITextPanel panel)
        {
            if (allGUIs.TryGetValue(GUIName, out GUI screen))
            {
                currentDisplayedScreen.addTextPanel(panelName, panel);
            }
        }

        /// <summary>
        ///  returns the format from the text panel with the name provided from the gui name provided, Use this to change the formats of text panels.
        /// </summary>
        public static TextFormat getTextPanelFormatFromGUI(string GUIName, string panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUI screen))
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


        public static void unHideTextPanelInGUI(string GUIName, string panelName)
        {
            if (allGUIs.TryGetValue(GUIName, out GUI screen))
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
            if (allGUIs.TryGetValue(GUIName, out GUI screen))
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

        public static void rebuildTextInGUI(string guiName)
        {
            if (allGUIs.TryGetValue(guiName, out GUI screen))
            {
                screen.buildText();
            }
            else
            {
                Application.error("GUIManager.rebuildTextInGUI() could not get the GUIScren from name provided: " + guiName);
            }
        }
        [Obsolete]
        public static void addNewGUIScreen(string name, string font, uint maxCharCount = 1024)
        {
            GUI screenToAdd = new GUI(name, font, maxCharCount);
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
        public static void onTick()
        {
            foreach(GUI screen in allGUIs.Values)
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
