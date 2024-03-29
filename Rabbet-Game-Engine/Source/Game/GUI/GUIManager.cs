﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;


using System.Collections.Generic;

//TODO: Make so transparent gui elements can be rendered with blur so they look like frosted glass
namespace RabbetGameEngine
{
    public enum ComponentAnchor
    {
        CENTER_LEFT,
        CENTER,
        CENTER_RIGHT,
        CENTER_TOP,
        CENTER_BOTTOM,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }


    public static class GUIManager
    {
        public static int guiLineWidth = (GameInstance.realScreenHeight / 1000) + 1;

        public static Vector2 guiMapRes = new Vector2(1920, 1080);

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

        public static void onFrame()
        {
            Profiler.startSection("guiUpdate");
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onUpdate(true);
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onUpdate(true);
            }
            Profiler.endCurrentSection();
        }

        public static void onTick()
        {
            Profiler.startTickSection("guiUpdate");
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onUpdate(false);
                GameInstance.get.pauseGame();
            }
            else
            {
                GameInstance.get.unPauseGame();
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onUpdate(false);
            }
            Profiler.endCurrentTickSection();
        }


        public static void onWindowResize()
        {
            if (currentDisplayedGUI != null)
            {
                foreach (GUI g in guiStack)
                    g.onWindowResize();
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onWindowResize();
            }
        }

        public static void requestRender(bool isFrameUpdate)
        {
            foreach (GUI g in persistentGUIs.Values)
            {
                g.requestGUIRender(isFrameUpdate);
            }
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.requestGUIRender(isFrameUpdate);
            }

        }

        public static void openGUI(GUI g)
        {
            guiStack.Push(g);
            currentDisplayedGUI = g;
        }

        public static void closeCurrentGUI()
        {
            if (guiStack.TryPop(out _))
            {
                SoundManager.playSound("buttonclick");
                currentDisplayedGUI = guiStack.Count > 0 ? guiStack.Peek() : null;
            }
        }

        public static void addPersistentGUI(GUI g)
        {
            persistentGUIs.Add(g.guiName, g);
        }

        public static void removePersistentGUI(string guiName)
        {
            if (!persistentGUIs.Remove(guiName))
            {
                Application.warn("GUIManager could not remove the requested gui named " + guiName);
            }
        }

        public static void onKeyDown(KeyboardKeyEventArgs e)
        {
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onKeyDown(e);
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onKeyDown(e);
            }
        }

        public static void onMouseDown(MouseButtonEventArgs e)
        {
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onMouseDown(e);
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onMouseDown(e);
            }
        }

        public static void onMouseWheel(float scrollDelta)
        {
            if (currentDisplayedGUI != null)
            {
                currentDisplayedGUI.onMouseWheel(scrollDelta);
            }
            foreach (GUI g in persistentGUIs.Values)
            {
                g.onMouseWheel(scrollDelta);
            }
        }
    }
}
