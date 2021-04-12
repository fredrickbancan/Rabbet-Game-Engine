using RabbetGameEngine;

namespace RabbetGameEngine
{
    /*This class is responsable for batching together multiple modeldrawables, using their indices.
      This allows multiple models with different indices and trinagles to be batched into one draw call.*/
    public static class TriangleCombiner
    {

        public static Model combineData(Model[] inputModels)
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

            Vertex[] resultVertices = new Vertex[totalVerticesCount];
            uint[] resultIndices = new uint[totalIndicesCount];

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
            uint prevModelIndicesIndex = 0;
            uint prevModelsVertexCount = 0;
            for (int i = 0; i < inputModels.Length; i++)
            {
                if (inputModels[i] != null)
                {
                    for (int j = 0; j < inputModels[i].indices.Length; j++)
                    {
                        resultIndices[prevModelIndicesIndex + j] = inputModels[i].indices[j] + prevModelsVertexCount;
                    }
                    prevModelIndicesIndex += (uint)inputModels[i].indices.Length;
                    prevModelsVertexCount += (uint)inputModels[i].vertices.Length;
                }
            }

            return new Model(resultVertices, resultIndices);
        }

        public static Model combineData(Model inputModel, Model newModel)
        {
            int totalVertices = inputModel.vertices.Length + newModel.vertices.Length;
            int totalIndices = inputModel.indices.Length + newModel.indices.Length;

            Vertex[] resultVertices = new Vertex[totalVertices];
            uint[] resultIndices = new uint[totalIndices];

            //fill resultvertices
            for (int i = 0; i < inputModel.vertices.Length; i++)
            {
                resultVertices[i] = inputModel.vertices[i];
            }
            for (int i = inputModel.vertices.Length; i < totalVertices; i++)
            {
                resultVertices[inputModel.vertices.Length + i] = newModel.vertices[i];
            }

            //fill resultIndices
            for (int i = 0; i < inputModel.indices.Length; i++)
            {
                resultIndices[i] = inputModel.indices[i];
            }
            for (int i = inputModel.indices.Length; i < totalIndices; i++)
            {
                resultIndices[inputModel.indices.Length + i] = (uint)(newModel.indices[i] + inputModel.vertices.Length);
            }
            
            inputModel.vertices = resultVertices;
            inputModel.indices = resultIndices;
            return inputModel;
        }
    }
}
