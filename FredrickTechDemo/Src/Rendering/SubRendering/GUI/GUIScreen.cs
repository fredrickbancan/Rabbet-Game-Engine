using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering.Text;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    /*An object containing multiple GUI based screen components to be displayed.
      Multiple gui screens can be created and displayed seperately.*/
    public class GUIScreen
    {
        private Dictionary<String, GUIScreenComponent> components = new Dictionary<String, GUIScreenComponent>();//all of the gui related components in this screen, such as crosshairs, health bars, menus ect. Each component can be individually hidden, changed or removed.
        private Dictionary<String, GUITextPanel> screenTextPanels = new Dictionary<String, GUITextPanel>();
        private ModelDrawableDynamic screenTextModel;
        private Font screenFont;
        private bool wholeScreenHidden = false;
        private UInt32 maxCharCount;

        public GUIScreen(Font textFont, UInt32 maxCharCount = 1024)
        {
            screenFont = textFont;
            this.maxCharCount = maxCharCount;
            screenTextModel = new ModelDrawableDynamic(TextUtil.textShaderDir, TextUtil.getTextureDirForFont(textFont), QuadBatcher.getIndicesForQuadCount((int)maxCharCount));
        }

        private void buildTextModel()
        {
            TextModelBuilder2D.batchAndSubmitTextToDynamicModel(screenTextModel, screenTextPanels, maxCharCount);
        }

        /*Add new or change already existing gui component*/
        public void addGuiComponent(String name, GUIScreenComponent component)
        {
            if(components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.deleteComponent();
                components.Remove(name);
            }

            components.Add(name, component);
        }

        public void hideWholeGUIScreen()
        {
            wholeScreenHidden = true;
        }
        public void unHideWholeGUIScreen()
        {
            wholeScreenHidden = false;
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
            if (!wholeScreenHidden)
            {
                Profiler.beginEndProfile("GUI draw");
                foreach (GUIScreenComponent component in components.Values)
                {
                    component.draw();
                }
                Profiler.beginEndProfile("GUI draw");
            }
        }

        public void onWindowResize()
        {
            foreach (GUIScreenComponent component in components.Values)
            {
                component.onWindowResize();
            }

            foreach(GUITextPanel panel in screenTextPanels.Values)
            {
                panel.buildOrRebuild();
            }
        }
    }
}
