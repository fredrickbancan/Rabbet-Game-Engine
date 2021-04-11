using OpenTK.Mathematics;
using RabbetGameEngine;
using System.Text;
namespace RabbetGameEngine.Text
{
    public static class TextModelBuilder3D
    {
        private static readonly byte spaceAscii = 32;
        

        /*Takes in an array of strings and converts them into an array of models. Each string in the array is treated as a new line of text.
          The lines start from the provided topleftorigin vector. Each model in the array is a line of text.*/
        public static Model convertStringToModel(string thestring, FontFace font, Vector4 color)
        {
            return new Model(convertStringToVertexArray(thestring, font, color), null);
        }
        public static Vertex[] convertStringToVertexArray(string thestring, FontFace font, Vector4 color)
        {
            Vector2 cursorPos = new Vector2(0, -font.getLineHeightPixels() / 2);
            byte[] charIds = Encoding.ASCII.GetBytes(thestring);
            Vertex[] result = new Vertex[thestring.Replace(" ", "").Length * 4];//number of vertices for each character. Excluding spaces.
            int vertexIndex = 0;

            //offset cursor to compensate for string length so center of string is at origin provided
            for (int i = 0; i < thestring.Length; i++)
            {
                if (charIds[i] == spaceAscii)
                {
                    cursorPos.X -= font.getSpaceWidthPixels() / 2;
                }
                else
                {
                    cursorPos.X -= font.getCharacter(charIds[i]).getXAdvancePixels() / 2;
                }
            }

            for (int i = 0; i < thestring.Length; i++)
            {
                if (charIds[i] == spaceAscii)
                {
                    cursorPos.X += font.getSpaceWidthPixels();
                }
                else
                {
                    Character currentChar = font.getCharacter(charIds[i]);
                    Vertex[] characterVertices = createVerticesFromChar(currentChar, color, cursorPos);
                    result[vertexIndex + 0] = characterVertices[0];
                    result[vertexIndex + 1] = characterVertices[1];
                    result[vertexIndex + 2] = characterVertices[2];
                    result[vertexIndex + 3] = characterVertices[3];
                    vertexIndex += 4;
                    cursorPos.X += currentChar.getXAdvancePixels();
                }
            }


            return result;
        }
       
        public static Vertex[] createVerticesFromChar(Character character, Vector4 color,Vector2 pixelCursorTopLeft)
        {
            //get pixel values
            float x = pixelCursorTopLeft.X + character.getxOffsetPixels();
            float y = pixelCursorTopLeft.Y + character.getyOffsetPixels();
            float xMax = x + character.getPixelWidth();
            float yMax = y + character.getPixelHeight();
            float u = character.getU();
            float v = character.getV();
            float uMax = character.getUMax();
            float vMax = character.getVMax();

            //convert pixel values to vertices in screen coords
            x = MathUtil.normalizeCustom(0, 1, 0, 1920, x);
            y = MathUtil.normalizeCustom(0, 1, 0, 1920, y);
            xMax = MathUtil.normalizeCustom(0, 1, 0, 1920, xMax);
            yMax = MathUtil.normalizeCustom(0, 1, 0, 1920, yMax);

            //so far we have been treating this system like the top left corner of the screen is smallest
            //so now at the end we flip it
            //flip y 
            y = -y;
            yMax = -yMax;

            Vertex[] modelVertices = new Vertex[4];

            //add vertices at screen coords with uv, in different order so the quads will now face fowards after being flipped, so that they show when face culling.
            modelVertices[0] = new Vertex(xMax, y, 0, color.X, color.Y, color.Z, color.W, uMax, v);//bottom right vertex1
            modelVertices[1] = new Vertex(x, y, 0, color.X, color.Y, color.Z, color.W, u, v);//Bottom left vertex0
            modelVertices[2] = new Vertex(xMax, yMax, 0, color.X, color.Y, color.Z, color.W, uMax, vMax);//top right vertex3
            modelVertices[3] = new Vertex(x, yMax, 0, color.X, color.Y, color.Z, color.W, u, vMax);//top left vertex2

            return modelVertices;
        }
    }
}
