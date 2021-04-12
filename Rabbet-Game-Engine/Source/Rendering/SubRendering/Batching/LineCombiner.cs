using RabbetGameEngine;

namespace RabbetGameEngine
{
    /*Class for Taking in  an even amount of vertices and batches them with indices which allow line rendering.*/
    public static class LineCombiner
    {
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

        /*retuns indices for multiple quads to render them as lined squares*/
        public static uint[] getIndicesForLineQuadCount(int count)
        {
            int indexCount = count * 8; //lines require two verts per line, each quad will need 4 lines, thus 8 indices.
            uint offset = 0;
            uint[] result = new uint[indexCount];
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.  {0,1,1,3,3,2,2,0}
            for (uint i = 0; i < indexCount; i += 8)
            {
                result[i + 0] = 0 + offset;
                result[i + 1] = 1 + offset;
                result[i + 2] = 1 + offset;
                result[i + 3] = 3 + offset;

                result[i + 4] = 3 + offset;
                result[i + 5] = 2 + offset;
                result[i + 6] = 2 + offset;
                result[i + 7] = 0 + offset;

                offset += 4;
            }

            return result;
        }

        public static uint[] getIndicesForLineCount(int count)
        {
            uint[] result = new uint[count * 2];
            for(uint i = 0; i < result.Length; i++)
            {
                result[i] = i;
            }
            return result;
        }
    }
}
