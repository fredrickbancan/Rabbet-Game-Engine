using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FredrickTechDemo.SubRendering.Text
{
    /*this class is responsable for building arrays of veritces for text on the screen.*/
    public static class TextModelBuilder2D
    {
        private static readonly byte spaceAscii = 32;
        

        /*Takes in an array of strings and converts them into an array of models. Each string in the array is treated as a new line of text.
          The lines start from the provided topleftorigin vector. Each model in the array is a line of text.*/
        public static Model[] convertStringArrayToModelArray(String[] theStrings, Font font, Vector4F color, Vector2F topLeftPixelOrigin, float fontSize, int screenEdgePadding, TextAlign alignment)
        {
            Model[] result = new Model[theStrings.Length];
            for(UInt32 i = 0; i < theStrings.Length; i++)
            {
                result[i] = new Model(convertStringToVertexArray(theStrings[i], font, color, topLeftPixelOrigin, fontSize, screenEdgePadding, alignment, i));
            }
            return result;
        }
        public static Vertex[] convertStringToVertexArray(String theString, Font font, Vector4F color, Vector2F topLeftPixelOrigin, float fontSize, int screenEdgePadding, TextAlign alignment, UInt32 previousLineCount = 0)
        {
            Vector2F cursorPos = topLeftPixelOrigin;
            cursorPos.x += screenEdgePadding;
            cursorPos.y += previousLineCount * (font.getLineHeightPixels() * fontSize);
            byte[] charIds = Encoding.ASCII.GetBytes(theString);
            Vertex[] result = new Vertex[theString.Replace(" ", "").Length * 4];//number of vertices for each character. Excluding spaces.
            int vertexIndex = 0;

            if (alignment == TextAlign.RIGHT)
            {
                //position cursor to right side and offset cursor to compensate for string length
                cursorPos.x = GameInstance.gameWindowWidth - cursorPos.x;//put cursor to right side orientation
                for (int i = 0; i < theString.Length; i++)
                {
                    if (charIds[i] == spaceAscii)
                    {
                        cursorPos.x -= font.getSpaceWidthPixels() * fontSize;
                    }
                    else
                    {
                        cursorPos.x -= font.getCharacter(charIds[i]).getXAdvancePixels() * fontSize;
                    }
                }

                for (int i = 0; i < theString.Length; i++)
                {
                    if (charIds[i] == spaceAscii)
                    {
                        cursorPos.x += font.getSpaceWidthPixels() * fontSize;
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
                        cursorPos.x += currentChar.getXAdvancePixels() * fontSize;
                    }
                }
            }
            else if (alignment == TextAlign.CENTER)
            {
                cursorPos.x -= screenEdgePadding;//remove padding if text is center alignment since it shouldnt be near edges
                //offset cursor to compensate for string length so center of string is at origin provided
                for (int i = 0; i < theString.Length; i++)
                {
                    if (charIds[i] == spaceAscii)
                    {
                        cursorPos.x -= font.getSpaceWidthPixels() / 2 * fontSize;
                    }
                    else
                    {
                        cursorPos.x -= font.getCharacter(charIds[i]).getXAdvancePixels() / 2 * fontSize;
                    }
                }

                for (int i = 0; i < theString.Length; i++)
                {
                    if (charIds[i] == spaceAscii)
                    {
                        cursorPos.x += font.getSpaceWidthPixels() * fontSize;
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
                        cursorPos.x += currentChar.getXAdvancePixels() * fontSize;
                    }
                }
            }
            else//default is left hand alignment
            {
                for (int i = 0; i < theString.Length; i++)
                {
                    if (charIds[i] == spaceAscii)
                    {
                        cursorPos.x += font.getSpaceWidthPixels() * fontSize;
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
                        cursorPos.x += currentChar.getXAdvancePixels() * fontSize;
                    }
                }
            }
            return result;
        }
       
        public static Vertex[] createVerticesFromChar(Character character, Vector4F color, Vector2F pixelCursorTopLeft, float fontSize)
        {
            //get pixel values
            float x = pixelCursorTopLeft.x + character.getxOffsetPixels() * fontSize;
            float y = pixelCursorTopLeft.y + character.getyOffsetPixels() * fontSize;
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

            Vertex[] modelVertices = new Vertex[4];

            //add vertices at screen coords with uv, in different order so the quads will now face fowards after being flipped, so that they show when face culling.
            //color additions and multiplications are for creating the gradient and highlight for visibility
            modelVertices[0] = new Vertex(xMax, y, 0, color.r + 0.1F, color.g + 0.1F, color.b + 0.1F, color.a, uMax, v);//bottom right vertex1
            modelVertices[1] = new Vertex(x, y, 0, color.r + 0.1F, color.g + 0.1F, color.b + 0.1F, color.a, u, v);//Bottom left vertex0
            modelVertices[2] = new Vertex(xMax, yMax, 0, color.r * 0.5F, color.g * 0.5F, color.b * 0.5F, color.a, uMax, vMax);//top right vertex3
            modelVertices[3] = new Vertex(x, yMax, 0, color.r * 0.5F, color.g * 0.5F, color.b * 0.5F, color.a, u, vMax);//top left vertex2

            return modelVertices;
        }


        /*Combines all the vertex data of all of the text panels, and sends the information to the dynamic model.*/
        public static void batchAndSubmitTextToDynamicModel(ModelDrawableDynamic dynamicModel, Dictionary<String, GUITextPanel> textPanels, UInt32 maxCharCount)
        {
            Vertex[] fillerVertexArray = new Vertex[maxCharCount * 4];//creating array big enough to fill max char count
            if (textPanels.Count > 0)
            {
                //combine all models
                Model[] combinedModels = null;

                int totalModelCount = 0;

                foreach (GUITextPanel panel in textPanels.Values)
                {
                    totalModelCount += panel.models.Length;
                }

                combinedModels = new Model[totalModelCount];

                int modelIndex = 0;
                foreach (GUITextPanel panel in textPanels.Values)
                {
                    for (int i = 0; i < panel.models.Length; i++)
                    {
                        combinedModels[modelIndex + i] = panel.models[i];
                    }
                    modelIndex += panel.models.Length;
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
                dynamicModel.submitData(fillerVertexArray);
            }
            else
            {
                dynamicModel.submitData(fillerVertexArray);//just fill buffer with empties to clear screen of text
            }
        }
    }
}
