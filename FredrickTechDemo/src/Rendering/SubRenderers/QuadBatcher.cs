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
        private static readonly byte vertexXYZComponentCount = 3;//number of floats per XYZ component of vertex
        private static readonly byte vertexRGBAComponentCount = 3;//number of floats per RGBA component of vertex
        private static readonly byte vertexUVComponentCount = 2;//number of floats per UV component of vertex
        public static readonly byte floatCountOfVertex = 8; //number of floats per vertex, including xyz rgb and uv
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

        public static ModelDrawableGUI batchQuadModelsGui(Model[] quadModels, String shaderFile, String textureFile)
        {
            float[] newVertexXYZ;
            float[] newVertexRGBA;
            float[] newVertexUV;
            combineData(quadModels, out newVertexXYZ, out newVertexRGBA, out newVertexUV);

            return new ModelDrawableGUI(shaderFile, textureFile, newVertexXYZ, newVertexRGBA, newVertexUV, indices);
        }

        public static ModelDrawable batchQuadModels3D(Model[] quadModels, String shaderFile, String textureFile)
        {
            float[] newVertexXYZ;
            float[] newVertexRGBA;
            float[] newVertexUV;
            combineData(quadModels, out newVertexXYZ, out newVertexRGBA, out newVertexUV);

            return new ModelDrawable(shaderFile, textureFile, newVertexXYZ, newVertexRGBA, newVertexUV, indices);
        }

        private static void combineData(Model[] modelsToCombine, out float[] newVertexXYZ, out float[] newVertexRGBA, out float[] newVertexUV)
        {
            int totalVertexCount = 0;
            for (int i = 0; i < modelsToCombine.Length; i++)
            {
                totalVertexCount += modelsToCombine[i].getVertexCount(); //count vertices based on position
            }
            if (totalVertexCount % 4 != 0)
            {
                Application.warn("QuadBatcher attempting to batch an un-even number of vertices! at combineData().");
            }

            newVertexXYZ = new float[totalVertexCount * vertexXYZComponentCount];//create new arrays based on total vertex count
            newVertexRGBA = new float[totalVertexCount * vertexRGBAComponentCount];
            newVertexUV = new float[totalVertexCount * vertexUVComponentCount];

            //itterate over each array and combine their data in new array
            int prevModelXYZindex = 0;
            int prevModelRGBAIndex = 0;
            int prevModelUVIndex = 0;
            for (int i = 0; i < modelsToCombine.Length; i++)
            {
                for (int j = vertexXYZComponentCount - 1; j < modelsToCombine[i].getVertexXYZ().Length; j += vertexXYZComponentCount)//combine xyz data
                {
                    float[] currentXYZ = modelsToCombine[i].getVertexXYZ();
                    float x = currentXYZ[j - 2];
                    float y = currentXYZ[j - 1];
                    float z = currentXYZ[j];
                    newVertexXYZ[prevModelXYZindex + (j - 2)] = x;
                    newVertexXYZ[prevModelXYZindex + (j - 1)] = y;
                    newVertexXYZ[prevModelXYZindex + j] = z;
                }
                for (int j = vertexRGBAComponentCount - 1; j < modelsToCombine[i].getVertexXYZ().Length; j += vertexRGBAComponentCount)//combine rgb data
                {
                    float[] currentRGBA = modelsToCombine[i].getVertexRGBA();
                    float r = currentRGBA[j - 2];
                    float g = currentRGBA[j - 1];
                    float b = currentRGBA[j];
                    newVertexRGBA[prevModelRGBAIndex + (j - 2)] = r;
                    newVertexRGBA[prevModelRGBAIndex + (j - 1)] = g;
                    newVertexRGBA[prevModelRGBAIndex + j] = b;
                }

                for (int j = vertexUVComponentCount - 1; j < modelsToCombine[i].getVertexUV().Length; j += vertexUVComponentCount)// do same for UV
                {
                    float[] currentUV = modelsToCombine[i].getVertexUV();
                    float u = currentUV[j - 1];
                    float v = currentUV[j];
                    newVertexUV[prevModelUVIndex + (j - 1)] = u;
                    newVertexUV[prevModelUVIndex + j] = v;
                }

                prevModelXYZindex += modelsToCombine[i].getVertexCount() * vertexXYZComponentCount;//increase the respective index's 
                prevModelRGBAIndex += modelsToCombine[i].getVertexCount() * vertexRGBAComponentCount;
                prevModelUVIndex += modelsToCombine[i].getVertexCount() * vertexUVComponentCount;

                if (modelsToCombine[i] is ModelDrawable md)//lastly, delete original model info opengl buffers and programs (if it is an instance of a modeldrawable)
                {
                    md.delete();
                }
            }
        }
    }
}
