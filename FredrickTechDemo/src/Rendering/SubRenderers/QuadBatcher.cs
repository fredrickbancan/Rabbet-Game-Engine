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
        public static readonly int floatCountOfVertex = 8; //number of floats per vertex, including xyz rgb and uv
        private static UInt32[] indices = new UInt32[maxIndexCount];
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

        public static ModelGuiDrawableQuads batchQuadModelsGui(Model[] quadModels, String shaderFile, String textureFile)
        {
            int totalVertexCount = 0;
            for(int i = 0; i < quadModels.Length; i++)
            {
                totalVertexCount += quadModels[i].getVertexCount(); //count vertices based on position
            }
            if(totalVertexCount % 4 != 0)
            {
                Application.warn("QuadBatcher attempting to batch an un-even number of vertices! at batchQuadModelsGui().");
            }

            float[] newVertexXYZ = new float[totalVertexCount * 3];//create new arrays based on total vertex count
            float[] newVertexRGB = new float[totalVertexCount * 3];
            float[] newVertexUV = new float[totalVertexCount * 2];

            //itterate over each array and combine their data in new array
            for (int i = 0; i < quadModels.Length; i++)
            {
                for(int j = 2; j < quadModels[i].getVertexXYZ().Length; j+= 3) //we can itterate over xyz and do rgb at the same time because they have the same count
                {
                    float[] currentXYZ = quadModels[i].getVertexXYZ();
                    float[] currentRGB = quadModels[i].getVertexRGB();
                    float x = currentXYZ[j - 2];
                    float y = currentXYZ[j - 1];
                    float z = currentXYZ[j    ];
                    float r = currentRGB[j - 2];
                    float g = currentRGB[j - 1];
                    float b = currentRGB[j    ];
                    newVertexXYZ[j - 2] = x;
                    newVertexXYZ[j - 1] = y;
                    newVertexXYZ[j    ] = z;
                    newVertexRGB[j - 2] = r;
                    newVertexRGB[j - 1] = g;
                    newVertexRGB[j    ] = b;
                }

                for(int j = 1; j < quadModels[i].getVertexUV().Length; j+= 2)// do same for UV
                {
                    float[] currentUV = quadModels[i].getVertexUV();
                    float u = currentUV[j - 1];
                    float v = currentUV[j    ];
                    newVertexUV[j - 1] = u;
                    newVertexUV[j    ] = v;
                }
            }

            return new ModelGuiDrawableQuads(shaderFile, textureFile, newVertexXYZ, newVertexRGB, newVertexUV, indices);
        }
    }
}
