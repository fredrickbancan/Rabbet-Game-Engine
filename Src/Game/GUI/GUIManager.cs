using System.Collections.Generic;

namespace RabbetGameEngine
{
    public static class GUIManager//TODO: Change to handle GUI's as objects.
    {
        /// <summary>
        /// stack of gui's opened one after the other
        /// </summary>
        private static Stack<GUI> guiStack;

        /// <summary>
        /// A single GUI which stays open persistently
        /// </summary>
        private static GUI persistentGUI;

        /// <summary>
        /// The currently displayed GUI from the stack, if any.
        /// </summary>
        private static GUI currentDisplayedGUI;

        public static void onTick()
        {
            if(persistentGUI != null)
            {
                persistentGUI.onTick();
            }

            if(currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onTick();
            }
        }

        public static void onWindowResize()
        {
            if(currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onWindowResize();
            }

            if (persistentGUI != null)
            {
                persistentGUI.onWindowResize();
            }
        }

        public static void requestRender()
        {
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.requestGUIRender();
                currentDisplayedGUI.requestTextRender();
            }

            if (persistentGUI != null)
            {
                persistentGUI.requestGUIRender();
                persistentGUI.requestTextRender();
            }
        }
    }
}
