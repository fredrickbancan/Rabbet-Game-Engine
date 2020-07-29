using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering.Text;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    class TextRenderer2D
    {
        private readonly String textShaderDir = ResourceHelper.getShaderFileDir("GuiTransparentShader.shader");
        private readonly String fontTextureDir;
        private FontFile font;

        private bool screenTextModelExists = false;
        private ColourF defaultColour;
        private Renderer gameRenderer;
        private ModelDrawableGUI screenTextModel;

        private Dictionary<String, TextPanel2D> currentScreenTextPanels = new Dictionary<String, TextPanel2D>();

        public TextRenderer2D(String font)
        {
            this.defaultColour = ColourF.white;
            fontTextureDir = ResourceHelper.getFontTextureFileDir(font + ".png");
            this.font = new FontFile(font);
        }

        public TextRenderer2D(String font, ColourF color)
        {
            this.defaultColour = color;
            fontTextureDir = ResourceHelper.getFontTextureFileDir(font + ".png");
            this.font = new FontFile(font);
        }

        public void setColour(ColourF newColour)
        {
            this.defaultColour = newColour;
        }

        public void addNewTextPanel(String textPanelName, String textPanelLine, Vector2F textPanelPosition, ColourF color)
        {
            addNewTextPanel(textPanelName, new string[] { textPanelLine }, textPanelPosition, color);
        }

        public void addNewTextPanel(String textPanelName, String textPanelLine, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, new string[] { textPanelLine}, textPanelPosition, defaultColour);
        }

        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, defaultColour);
        }

        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor)
        {
            currentScreenTextPanels.Add(textPanelName, new TextPanel2D(textPanelLines, textPanelPosition, textPanelColor, font));
            batchScreenTextForRendering();
        }

        public void removeTextPanel(String textPanelName)
        {
            if (!currentScreenTextPanels.Remove(textPanelName))
            {
                Application.error("TextRenderer2D could not remove panel named " + textPanelName);
            }
            else
            {
                batchScreenTextForRendering();
            }
        }

        public void batchScreenTextForRendering()
        {
            Application.say("TextRenderer2D Batching new screen text model");
            if(screenTextModelExists)
            {
                screenTextModel.delete();
            }
            int itterator = 0; // total number of line models
            foreach(KeyValuePair<String, TextPanel2D> entry in currentScreenTextPanels)
            {
                for(int j = 0; j < entry.Value.getTextPanelTextLines().Length; j++)
                {
                    itterator++;
                }
            }

            //build the array of line models according to the acculated size
            Model[] arrayOfLineModels = new Model[itterator];

            //loop through each enrty, then loop through each entry's array of line models and add each one to the model array.
            foreach (KeyValuePair<String, TextPanel2D> entry in currentScreenTextPanels)
            {
                for (int j = 0; j < entry.Value.getTextPanelTextLines().Length; j++)
                {
                    arrayOfLineModels[j] = entry.Value.getTextPanelTextLines()[j].getLineModel();
                }
            }

            screenTextModel = QuadBatcher.batchQuadModelsGui(arrayOfLineModels, textShaderDir, fontTextureDir);
            screenTextModelExists = true;
        }


        public void draw(float aspectRatio)
        {
            if(!screenTextModelExists || screenTextModel == null)
            {
                Application.error("TextRenderer2D attempted to draw without properly batched model!");
            }
            else
            {
                screenTextModel.draw(aspectRatio);
            }
        }

        public void delete()
        {
            screenTextModel.delete();
            screenTextModel = null;
            screenTextModelExists = false;
        }
    }
}
