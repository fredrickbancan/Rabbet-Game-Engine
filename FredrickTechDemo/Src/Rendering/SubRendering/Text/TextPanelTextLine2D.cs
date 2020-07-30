using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;
using System.Text;

namespace FredrickTechDemo.SubRendering.Text
{
    /*This object is responsable for converting a given string into a model made of quads for each character ignoring white space.
      The models should have the provided colour in their VAO and have their positions correct depending on white space, previous lines in panel,
    panel position, screen size etc*/
    class TextPanelTextLine2D
    {
        private static readonly float lineHeightMultiplier = 0.03F;
        private static readonly byte spaceAscii = 32;
        private Vector2F cursorPos;
        private Model lineModel;
        private float[] vertexXYZ;
        private float[] vertexRGB;
        private float[] vertexUV;
        private int vertexXYZIndex = 2;
        private int vertexRGBIndex = 2;
        private int vertexUVIndex = 1;
        private int previousLineCount;//used to tell how many lines to offset this line from the origin. Origin is the top left corner of the parent text panel
        private Vector3F color;
        private float fontSize;
        float verticalSizePerPixel;
        float horizontalSizePerPixel;

        public TextPanelTextLine2D(String textToBeConverted, Vector2F panelPosition, ColourF color, float fontSize, int previousLineCount, FontReader font)
        {
            this.color = color.normalize();
            this.cursorPos = panelPosition;
            this.fontSize = fontSize;
            this.previousLineCount = previousLineCount;
            this.vertexXYZ = new float[textToBeConverted.Replace(" ", "").Length * 12];//number of vertices for each character. Excluding spaces.
            this.vertexRGB = new float[textToBeConverted.Replace(" ", "").Length * 12];
            this.vertexUV = new float[textToBeConverted.Replace(" ", "").Length * 8];

            this.convertStringToCharacterArrayModel(textToBeConverted, panelPosition, font);
        }


        private void convertStringToCharacterArrayModel(String text, Vector2F panelTopLeftPosition, FontReader font)
        {
            this.verticalSizePerPixel = (lineHeightMultiplier / font.getLineHeightPixels()) * fontSize;
            this.horizontalSizePerPixel = verticalSizePerPixel / GameInstance.aspectRatio;
            this.cursorPos.y -= (previousLineCount + 1) * verticalSizePerPixel;
            byte[] charIds = Encoding.ASCII.GetBytes(text);

            for (int i = 0; i < text.Length; i++)
            {
                if (charIds[i] == spaceAscii)
                {
                    this.cursorPos.x += font.getSpaceWidthPixels() * verticalSizePerPixel;
                }
                else
                {
                    Character currentChar = font.getCharacter(charIds[i]);
                    addVerticesForCharAtCursor(currentChar);
                    this.cursorPos.x += currentChar.getXAdvancePixels() * horizontalSizePerPixel;
                }
            }
            this.lineModel = new Model(vertexXYZ, vertexRGB, vertexUV);
        }
        
        private void addVerticesForCharAtCursor(Character character)
        {
            Vector2F posMin;
            posMin.x = cursorPos.x + character.getxOffsetPixels() * horizontalSizePerPixel;
            posMin.y = cursorPos.y + character.getyOffsetPixels() * verticalSizePerPixel;

            Vector2F posMax;
            posMax.x = posMin.x + character.getXPixels() * horizontalSizePerPixel;
            posMax.y = posMin.y + character.getYPixels() * verticalSizePerPixel;

            posMin.x = 2 * posMin.x - 1F;
            posMax.x = 2 * posMax.x - 1F;

            addVertex(posMin.x, posMin.y, character.getU(), character.getV());
            addVertex(posMax.x, posMin.y, character.getUMax(), character.getV());
            addVertex(posMin.x, posMax.y, character.getU(), character.getVMax());
            addVertex(posMax.x, posMax.y, character.getUMax(), character.getVMax());
        }

        private void addVertex(float x, float y, float u, float v)
        {
            this.vertexXYZ[vertexXYZIndex - 2] = x;
            this.vertexXYZ[vertexXYZIndex - 1] = y;
            this.vertexXYZ[vertexXYZIndex - 0] = 0.0F;

            this.vertexRGB[vertexRGBIndex - 2] = color.x;
            this.vertexRGB[vertexRGBIndex - 1] = color.y;
            this.vertexRGB[vertexRGBIndex - 0] = color.z;

            this.vertexUV[vertexUVIndex - 1] = u;
            this.vertexUV[vertexUVIndex - 0] = v;

            //do last
            this.vertexXYZIndex += 3;
            this.vertexRGBIndex += 3;
            this.vertexUVIndex += 2;
        }
        public Model getLineModel()
        {
            return this.lineModel;
        }
    }
}
