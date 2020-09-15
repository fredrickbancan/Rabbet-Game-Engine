using Coictus.GUI.Text;
using Coictus.Models;
using Coictus.SubRendering.GUI.Text;
using System.Collections.Generic;

namespace Coictus.SubRendering.GUI
{
    /*An object containing multiple GUI based screen components to be displayed.
      Multiple gui screens can be created and displayed seperately.*/
    public class GUIScreen
    {
        private Dictionary<string, GUIScreenComponent> components = new Dictionary<string, GUIScreenComponent>();//all of the gui related components in this screen, such as crosshairs, health bars, menus ect. Each component can be individually hidden, changed or removed.
        private Dictionary<string, GUITextPanel> screenTextPanels = new Dictionary<string, GUITextPanel>();
        private ModelDrawableDynamic screenTextModel;
        private int preHideWindowWidth;
        private int preHideWindowHeight;
        private FontFace screenFont;
        private bool wholeScreenHidden = false;
        private uint maxCharCount;
        public string screenName = "";

        public GUIScreen(string screenName, string textFont, uint maxCharCount = 1024)
        {
            if(!TextUtil.tryGetFont(textFont, out screenFont))
            {
                Application.error("GUIScreen " + screenName + " could not load its provided font: " + textFont + ", it will have a null font!");
            }
            this.screenName = screenName;
            this.maxCharCount = maxCharCount;
            screenTextModel = new ModelDrawableDynamic(TextUtil.textShaderName, TextUtil.getTextureNameForScreenFont(screenFont), QuadBatcher.getIndicesForQuadCount((int)maxCharCount), (int)maxCharCount * 4);
        }

        public void buildScreenTextModel()
        {
            foreach(GUITextPanel panel in screenTextPanels.Values)
            {
                panel.buildOrRebuild();
            }
            TextModelBuilder2D.batchAndSubmitTextToDynamicModel(screenTextModel, screenTextPanels, maxCharCount);
        }

        /*Add new or change already existing gui component*/
        public void addGuiComponent(string name, GUIScreenComponent component)
        {
            if(components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.deleteComponent();
                components.Remove(name);
            }

            components.Add(name, component);
        }

        /*Add new text panel, or override existing one, Do not use this to update existing panels, use updateTextPanel() instead*/
        public void addTextPanel(string name, GUITextPanel textPanel)
        {
            if (screenTextPanels.TryGetValue(name, out GUITextPanel panel))
            {
                screenTextPanels.Remove(name);//If theres already the text panel, override it
            }
            else
            {
                textPanel.setFont(this.screenFont);
                textPanel.buildOrRebuild();
                screenTextPanels.Add(name, textPanel);
                buildScreenTextModel();
            }
        }

        public bool getTextPanel(string name, out GUITextPanel foundPanel)
        {
            if (screenTextPanels.TryGetValue(name, out GUITextPanel result))
            {
                foundPanel = result;
                return true;
            }
            else
            {
                Application.error("GUIScreen " + screenName + " could not find requested text panel to update: " + name);
                foundPanel = null;
                return false;
            }
        }

        public void hideWholeGUIScreen()
        {
            wholeScreenHidden = true;
            preHideWindowWidth = GameInstance.gameWindowWidth;
            preHideWindowHeight = GameInstance.gameWindowHeight;
        }
        public void unHideWholeGUIScreen()
        {
            wholeScreenHidden = false;

            if(preHideWindowWidth != GameInstance.gameWindowWidth || preHideWindowHeight != GameInstance.gameWindowHeight)
            {
                onWindowResize();
            }
        }

        public void hideComponent(string name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.setHide(true);
            }
        }
        public void unHideComponent(string name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.setHide(false);
            }
        }
        public void deleteComponent(string name)
        {
            if (components.TryGetValue(name, out GUIScreenComponent comp))
            {
                comp.deleteComponent();
                components.Remove(name);
            }
        }
        public void deleteTextPanel(string name)
        {
            if (screenTextPanels.TryGetValue(name, out GUITextPanel panel))
            {
                screenTextPanels.Remove(name);
                buildScreenTextModel();
            }
            else
            {
                Application.warn("GUIScreen " + screenName + " Could not remove requested text panel: " +  name);
            }
        }

        public void onTick()
        {
            foreach (GUIScreenComponent component in components.Values)
            {
                component.onTick();
            }
        }

        public void drawAll()
        {
            if (!wholeScreenHidden)
            {
                foreach (GUIScreenComponent component in components.Values)//temp, inefficient (no batches)
                {
                    component.draw();
                }
                screenTextModel.draw();
            }
        }

        public void onWindowResize()
        {
            if (!wholeScreenHidden)
            {
                foreach (GUIScreenComponent component in components.Values)
                {
                    component.onWindowResize();
                }
                buildScreenTextModel();
            }
        }

        public bool isFontNull()
        {
            return this.screenFont == null;
        }
    }
}
