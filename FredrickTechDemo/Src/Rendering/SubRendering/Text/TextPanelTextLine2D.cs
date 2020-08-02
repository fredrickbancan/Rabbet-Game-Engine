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
        private Vertex[] vertices;
        private int vertexIndex;
        private int previousLineCount;//used to tell how many lines to offset this line from the origin. Origin is the top left corner of the parent text panel
        private Vector4F color;
        private float fontSize;
        private byte screenEdgePaddingPixels = 10;//number of pixels to pad the top and left edges of screen

        public TextPanelTextLine2D(String textToBeConverted, Vector2F pos, Vector4F color, float fontSize, int previousLineCount, FontBuilder font)
        {
            this.color = color;
            cursorPixelPos = pos;
            this.fontSize = fontSize;
            this.previousLineCount = previousLineCount;
            this.vertices = new Vertex[textToBeConverted.Replace(" ", "").Length * 4];//number of vertices for each character. Excluding spaces.

            this.convertStringToCharacterArrayModel(textToBeConverted, pos, font);
        }


        /*Loop through each letter and convert it to a quad on the screen*/
        private void convertStringToCharacterArrayModel(String text, Vector2F panelTopLeftPosition, FontBuilder font)
        {
            this.cursorPixelPos.y += previousLineCount * (font.getLineHeightPixels() * fontSize);
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
            this.lineModel = new Model(vertices);
        }
        
        /*Add the vertices for the letter at the virtual cursor position*/
        private void addVerticesForCharAtCursor(Character character, FontBuilder font)
        {
            //get pixel values
            float x = cursorPixelPos.x + character.getxOffsetPixels() * fontSize + screenEdgePaddingPixels;
            float y = cursorPixelPos.y + character.getyOffsetPixels() * fontSize + screenEdgePaddingPixels;
            float xMax = x + character.getPixelWidth() * fontSize;
            float yMax = y + character.getPixelHeight() * fontSize;
            float u = character.getU();
            float v = character.getV();
            float uMax = character.getUMax();
            float vMax = character.getVMax();

            //convert pixel values to vertices in screen coords
            x = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, x);
            y = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, y);
            xMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, xMax);
            yMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, yMax);

            //so far we have been treating this system like the top left corner of the screen is smallest
            //so now at the end we flip it
            //flip y 
            y = -y;
            yMax = -yMax;

            //add vertices at screen coords with uv, in different order so the quads will now face fowards after being flipped, so that they show when face culling.
            addVertexScreenCoords(xMax, y,color.r + 0.1F, color.g + 0.1F, color.b + 0.1F, color.a, uMax, v);//bottom right vertex1
            addVertexScreenCoords(x, y, color.r + 0.1F, color.g + 0.1F, color.b + 0.1F, color.a, u, v);//Bottom left vertex0
            addVertexScreenCoords(xMax, yMax, color.r * 0.5F, color.g * 0.5F, color.b * 0.5F, color.a, uMax, vMax);//top right vertex3
            addVertexScreenCoords(x, yMax, color.r * 0.5F, color.g * 0.5F, color.b * 0.5F, color.a, u, vMax);//top left vertex2
        }

        /*Add vertices to screen space*/
        private void addVertexScreenCoords(float x, float y,float r, float g, float b, float a, float u, float v)
        {
            this.vertices[vertexIndex] = new Vertex( x,  y, 0,  r,  g,  b,  a,  u,  v);

            //do last
            this.vertexIndex++;
        }
        public Model getLineModel()
        {
            return this.lineModel;
        }
    }
}
