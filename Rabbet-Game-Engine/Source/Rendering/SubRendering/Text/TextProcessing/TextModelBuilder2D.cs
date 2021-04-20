using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;
namespace RabbetGameEngine
{
    public static class TextModelBuilder2D
    {
        private static float sizeMult = 0;
        private static readonly byte spaceAscii = 32;

        private static void calSizeMult(bool dpiRelative)
        {
            float sizeMultWidth = MathUtil.normalize(0, GUIManager.guiMapRes.X, GameInstance.gameWindowWidth);
            float sizeMultHeight = MathUtil.normalize(0, GUIManager.guiMapRes.Y, GameInstance.gameWindowHeight);
            sizeMult = dpiRelative ? sizeMultHeight : Math.Min(sizeMultWidth, sizeMultHeight);
        }
        public static Model[] convertStringsToModelsWithColor(List<string> theStrings, List<Color> colors, FontFace font, Vector3 pixelTranslation, float fontSize, ComponentAnchor alignment, bool dpiRelative = true)
        {
            calSizeMult(dpiRelative);
            Model[] result = new Model[theStrings.Count];
            for (int i = 0; i < theStrings.Count; i++)
            {
                result[i] = new Model(convertStringToVertexArray(theStrings[i], font, colors[i].toNormalVec4(), pixelTranslation, fontSize, alignment, i, theStrings.Count - 1 - i), null);
            }
            return result;
        }

        public static Model[] convertStringsToModels(List<string> thestrings, FontFace font, Vector4 color, Vector3 pixelTranslation, float fontSize, ComponentAnchor alignment, bool dpiRelative = true)
        {
            calSizeMult(dpiRelative);
            Model[] result = new Model[thestrings.Count];
            for (int i = 0; i < thestrings.Count; i++)
            {
                result[i] = new Model(convertStringToVertexArray(thestrings[i], font, color, pixelTranslation, fontSize, alignment, i, thestrings.Count - 1 - i), null);
            }
            return result;
        }

        public static Model convertStringToModel(string theString, FontFace font, Vector4 color, Vector3 pixelTranslation, float fontSize, ComponentAnchor alignment, bool dpiRelative = true)
        {
            calSizeMult(dpiRelative);
            Model result = new Model(convertStringToVertexArray(theString, font, color, pixelTranslation, fontSize, alignment, 0), null);
            return result;
        }
        private static Vertex[] convertStringToVertexArray(string thestring, FontFace font, Vector4 color, Vector3 pixelTranslation, float fontSize, ComponentAnchor alignment, int linesAbove = 0, int linesBelow = 0)
        {
            fontSize *= sizeMult;
            Vector3 cursorPos = pixelTranslation;
            float halfLineHeight = font.getLineHeightPixels() * 0.5F * fontSize;
            cursorPos.Y -= linesAbove * (font.getLineHeightPixels() * fontSize) - halfLineHeight;
            byte[] charIds = Encoding.ASCII.GetBytes(thestring);
            Vertex[] result = new Vertex[thestring.Replace(" ", "").Length * 4];//number of vertices for each character. Excluding spaces.
            int vertexIndex = 0;

            switch (alignment)
            {
                case ComponentAnchor.CENTER_RIGHT:
                    {
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
                case ComponentAnchor.CENTER:
                    {
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
                case ComponentAnchor.CENTER_LEFT:
                    {
                        cursorPos.X += 10;
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
                case ComponentAnchor.CENTER_BOTTOM:
                    cursorPos.Y += (linesAbove + linesBelow) * (font.getLineHeightPixels() * fontSize) - halfLineHeight;
                    cursorPos.Y += 10 + linesBelow * (font.getLineHeightPixels() * fontSize) + halfLineHeight;

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
                    break;
                case ComponentAnchor.CENTER_TOP:
                    cursorPos.Y -= 10 + halfLineHeight;

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
                    break;
                case ComponentAnchor.BOTTOM_LEFT:
                    cursorPos.Y += (linesAbove + linesBelow) * (font.getLineHeightPixels() * fontSize) - halfLineHeight;
                    cursorPos.Y += 10 + linesBelow * (font.getLineHeightPixels() * fontSize) + halfLineHeight;
                    cursorPos.X += 10;

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
                    break;
                case ComponentAnchor.BOTTOM_RIGHT:
                    cursorPos.Y += (linesAbove + linesBelow) * (font.getLineHeightPixels() * fontSize) - halfLineHeight;
                    cursorPos.Y += 10 + linesBelow * (font.getLineHeightPixels() * fontSize) + halfLineHeight;
                    cursorPos.X -= 10;

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
                    break;
                case ComponentAnchor.TOP_LEFT:
                    cursorPos.Y -= 10 + halfLineHeight;
                    cursorPos.X += 10;
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
                    break;
                case ComponentAnchor.TOP_RIGHT:
                    cursorPos.Y -= 10 + halfLineHeight;
                    cursorPos.X -= 10;

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
                    break;
            }

            return result;
        }

        private static Vertex[] createVerticesFromChar(Character character, Vector4 color, Vector3 pixelCursorTopLeft, float fontSize)
        {
            //get pixel values
            float x = pixelCursorTopLeft.X + character.getxOffsetPixels() * fontSize;
            float y = pixelCursorTopLeft.Y - character.getyOffsetPixels() * fontSize;
            float xMax = x + character.getPixelWidth() * fontSize;
            float yMax = y - character.getPixelHeight() * fontSize;
            float u = character.getU();
            float v = character.getV();
            float uMax = character.getUMax();
            float vMax = character.getVMax();

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
