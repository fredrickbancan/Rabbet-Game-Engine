using OpenTK.Windowing.Common;

using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUI
    {
        private Dictionary<string, GUIComponent> components = new Dictionary<string, GUIComponent>();//all of the gui related components in this GUI, such as crosshairs, health bars, menus ect. Each component can be individually hidden, changed or removed.
        public FontFace guiFont;
        private bool wholeScreenHidden = false;
        private uint maxCharCount;
        public string guiName = "";
        public GUI(string guiName, string textFont, uint maxCharCount = 1024)
        {
            if(!TextUtil.tryGetFont(textFont, out guiFont))
            {
                Application.error("GUIScreen " + guiName + " could not load its provided font: " + textFont + ", it will have a null font!");
            }
            this.guiName = guiName;
            this.maxCharCount = maxCharCount;
        }
        public void requestGUIRender()
        {
            foreach(GUIComponent comp in components.Values)
            {
                comp.requestRender();
            }
        }
        /// <summary>
        ///  Add new or change already existing gui component
        /// </summary>
        public void addGuiComponent(string name, GUIComponent component)
        {
            if(components.TryGetValue(name, out GUIComponent comp))
            {
                components.Remove(name);
            }

            components.Add(name, component);
            component.setName(name);
            component.updateRenderData();
        }

        public void hideComponent(string name)
        {
            if (components.TryGetValue(name, out GUIComponent comp))
            {
                comp.setHide(true);
            }
        }
        public void unHideComponent(string name)
        {
            if (components.TryGetValue(name, out GUIComponent comp))
            {
                comp.setHide(false);
            }
        }
        public void deleteComponent(string name)
        {
            if (components.TryGetValue(name, out GUIComponent comp))
            {
                components.Remove(name);
            }
        }

        public virtual void onUpdate()
        {
            foreach (GUIComponent component in components.Values)
            {
                if(!component.paused)
                    component.onUpdate();
            }
        }

        public virtual void onFrame()
        {
            foreach (GUIComponent component in components.Values)
            {
                if (!component.paused)
                    component.onFrame();
            }
        }

        public virtual void onComponentValueChanged()
        {

        }

        public void onWindowResize()
        {
            if (!wholeScreenHidden)
            {
                foreach (GUIComponent component in components.Values)
                {
                    component.onWindowResize();
                }
            }
        }

        public bool isFontNull()
        {
            return this.guiFont == null;
        }

        public void pauseAllExcept(GUIComponent g)
        {
            foreach(GUIComponent c in components.Values)
            {
                if(c != g)
                c.pause();
            }
        }

        public void unPauseAll()
        {
            foreach (GUIComponent c in components.Values)
            {
                c.unPause();
            }
        }
        public virtual void onKeyDown(KeyboardKeyEventArgs e)
        {
            foreach (GUIComponent c in components.Values)
            {
                c.onKeyDown(e);
            }
        }

        public virtual void onMouseDown(MouseButtonEventArgs e)
        {
            foreach (GUIComponent c in components.Values)
            {
                c.onMouseDown(e);
            }
        }

        public virtual void onMouseWheel(float scrollDelta)
        {
            foreach (GUIComponent c in components.Values)
            {
                c.onMouseWheel(scrollDelta);
            }
        }
    }
}
