using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    class TextPanelTextLine2D
    {
        private Model lineModel;
        private float consolasCharWidth = 0.05F;//TODO change to actual consolas char width
        private float[] vertexXYZ;
        private float[] vertexRGB;
        private float[] vertexUV;
        public TextPanelTextLine2D(String textToBeConverted, Vector2F pos, ColourF color)
        {

            vertexXYZ = new float[textToBeConverted.Replace(" ", "").Length * 3];

            vertexRGB = new float[textToBeConverted.Replace(" ", "").Length * 3];

            vertexUV = new float[textToBeConverted.Replace(" ", "").Length * 2];
            convertStringToCharacterArrayModel(textToBeConverted, pos, color);
        }

        /*pos is where the line of text will start, the left bottom corner of the first character in text*/
        private void convertStringToCharacterArrayModel(String text, Vector2F pos, ColourF color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                //convert string into model containing vertex positions for each character quad
                //each character quad will need to have the correct positioning depending on letter, since each letter will have different height.
            }
        }

        public Model getLineModel()
        {
            return this.lineModel;
        }
    }
}
