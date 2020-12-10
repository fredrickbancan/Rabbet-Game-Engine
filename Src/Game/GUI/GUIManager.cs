using RabbetGameEngine.Debugging;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public enum ComponentAlignment
    {
        LEFT,
        CENTER,
        RIGHT
    }
    public static class GUIManager//TODO: Change to handle GUI's as objects.
    {
        /// <summary>
        /// stack of gui's opened one after the other
        /// </summary>
        private static Stack<GUI> guiStack = new Stack<GUI>();

        /// <summary>
        /// List of GUIs which persistently stay open on top of each other, e.g, a HUD and debug info
        /// </summary>
        private static Dictionary<string, GUI> persistentGUIs = new Dictionary<string, GUI>();

        /// <summary>
        /// The currently displayed GUI from the stack, if any.
        /// </summary>
        private static GUI currentDisplayedGUI = null;

        public static void doUpdate()
        {
            Profiler.beginEndProfile("guiUpdate");
            if(currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onUpdate();
                GameInstance.get.pauseGame();
            }
            else
            {
                GameInstance.get.unPauseGame();
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onUpdate();
            }
            Profiler.beginEndProfile("guiUpdate");
        }

        public static void onWindowResize()
        {
            if(currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onWindowResize();
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onWindowResize();
            }
        }

        public static void requestRender()
        {
            foreach(GUI g in persistentGUIs.Values)
            {
                g.requestGUIRender();
                g.requestTextRender();
            }
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.requestGUIRender();
                currentDisplayedGUI.requestTextRender();
            }

        }

        public static void openGUI(GUI g)
        {
            guiStack.Push(g);
            currentDisplayedGUI = g;
        }

        public static void closeCurrentGUI()
        {
            if(guiStack.TryPop(out _))
            {
                currentDisplayedGUI = guiStack.Count > 0 ? guiStack.Peek() : null;
            }
        }

        public static void addPersistentGUI(GUI g)
        {
            persistentGUIs.Add(g.guiName, g);
        }

        public static void removePersistentGUI(string guiName)
        {
            if(!persistentGUIs.Remove(guiName))
            {
                Application.warn("GUIManager could not remove the requested gui named " + guiName);
            }
        }
    }
}
