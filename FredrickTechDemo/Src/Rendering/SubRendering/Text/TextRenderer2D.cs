using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering.Text;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    class TextRenderer2D
    {
        private readonly String textShaderDir = ResourceHelper.getShaderFileDir("GuiTextShader.shader");
        private readonly String fontTextureDir;
        private FontBuilder font;
        private float defaultFontSize = 1.0F;
        private bool screenTextModelExists = false;
        private ColourF defaultColour;
        private ModelDrawable screenTextModel;

        private Dictionary<String, TextPanel2D> currentScreenTextPanels = new Dictionary<String, TextPanel2D>();

        public TextRenderer2D(String font)
        {
            this.defaultColour = ColourF.white;
            fontTextureDir = ResourceHelper.getFontTextureFileDir(font + ".png");
            this.font = new FontBuilder(font);
        }

        public TextRenderer2D(String font, ColourF color)
        {
            this.defaultColour = color;
            fontTextureDir = ResourceHelper.getFontTextureFileDir(font + ".png");
            this.font = new FontBuilder(font);
        }

        public void setColour(ColourF newColour)
        {
            this.defaultColour = newColour;
        }


        #region addNewTextPanel
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, defaultColour, defaultFontSize);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, defaultFontSize);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor,float fontSize)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, fontSize);
        }

        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, defaultColour, defaultFontSize);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, textPanelColor, defaultFontSize);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, float fontSize)
        {
            if(currentScreenTextPanels.ContainsKey(textPanelName))
            {
                if (!currentScreenTextPanels.Remove(textPanelName))
                {
                    Application.error("TextRenderer2D addNewTextPanel() could not remove old panel named " + textPanelName);
                }
            }
            currentScreenTextPanels.Add(textPanelName, new TextPanel2D(textPanelLines, textPanelPosition,  textPanelColor, fontSize, font));
            batchScreenTextForRendering();
        }
        #endregion


        public void removeTextPanel(String textPanelName)
        {
            if (!currentScreenTextPanels.Remove(textPanelName))
            {
                Application.error("TextRenderer2D removeTextPanel() could not remove panel named " + textPanelName);
            }
            else
            {
                batchScreenTextForRendering();
            }
        }

        /*Builds or re builds all the text*/
        private void buildAllText()
        {
            foreach(KeyValuePair<String, TextPanel2D> pair in currentScreenTextPanels)
            {
                pair.Value.build();
            }
        }

        public void clearAllText()
        {
            currentScreenTextPanels.Clear();
            delete();
        }

        public void batchScreenTextForRendering()
        {
            long startTime = TicksAndFps.getMiliseconds();
            if(screenTextModelExists && screenTextModel != null)
            {
                delete();
            }
            if(currentScreenTextPanels.Count > 0)
            {
                buildAllText();
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
            int index = 0;//an index must be used to stack the models in the array after each panel
            foreach (KeyValuePair<String, TextPanel2D> entry in currentScreenTextPanels)
            {
                int previousModelCount = 0;
                for (int j = 0; j < entry.Value.getTextPanelTextLines().Length; j++)
                {
                    arrayOfLineModels[index + j] = entry.Value.getTextPanelTextLines()[j].getLineModel();
                    previousModelCount++;
                }
                index += previousModelCount;
            }

            if (arrayOfLineModels.Length > 0)
            {
                screenTextModel = QuadBatcher.batchQuadModels(arrayOfLineModels, textShaderDir, fontTextureDir);
                screenTextModelExists = true;
            }
            Application.debug("TextRenderer2D took " + (TicksAndFps.getMiliseconds() - startTime) + " miliseconds to batch text.");
        }


        /*If theres any text model available it will be rendered.*/
        public void renderAnyText()
        {
            if(screenTextModelExists && screenTextModel != null)
            {
                screenTextModel.draw();
            }
        }

        public void onWindowResize()
        {
            if (currentScreenTextPanels.Count > 0 && screenTextModelExists && screenTextModel != null)
            {
                batchScreenTextForRendering();
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
