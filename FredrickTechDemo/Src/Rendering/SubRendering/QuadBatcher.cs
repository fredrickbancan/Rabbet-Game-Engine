using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo.SubRendering
{

    /*This class will take in multiple Models (Made of quads, containing multiple xyz, rgb, uv and indicies arrays) and combine them into one*
     *Drawable Model for one draw call.*/
    static class QuadBatcher
    {
        public static readonly int maxQuadCount = 196608; // maximum number of quads that can be batched into one call, otherwise a new one must be made.
        public static readonly int maxVertexCount = maxQuadCount * 4;
        public static readonly int maxIndexCount = maxQuadCount * 6;
        private static readonly UInt32[] indices = new UInt32[maxIndexCount];
        private static UInt32 offset = 0;

        static QuadBatcher()
        {
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.
            for (UInt32 i = 0; i < maxIndexCount; i += 6)
            {
                indices[i + 0] = 0 + offset;
                indices[i + 1] = 1 + offset;
                indices[i + 2] = 2 + offset;

                indices[i + 3] = 1 + offset;
                indices[i + 4] = 3 + offset;
                indices[i + 5] = 2 + offset;

                offset += 4;
            }
        }


        /*Batches together models made of quads for being rendered in 3D*/
        public static ModelDrawable batchQuadModels(Model[] quadModels, String shaderFile, String textureFile)
        {
            Vertex[] newVertices;
            combineData(quadModels, out newVertices);

            return new ModelDrawable(shaderFile, textureFile, newVertices, indices);
        }

        /*Combines all the data in the model array and outputs the combined ordered arrays.*/
        private static void combineData(Model[] modelsToCombine, out Vertex[] newVertices)
        {
            int totalVertexCount = 0;
            for (int i = 0; i < modelsToCombine.Length; i++)
            {
                if (modelsToCombine[i] != null)
                {
                    totalVertexCount += modelsToCombine[i].getVertexCount(); //count vertices based on position
                }
                else
                {
                    Application.warn("QuadBatcher detected null model in array at combineData() index: " + i);
                }
            }
            if (totalVertexCount % 4 != 0)
            {
                Application.warn("QuadBatcher attempting to batch an un-even number of vertices!");
            }

            newVertices = new Vertex[totalVertexCount];//create new arrays based on total vertex count

            //itterate over each array and combine their data in new array
            int prevModelVertexIndex = 0;
            for (int i = 0; i < modelsToCombine.Length; i++)
            {
                if (modelsToCombine[i] != null)
                {
                    for(int j = 0; j < modelsToCombine[i].getVertexCount(); j++)
                    {
                        newVertices[prevModelVertexIndex + j] = modelsToCombine[i].getVertexArray()[j];
                    }
                    prevModelVertexIndex += modelsToCombine[i].getVertexCount();

                    if (modelsToCombine[i] is ModelDrawable md)//lastly, delete original model info opengl buffers and programs (if it is an instance of a modeldrawable)
                    {
                        md.delete();
                    }
                }
            }
        }
    }
}
