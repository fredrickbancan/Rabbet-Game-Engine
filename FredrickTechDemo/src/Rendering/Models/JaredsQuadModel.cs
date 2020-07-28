using System;

namespace FredrickTechDemo.Models
{
    public static class JaredsQuadModelData
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

        
    }
}
