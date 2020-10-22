using RabbetGameEngine.Models;

namespace RabbetGameEngine.SubRendering
{

    /*This class will take in multiple Models (Made of quads, containing multiple xyz, rgb, uv and indicies arrays) and combine them into one*
     *Drawable Model for one draw call.*/
    static class QuadCombiner
    {

        /*Batches together models made of quads for being rendered in 3D*/
        public static ModelDrawable combineQuadModels(Model[] quadModels, string shaderName, string textureName)
        {
            Vertex[] newVertices = combineData(quadModels);

            return new ModelDrawable(shaderName, textureName, newVertices, getIndicesForQuadCount(newVertices.Length / 4));
        }

        /*Combines all the data in the model array and outputs the combined ordered arrays.*/
        public static Vertex[] combineData(Model[] modelsToCombine)
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

            Vertex[] newVertices = new Vertex[totalVertexCount];//create new arrays based on total vertex count

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

            return newVertices;
        }

        public static uint[] getIndicesForQuadCount(int quadCount)
        {
            int indexCount = quadCount * 6;
            uint offset = 0;
            uint[] result = new uint[indexCount];
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.
            for (uint i = 0; i < indexCount; i += 6)
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
