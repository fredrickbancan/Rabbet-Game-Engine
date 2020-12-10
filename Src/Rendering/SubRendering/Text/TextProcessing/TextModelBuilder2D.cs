using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System.Text;
namespace RabbetGameEngine.Text
{
    public static class TextModelBuilder2D
    {
        private static readonly byte spaceAscii = 32;
        public static Model[] convertstringArrayToModelArray(string[] thestrings, FontFace font, Vector4 color, Vector3 pixelTranslation, float fontSize, ComponentAlignment alignment)
        {
          
            Model[] result = new Model[thestrings.Length];
            for(uint i = 0; i < thestrings.Length; i++)
            {
                result[i] = new Model(convertstringToVertexArray(thestrings[i], font, color, pixelTranslation, fontSize, alignment, i), null);
            }
            return result;
        }
        public static Vertex[] convertstringToVertexArray(string thestring, FontFace font, Vector4 color, Vector3 pixelTranslation, float fontSize, ComponentAlignment alignment, uint previousLineCount = 0)
        {
            float sizeMult = MathUtil.normalize(0, 1080, GameInstance.gameWindowHeight);
            fontSize *= sizeMult;
            Vector3 cursorPos = pixelTranslation; 
            cursorPos.Y -= previousLineCount * (font.getLineHeightPixels() * fontSize) - font.getLineHeightPixels() * 0.5F * fontSize;
            byte[] charIds = Encoding.ASCII.GetBytes(thestring);
            Vertex[] result = new Vertex[thestring.Replace(" ", "").Length * 4];//number of vertices for each character. Excluding spaces.
            int vertexIndex = 0;

            switch (alignment)
            {
                case ComponentAlignment.RIGHT:
                    {
                        //position cursor to right side and offset cursor to compensate for string length
                        cursorPos.X = GameInstance.gameWindowWidth - cursorPos.X - 10;//put cursor to right side orientation
                        for (int i = 0; i < thestring.Length; i++)
                        {
                            if (charIds[i] == spaceAscii)
                            {
                                cursorPos.X -= font.getSpaceWidthPixels() * fontSize;
                            }
                            else
                            {
                                cursorPos.X -= font.getCharacter(charIds[i]).getXAdvancePixels() * fontSize;
                            }
                        }

                        for (int i = 0; i < thestring.Length; i++)
                        {
                            if (charIds[i] == spaceAscii)
                            {
                                cursorPos.X += font.getSpaceWidthPixels() * fontSize;
                            }
                            else
                            {
                                Character currentChar = font.getCharacter(charIds[i]);
                                Vertex[] characterVertices = createVerticesFromChar(currentChar, color, cursorPos, fontSize);
                                result[vertexIndex + 0] = characterVertices[0];
                                result[vertexIndex + 1] = characterVertices[1];
                                result[vertexIndex + 2] = characterVertices[2];
                                result[vertexIndex + 3] = characterVertices[3];
                                vertexIndex += 4;
                                cursorPos.X += currentChar.getXAdvancePixels() * fontSize;
                            }
                        }
                    }
                    break;
                case ComponentAlignment.CENTER:
                    {
                        //offset cursor to compensate for string length so center of string is at origin provided
                        for (int i = 0; i < thestring.Length; i++)
                        {
                            if (charIds[i] == spaceAscii)
                            {
                                cursorPos.X -= font.getSpaceWidthPixels() / 2 * fontSize;
                            }
                            else
                            {
                                cursorPos.X -= font.getCharacter(charIds[i]).getXAdvancePixels() / 2 * fontSize;
                            }
                        }

                        for (int i = 0; i < thestring.Length; i++)
                        {
                            if (charIds[i] == spaceAscii)
                            {
                                cursorPos.X += font.getSpaceWidthPixels() * fontSize;
                            }
                            else
                            {
                                Character currentChar = font.getCharacter(charIds[i]);
                                Vertex[] characterVertices = createVerticesFromChar(currentChar, color, cursorPos, fontSize);
                                result[vertexIndex + 0] = characterVertices[0];
                                result[vertexIndex + 1] = characterVertices[1];
                                result[vertexIndex + 2] = characterVertices[2];
                                result[vertexIndex + 3] = characterVertices[3];
                                vertexIndex += 4;
                                cursorPos.X += currentChar.getXAdvancePixels() * fontSize;
                            }
                        }
                    }
                    break;
                case ComponentAlignment.LEFT:
                    {
                        for (int i = 0; i < thestring.Length; i++)
                        {
                            if (charIds[i] == spaceAscii)
                            {
                                cursorPos.X += font.getSpaceWidthPixels() * fontSize;
                            }
                            else
                            {
                                Character currentChar = font.getCharacter(charIds[i]);
                                Vertex[] characterVertices = createVerticesFromChar(currentChar, color, cursorPos, fontSize);
                                result[vertexIndex + 0] = characterVertices[0];
                                result[vertexIndex + 1] = characterVertices[1];
                                result[vertexIndex + 2] = characterVertices[2];
                                result[vertexIndex + 3] = characterVertices[3];
                                vertexIndex += 4;
                                cursorPos.X += currentChar.getXAdvancePixels() * fontSize;
                            }
                        }
                    }
                    break;
            }

            return result;
        }
       
        public static Vertex[] createVerticesFromChar(Character character, Vector4 color, Vector3 pixelCursorTopLeft, float fontSize)
        {
            //get pixel values
            float x = pixelCursorTopLeft.X + character.getxOffsetPixels() * fontSize;
            float y = pixelCursorTopLeft.Y - character.getyOffsetPixels() * fontSize;
            float xMax = x + character.getPixelWidth() * fontSize;
            float yMax = y - character.getPixelHeight() * fontSize;
            float u = character.getU() + 0.00001F;
            float v = character.getV() + 0.00001F;
            float uMax = character.getUMax() - 0.00001F;
            float vMax = character.getVMax() - 0.00001F;

            Vertex[] modelVertices = new Vertex[4];

            //add vertices at screen coords with uv, in different order so the quads will now face fowards after being flipped, so that they show when face culling.
            modelVertices[1] = new Vertex(x, y, pixelCursorTopLeft.Z, color.X, color.Y, color.Z, color.W, u, v);//Bottom left vertex0
            modelVertices[0] = new Vertex(xMax, y, pixelCursorTopLeft.Z, color.X, color.Y, color.Z, color.W, uMax, v);//bottom right vertex1
            modelVertices[3] = new Vertex(x, yMax, pixelCursorTopLeft.Z, color.X, color.Y, color.Z, color.W, u, vMax);//top left vertex2
            modelVertices[2] = new Vertex(xMax, yMax, pixelCursorTopLeft.Z, color.X, color.Y, color.Z, color.W, uMax, vMax);//top right vertex3

            return modelVertices;
        }
    }
}
