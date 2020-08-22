using Coictus.Models;
using System;

namespace Coictus.SubRendering
{
    /*This class is responsable for batching together multiple modeldrawables, using their indices.
      This allows multiple models with different indices and trinagles to be batched into one draw call.*/
    public static class TriangleBatcher
    {

        public static void combineData(ModelDrawable[] inputModels, out Vertex[] resultVertices, out UInt32[] resultIndices)
        {
            int totalVerticesCount = 0;
            int totalIndicesCount = 0;
            for(int i = 0; i < inputModels.Length; i++)
            {
                if(inputModels[i] != null)
                {
                    totalVerticesCount += inputModels[i].vertices.Length;
                    totalIndicesCount += inputModels[i].indices.Length;
                }
                else
                {
                    Application.warn("TriangleBatcher detected null ModelDrawable at index " + i);
                }
            }

            resultVertices = new Vertex[totalVerticesCount];
            resultIndices = new UInt32[totalIndicesCount];

            //fill resultvertices
            int prevModelVerticesIndex = 0;
            for(int i = 0; i < inputModels.Length; i++)
            {
                if (inputModels[i] != null)
                {
                    for (int j = 0; j < inputModels[i].vertices.Length; j++)
                    {
                        resultVertices[prevModelVerticesIndex + j] = inputModels[i].vertices[j];
                    }
                    prevModelVerticesIndex += inputModels[i].vertices.Length;
                }
            }

            //fill resultIndices
            UInt32 prevModelIndicesIndex = 0;
            UInt32 prevModelsVertexCount = 0;
            for (int i = 0; i < inputModels.Length; i++)
            {
                if (inputModels[i] != null)
                {
                    for (int j = 0; j < inputModels[i].indices.Length; j++)
                    {
                        resultIndices[prevModelIndicesIndex + j] = inputModels[i].indices[j] + prevModelsVertexCount;
                    }
                    prevModelIndicesIndex += (UInt32)inputModels[i].indices.Length;
                    prevModelsVertexCount += (UInt32)inputModels[i].vertices.Length;
                }
            }
        }
    }
}
