using System;

namespace Coictus.Models
{
    public static class QuadPrefab
    {
        public static readonly Vertex[] quadVertices = new Vertex[]
        {
            new Vertex(-0.5F, -0.5F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F),//0
            new Vertex(0.5F, -0.5F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F),//1
            new Vertex(-0.5F, 0.5F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F),//2
            new Vertex(0.5F, 0.5F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F),//3
        };

        public static readonly uint[] quadIndices = new uint[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };

        /*generates a new model using copies of this models arrays.*/
        public static Model getNewModel()
        {
            Vertex[] verticesCopy = new Vertex[quadVertices.Length];
            Array.Copy(quadVertices, verticesCopy, quadVertices.Length);
            return new Model(verticesCopy);
        }
        public static ModelDrawable getNewModelDrawable()
        {
            Vertex[] verticesCopy = new Vertex[quadVertices.Length];
            uint[] indicesCopy = new uint[quadIndices.Length];
            Array.Copy(quadVertices, verticesCopy, quadVertices.Length);
            Array.Copy(quadIndices, indicesCopy, quadIndices.Length);
            return new ModelDrawable(getShaderDir(), getTextureDir(), verticesCopy, indicesCopy);
        }

        public static string getShaderDir()
        {
            return ResourceUtil.getShaderFileDir("ColorTextureFog3D.shader");
        }

        public static string getTextureDir()
        {
            return ResourceUtil.getTextureFileDir("EntityCactus.png");
        }
    }
}
