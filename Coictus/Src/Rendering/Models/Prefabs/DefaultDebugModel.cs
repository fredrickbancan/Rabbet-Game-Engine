
using Coictus.SubRendering;
using OpenTK;
using System;
namespace Coictus.Models
{

    //just an ugly cube for when a model fails to load
    public static class DefaultDebugModel
    {
        public static readonly Vertex[] vertices = new Vertex[]
        {   
            //zPos
            new Vertex(-0.5F, -0.5F, 0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 0.5F),//0
            new Vertex(0.5F, -0.5F, 0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 0.5F),//1
            new Vertex(-0.5F, 0.5F, 0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 1F),//2
            new Vertex(0.5F, 0.5F, 0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 1F),//3
            //zNeg
            new Vertex(0.5F, -0.5F, -0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.0F, 0.5F),//0
            new Vertex(-0.5F, -0.5F, -0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 0.5F),//1
            new Vertex(0.5F, 0.5F, -0.5F, 0.8F, 0.8F, 0.8F, 1F,  0.0F, 1F),//2
            new Vertex(-0.5F, 0.5F, -0.5F, 0.8F, 0.8F, 0.8F, 1F, 0.5F, 1F),//3
            //xPos
            new Vertex(0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 0.5F),//0
            new Vertex(0.5F, -0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 0.5F),//1
            new Vertex(0.5F,0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 1F),//2
            new Vertex(0.5F, 0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 1F),//3
            //xNeg
            new Vertex(-0.5F, -0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 0.5F),//0
            new Vertex(-0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 0.5F),//1
            new Vertex(-0.5F,0.5F, -0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.0F, 1F),//2
            new Vertex(-0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1F, 0.5F, 1F),//3
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

        public static readonly UInt32[] indices = QuadBatcher.getIndicesForQuadCount(6);

        /*generates a new model using copies of this models arrays.*/
        public static Model getNewModel()
        {
            Vertex[] verticesCopy = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopy, vertices.Length);
            return new Model(verticesCopy).translateVertices(new Vector3(0, 0.5F, 0));
        }
        public static ModelDrawable getNewModelDrawable()
        {
            Vertex[] verticesCopy = new Vertex[vertices.Length];
            UInt32[] indicesCopy = new uint[indices.Length];
            Array.Copy(vertices, verticesCopy, vertices.Length);
            Array.Copy(indices, indicesCopy, indices.Length);
            return (ModelDrawable)new ModelDrawable(getShaderDir(), new Texture(), verticesCopy, indicesCopy).translateVertices(new Vector3(0, 0.5F, 0));
        }

        public static String getShaderDir()
        {
            return ResourceUtil.getShaderFileDir("ColorTexture3D.shader");
        }

        public static String getTextureDir()
        {
            return "";
        }

    }
}
