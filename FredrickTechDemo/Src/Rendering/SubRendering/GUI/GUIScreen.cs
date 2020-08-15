using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    /*An object containing multiple GUI based screen components to be displayed.
      Multiple gui screens can be created and displayed seperately*/
    public class GUIScreen
    {
        private Dictionary<String, GUIScreenComponent> components;
        private TextRenderer2D textRenderer;

        public GUIScreen()
        {
            components = new Dictionary<String, GUIScreenComponent>();
            textRenderer = new TextRenderer2D("Trebuchet", 1024);
        }

        public void addGuiComponent(String name, GUIScreenComponent component)
        {
            if(components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.deleteComponent();
                components.Remove(name);
            }

            components.Add(name, component);
        }

        public void hideComponent(String name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.setHide(true);
            }
        }
        public void unHideComponent(String name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.setHide(false);
            }
        }
        public void deleteComponent(String name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.deleteComponent();
                components.Remove(name);
            }
        }

        public void onTick()
        {
            foreach (GUIScreenComponent component in components.Values)
            {
                component.onTick();
            }
        }

        public void drawAll()//temp, inefficient (no batches)
        {
            Profiler.beginEndProfile("GUI draw");
            foreach(GUIScreenComponent component in components.Values)
            {
                component.draw();
            }
            Profiler.beginEndProfile("GUI draw");
        }

        public void onWindowResize()
        {
            foreach (GUIScreenComponent component in components.Values)
            {
                component.onWindowResize();
            }
        }
    }
}
