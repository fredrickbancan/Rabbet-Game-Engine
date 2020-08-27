using System;

namespace Coictus.SubRendering
{
    /*Class for Taking in  an even amount of vertices and batches them with indices which allow line rendering.*/
    public static class LineBatcher
    {

        /*retuns indices for multiple quads to render them as lined squares*/
        public static UInt32[] getIndicesForLineQuadCount(int count)
        {
            int indexCount = count * 8; //lines require two vertices per line, each quad will need 4 lines, thus 8 vertices.
            UInt32 offset = 0;
            UInt32[] result = new UInt32[indexCount];
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.  {0,1,1,3,3,2,2,0}
            for (UInt32 i = 0; i < indexCount; i += 8)
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
    }
}
