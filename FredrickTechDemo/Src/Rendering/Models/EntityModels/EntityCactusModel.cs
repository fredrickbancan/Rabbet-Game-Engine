using FredrickTechDemo.FredsMath;
using FredrickTechDemo.SubRendering;
using System;

namespace FredrickTechDemo.Models
{
    static class EntityCactusModel
    {
        public static readonly Vertex[] cactusVertices = new Vertex[]
        {   
            //zPos
            new Vertex(-0.5F, -0.5F, 0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 0.5F),//0
            new Vertex(0.5F, -0.5F, 0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 0.5F),//1
            new Vertex(-0.5F, 0.5F, 0.43755F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 1F),//2
            new Vertex(0.5F, 0.5F, 0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 1F),//3
            //zNeg
            new Vertex(0.5F, -0.5F, -0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 0.5F),//0
            new Vertex(-0.5F, -0.5F, -0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 0.5F),//1
            new Vertex(0.5F, 0.5F, -0.4375F, 0.8F, 0.8F, 0.8F, 1F,  0.0F, 1F),//2
            new Vertex(-0.5F, 0.5F, -0.4375F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 1F),//3
            //xPos
            new Vertex(0.4375F, -0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 0.5F),//0
            new Vertex(0.4375F, -0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 0.5F),//1
            new Vertex(0.4375F,0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 1F),//2
            new Vertex(0.4375F, 0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 1F),//3
            //xNeg
            new Vertex(-0.4375F, -0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 0.5F),//0
            new Vertex(-0.4375F, -0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 0.5F),//1
            new Vertex(-0.4375F,0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 1F),//2
            new Vertex(-0.4375F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 1F),//3
            //yPos
            new Vertex(0.5F, 0.5F, -0.5F, 1.0F, 1.0F, 1.0F, 1.0F, 0.5F, 0.5F),//0
            new Vertex(-0.5F, 0.5F, -0.5F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.5F),//1
            new Vertex(0.5F, 0.5F, 0.5F, 1.0F, 1.0F, 1.0F, 1.0F, 0.5F, 1F),//2
            new Vertex(-0.5F, 0.5F, 0.5F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1F),//3
            //yNeg
            new Vertex(-0.5F, -0.5F, -0.5F, 0.4F, 0.4F, 0.4F, 1F, 0.0F, 0F),//0
            new Vertex(0.5F, -0.5F, -0.5F, 0.4F, 0.4F, 0.4F, 1F, 0.5F, 0F),//1
            new Vertex(-0.5F, -0.5F, 0.5F, 0.4F, 0.4F, 0.4F, 1F, 0.0F, 0.5F),//2
            new Vertex(0.5F, -0.5F, 0.5F, 0.4F, 0.4F, 0.4F, 1F, 0.5F, 0.5F)//3
        };

        public static readonly UInt32[] cactusIndices = QuadBatcher.getIndicesForQuadCount(6);

        /*generates a new model using copies of this models arrays.*/
        public static Model getNewModel()
        {
            Vertex[] verticesCopy = new Vertex[cactusVertices.Length];
            Array.Copy(cactusVertices, verticesCopy, cactusVertices.Length);
            return new Model(verticesCopy).translateVertices(new Vector3F(0,0.5F,0));
        }
        public static ModelDrawable getNewModelDrawable()
        {
            Vertex[] verticesCopy = new Vertex[cactusVertices.Length];
            UInt32[] indicesCopy = new uint[cactusIndices.Length];
            Array.Copy(cactusVertices, verticesCopy, cactusVertices.Length);
            Array.Copy(cactusIndices, indicesCopy, cactusIndices.Length);
            return (ModelDrawable) new ModelDrawable(getShaderDir(), getTextureDir(), verticesCopy, indicesCopy).translateVertices(new Vector3F(0, 0.5F, 0));
        }

        public static String getShaderDir()
        {
            return ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        }

        public static String getTextureDir()
        {
            return ResourceHelper.getTextureFileDir("EntityCactus.png");
        }
    }
}
