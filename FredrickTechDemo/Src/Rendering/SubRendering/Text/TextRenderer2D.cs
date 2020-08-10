using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering.Text;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    public class TextRenderer2D
    {
        private readonly String textShaderDir = ResourceHelper.getShaderFileDir("GuiTextShader.shader");
        private readonly String fontTextureDir;
        private readonly int screenEdgePadding = 10;
        private FontBuilder font;
        private float defaultFontSize = 0.02F;
        private ColourF defaultColour;
        private ModelDrawableDynamic screenTextModel;
        private int maxCharCount;
        private bool needsBuilding = false;//will be true if text has changed since last tick update and model needs to be recalculated

        private Dictionary<String, TextPanel2D> currentScreenTextPanels = new Dictionary<String, TextPanel2D>();

        public TextRenderer2D(String font, int maxCharacterCount = 256)
        {
            this.maxCharCount = maxCharacterCount;
            this.defaultColour = ColourF.white;
            this.fontTextureDir = ResourceHelper.getFontFileDir(font + ".png");
            this.font = new FontBuilder(font);
            this.screenTextModel = new ModelDrawableDynamic(textShaderDir, fontTextureDir, QuadBatcher.getIndicesForQuadCount(maxCharCount), maxCharCount * 4);
        }

        public TextRenderer2D(String font, ColourF color, int maxCharacterCount = 256)
        {
            this.maxCharCount = maxCharacterCount;
            this.defaultColour = color;
            this.fontTextureDir = ResourceHelper.getFontFileDir(font + ".png");
            this.font = new FontBuilder(font);
            this.screenTextModel = new ModelDrawableDynamic(textShaderDir, fontTextureDir, QuadBatcher.getIndicesForQuadCount(maxCharCount), maxCharCount * 4);

        }

        public void setColour(ColourF newColour)
        {
            this.defaultColour = newColour;
        }

        //lots of different addNewTextPanel() functions with different params for versatility
        #region addNewTextPanel
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, TextAlign alignment)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, defaultColour, defaultFontSize, alignment);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, TextAlign alignment)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, defaultFontSize, alignment);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, float fontSize, TextAlign alignment)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, fontSize, alignment);
        }

        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, TextAlign alignment)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, defaultColour, defaultFontSize, alignment);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, TextAlign alignment)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, textPanelColor, defaultFontSize, alignment);
        }

        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, defaultColour, defaultFontSize, TextAlign.LEFT);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, defaultFontSize, TextAlign.LEFT);
        }
        public void addNewTextPanel(String textPanelName, String textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, float fontSize)
        {
            addNewTextPanel(textPanelName, new String[] { textPanelLines }, textPanelPosition, textPanelColor, fontSize, TextAlign.LEFT);
        }

        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, defaultColour, defaultFontSize, TextAlign.LEFT);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, textPanelColor, defaultFontSize, TextAlign.LEFT);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, float fontSize)
        {
            addNewTextPanel(textPanelName, textPanelLines, textPanelPosition, textPanelColor, fontSize, TextAlign.LEFT);
        }
        public void addNewTextPanel(String textPanelName, String[] textPanelLines, Vector2F textPanelPosition, ColourF textPanelColor, float fontSize, TextAlign alignment)
        {
            currentScreenTextPanels.Remove(textPanelName);//removes the text panel if it exists, so that it can be updated instead.
            currentScreenTextPanels.Add(textPanelName, new TextPanel2D(textPanelLines, textPanelPosition,  textPanelColor, fontSize, screenEdgePadding, font, alignment));
            needsBuilding = true;
        }
        #endregion
         
        /*Combines all the vertex data of all of the text panels, and sends the information to the dynamic model.*/
        private void submitDataToDynamicModel()
        {
            Vertex[] fillerVertexArray = new Vertex[maxCharCount * 4];//creating array big enough to fill max char count
            if (currentScreenTextPanels.Count > 0)
            {
                //combine all models
                Model[] combinedModels = null;

                int totalModelCount = 0;
                foreach (KeyValuePair<String, TextPanel2D> pair in currentScreenTextPanels)
                {
                    totalModelCount += pair.Value.models.Length;
                }

                combinedModels = new Model[totalModelCount];

                int modelIndex = 0;
                foreach (KeyValuePair<String, TextPanel2D> pair in currentScreenTextPanels)
                {
                    for (int i = 0; i < pair.Value.models.Length; i++)
                    {
                        combinedModels[modelIndex + i] = pair.Value.models[i];
                    }
                    modelIndex += pair.Value.models.Length;
                }

                //get and combine all vertex arrays and submit them to model
                Vertex[] combinedVertices;
                QuadBatcher.combineData(combinedModels, out combinedVertices);

                if (combinedVertices.Length > maxCharCount * 4)
                {
                    Application.error("TextRenderer2D has gone over its provided limit of characters!");
                }
                else
                {
                    //fill combined vertices with zero values untill it reaches the defined maximum character limit * 4
                    //This must be done to override the old data in the vertex buffer
                    Array.Copy(combinedVertices, fillerVertexArray, combinedVertices.Length);
                }
                screenTextModel.submitData(fillerVertexArray);
            }
            else
            {
                screenTextModel.submitData(fillerVertexArray);//just fill buffer with empties to clear screen of text
            }

            needsBuilding = false;
        }
        
        /*Called every tick*/
        public void onTick()
        {
            Profiler.beginEndProfile(Profiler.textRender2DBuildingName);
            if (needsBuilding)
            {
                submitDataToDynamicModel();
            }
            Profiler.beginEndProfile(Profiler.textRender2DBuildingName);
        }

        public void removeTextPanel(String textPanelName)
        {
            if (currentScreenTextPanels.Remove(textPanelName))
            {
                needsBuilding = true;
            }
        }

        public void clearAllText()
        {
            currentScreenTextPanels.Clear();
            needsBuilding = true;
        }



        /*If theres any text model available it will be rendered.*/
        public void renderAnyText()
        {
            if(screenTextModel != null)
            {
                screenTextModel.draw();
            }
        }

        public void onWindowResize()
        {
            if (currentScreenTextPanels.Count > 0  && screenTextModel != null)
            {
                buildAll();
                needsBuilding = true;
            }
        }
        private void buildAll()
        {
            foreach (KeyValuePair<String, TextPanel2D> pair in currentScreenTextPanels)
            {
                pair.Value.build();
            }
        }

        public void delete()
        {
            screenTextModel.delete();
            screenTextModel = null;
        }
    }
}
