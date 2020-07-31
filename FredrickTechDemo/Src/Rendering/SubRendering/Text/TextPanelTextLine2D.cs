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
        private static readonly byte spaceAscii = 32;
        private Vector2F cursorPixelPos;
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
        private byte screenEdgePaddingPixels = 10;//number of pixels to pad the top and left edges of screen

        public TextPanelTextLine2D(String textToBeConverted, Vector2F pos, Vector3F color, float fontSize, int previousLineCount, FontReader font)
        {
            this.color = color;
            cursorPixelPos = pos;
            this.fontSize = fontSize;
            this.previousLineCount = previousLineCount;
            this.vertexXYZ = new float[textToBeConverted.Replace(" ", "").Length * 12];//number of vertices for each character. Excluding spaces.
            this.vertexRGB = new float[textToBeConverted.Replace(" ", "").Length * 12];
            this.vertexUV = new float[textToBeConverted.Replace(" ", "").Length * 8];

            this.convertStringToCharacterArrayModel(textToBeConverted, pos, font);
        }


        /*Loop through each letter and convert it to a quad on the screen*/
        private void convertStringToCharacterArrayModel(String text, Vector2F panelTopLeftPosition, FontReader font)
        {
            this.cursorPixelPos.y += previousLineCount * font.getLineHeightPixels();
            byte[] charIds = Encoding.ASCII.GetBytes(text);

            for (int i = 0; i < text.Length; i++)
            {
                if (charIds[i] == spaceAscii)
                {
                    this.cursorPixelPos.x += font.getSpaceWidthPixels() * fontSize;
                }
                else
                {
                    Character currentChar = font.getCharacter(charIds[i]);
                    addVerticesForCharAtCursor(currentChar, font);
                    this.cursorPixelPos.x += currentChar.getXAdvancePixels() * fontSize;
                }
            }
            this.lineModel = new Model(vertexXYZ, vertexRGB, vertexUV);
        }
        
        /*Add the vertices for the letter at the virtual cursor position*/
        private void addVerticesForCharAtCursor(Character character, FontReader font)
        {
            //get pixel values
            float x = cursorPixelPos.x + character.getxOffsetPixels() * fontSize + screenEdgePaddingPixels;
            float y = cursorPixelPos.y + character.getyOffsetPixels() * fontSize ;
            float xMax = x + character.getXPixels() * fontSize;
            float yMax = y + character.getYPixels() * fontSize;
            float u = character.getUPixels();
            float v = character.getVPixels();
            float uMax = character.getUPixelMax();
            float vMax = character.getVPixelMax();

            //convert pixel values to vertices in screen coords
            //x = (x * fontSize);
          //  y = (y * fontSize);
           // xMax = (xMax * fontSize);
           // yMax = (yMax * fontSize);
            x = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, x);
            y = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, y);
            xMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, xMax);
            yMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, yMax);
            y = -y;
            yMax = -yMax;
            //convert pixel uv values to opengl uv values for texture atlas
            u = u / font.getImagePixelWidth();
            v = 1 - (v / font.getImagePixelHeight());//1 minus because opengl starts from bottom left and these uv's are from top left
            uMax = uMax / font.getImagePixelWidth();
            vMax = 1 - (vMax / font.getImagePixelHeight());//1 minus because opengl starts from bottom left and these uv's are from top left

            //add vertices at screen coords with uv
            addVertexScreenCoords(x, y, u, v);//Bottom left vertex
            addVertexScreenCoords(xMax, y, uMax, v);//bottom right vertex
            addVertexScreenCoords(x, yMax, u, vMax);//top left vertex
            addVertexScreenCoords(xMax, yMax, uMax, vMax);//top right vertex
        }

        /*Add vertices to screen space*/
        private void addVertexScreenCoords(float x, float y, float u, float v)
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
