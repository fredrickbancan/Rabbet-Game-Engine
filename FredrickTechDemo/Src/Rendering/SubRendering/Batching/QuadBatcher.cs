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
        private static readonly UInt32[] indices = getIndicesForQuadCount(maxQuadCount);


        /*Batches together models made of quads for being rendered in 3D*/
        public static ModelDrawable batchQuadModels(Model[] quadModels, String shaderFile, String textureFile)
        {
            Vertex[] newVertices;
            combineData(quadModels, out newVertices);

            return new ModelDrawable(shaderFile, textureFile, newVertices, indices);
        }

        /*Combines all the data in the model array and outputs the combined ordered arrays.*/
        public static void combineData(Model[] modelsToCombine, out Vertex[] newVertices)
        {
            int totalVertexCount = 0;
            for (int i = 0; i < modelsToCombine.Length; i++)
            {
                if (modelsToCombine[i] != null)
                {
                    totalVertexCount += modelsToCombine[i].vertices.Length; //count vertices based on position
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
                    for(int j = 0; j < modelsToCombine[i].vertices.Length; j++)
                    {
                        newVertices[prevModelVertexIndex + j] = modelsToCombine[i].vertices[j];
                    }
                    prevModelVertexIndex += modelsToCombine[i].vertices.Length;
                }
            }
        }

        public static UInt32[] getIndicesForQuadCount(int quadCount)
        {
            int indexCount = quadCount * 6;
            UInt32 offset = 0;
            UInt32[] result = new UInt32[indexCount];
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.
            for (UInt32 i = 0; i < indexCount; i += 6)
            {
                result[i + 0] = 0 + offset;
                result[i + 1] = 1 + offset;
                result[i + 2] = 2 + offset;

                result[i + 3] = 1 + offset;
                result[i + 4] = 3 + offset;
                result[i + 5] = 2 + offset;

                offset += 4;
            }

            return result;
        }
    }
}
