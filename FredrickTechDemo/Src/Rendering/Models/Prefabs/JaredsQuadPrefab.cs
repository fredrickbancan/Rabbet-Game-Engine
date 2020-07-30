using System;

namespace FredrickTechDemo.Models
{
    public static class JaredsQuadPrefab
    {
        public static readonly  float[] jaredsQuadVerticesXYZ = new float[]
        {/*   x      y      z   */
            -0.5F, -0.5F, 0.0F,//*vertex 0 bottom left*//*
             0.5F, -0.5F, 0.0F,//*vertex 1 bottom right*//*
            -0.5F,  0.5F, 0.0F,//*vertex 2 top left*//*
             0.5F,  0.5F, 0.0F//*vertex 3 top right*//*
        };

        public static readonly float[] jaredsQuadVerticesRGB = new float[]
        {/*   r      g      b   */
             1.0F, 1.0F, 0.0F,//*vertex 0 bottom left*//*
             0.0F, 1.0F, 0.0F,//*vertex 1 bottom right*//*
             1.0F, 0.0F, 0.0F,//*vertex 2 top left*//*
             0.0F, 0.0F, 1.0F//*vertex 3 top right*//*
        };

        public static readonly float[] jaredsQuadVerticesUV = new float[]
        {/*   u      v   */
            0.0F, 0.0F,//*vertex 0 bottom left*//*
            1.0F, 0.0F,//*vertex 1 bottom right*//*
            0.0F, 1.0F,//*vertex 2 top left*//*
            1.0F, 1.0F//*vertex 3 top right*//*
        };
        public static readonly UInt32[] jaredsQuadIndices = new UInt32[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };

        /*generates a new model using copies of this models arrays.*/
        public static Model getNewModel()
        {
            float[] copyXYZ = new float[jaredsQuadVerticesXYZ.Length];
            float[] copyRGB = new float[jaredsQuadVerticesRGB.Length];
            float[] copyUV = new float[jaredsQuadVerticesUV.Length];
            UInt32[] copyIndices = new uint[jaredsQuadIndices.Length];
            Array.Copy(jaredsQuadVerticesXYZ, copyXYZ, jaredsQuadVerticesXYZ.Length);
            Array.Copy(jaredsQuadVerticesRGB, copyRGB, jaredsQuadVerticesRGB.Length);
            Array.Copy(jaredsQuadVerticesUV, copyUV, jaredsQuadVerticesUV.Length);
            Array.Copy(jaredsQuadIndices, copyIndices, jaredsQuadIndices.Length);
            return new Model(copyXYZ, copyRGB, copyUV, copyIndices);
        }
        public static ModelDrawable getNewModelDrawable()
        {
            float[] copyXYZ = new float[jaredsQuadVerticesXYZ.Length];
            float[] copyRGB = new float[jaredsQuadVerticesRGB.Length];
            float[] copyUV = new float[jaredsQuadVerticesUV.Length];
            UInt32[] copyIndices = new uint[jaredsQuadIndices.Length];
            Array.Copy(jaredsQuadVerticesXYZ, copyXYZ, jaredsQuadVerticesXYZ.Length);
            Array.Copy(jaredsQuadVerticesRGB, copyRGB, jaredsQuadVerticesRGB.Length);
            Array.Copy(jaredsQuadVerticesUV, copyUV, jaredsQuadVerticesUV.Length);
            Array.Copy(jaredsQuadIndices, copyIndices, jaredsQuadIndices.Length);
            return new ModelDrawable(getShaderDir(), getTextureDir(), copyXYZ, copyRGB, copyUV, copyIndices);
        }

        public static String getShaderDir()
        {
            return ResourceHelper.getShaderFileDir("ColourTextureShader3DFog.shader");
        }

        public static String getTextureDir()
        {
            return ResourceHelper.getTextureFileDir("aie.png");
        }
    }
}
