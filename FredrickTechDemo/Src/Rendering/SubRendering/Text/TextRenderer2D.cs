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
        private readonly byte screenEdgePadding = 10;
        private FontBuilder font;
        private float defaultFontSize = 1.0F;
        private ColourF defaultColour;
        private ModelDrawableDynamic screenTextModel;
        private int maxCharCount;

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
            currentScreenTextPanels.Remove(textPanelName);
            currentScreenTextPanels.Add(textPanelName, new TextPanel2D(textPanelLines, textPanelPosition,  textPanelColor, fontSize, screenEdgePadding, font));
            buildAndSubmitDataToDynamicModel();
        }
        #endregion

        private void buildAndSubmitDataToDynamicModel()
        {
            Profiler.beginEndProfile("TextRenderer Submitting");
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

                //fill combined vertices with zero values untill it reaches the defined maximum character limit * 4
                //This must be done to override the old data in the vertex buffer

                Vertex[] combinedVerticesFilled = new Vertex[combinedVertices.Length + (maxCharCount * 4 - combinedVertices.Length)];//creating array big enough to fill max char count
                Array.Copy(combinedVertices, combinedVerticesFilled, combinedVertices.Length);
                screenTextModel.submitData(combinedVerticesFilled);
            }
            else
            {
                screenTextModel.submitData(new Vertex[0]);
            }
            Profiler.beginEndProfile("TextRenderer Submitting");
        }

        public void removeTextPanel(String textPanelName)
        {
            if (!currentScreenTextPanels.Remove(textPanelName))
            {
                Application.error("TextRenderer2D removeTextPanel() could not remove panel named " + textPanelName);
            }
            else
            {
                buildAndSubmitDataToDynamicModel();
            }
        }

        public void clearAllText()
        {
            currentScreenTextPanels.Clear();
            buildAndSubmitDataToDynamicModel();
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
                buildAndSubmitDataToDynamicModel();
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
