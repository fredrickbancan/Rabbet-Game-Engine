using Coictus.SubRendering;
using OpenTK;
using System;
namespace Coictus.Models
{
    public static class CubePrefab
    {
        public static readonly Vertex[] cubeVertices;

        public static readonly uint[] cubeIndices;

        static CubePrefab()
        {
            Model[] boxSides = new Model[6];

            for (int i = 0; i < boxSides.Length; i++)
            {
                boxSides[i] = QuadPrefab.getNewModel().setColor(CustomColor.magenta);
            }

            boxSides[0].rotateVertices(new Vector3(0, 180, 0)).translateVertices(new Vector3(0, 0, -0.5F));//negZ face
            boxSides[1].translateVertices(new Vector3(0, 0, 0.5F));//posZ face
            boxSides[2].rotateVertices(new Vector3(0, 90, 0)).translateVertices(new Vector3(-0.5F, 0, 0));//negX face
            boxSides[3].rotateVertices(new Vector3(0, -90, 0)).translateVertices(new Vector3(0.5F, 0, 0));//posX face
            boxSides[4].rotateVertices(new Vector3(-90, 0, 0)).translateVertices(new Vector3(0, -0.5F, 0));//negY face
            boxSides[5].rotateVertices(new Vector3(90, 0, 0)).translateVertices(new Vector3(0, 0.5F, 0));//posY face

            QuadBatcher.combineData(boxSides, out cubeVertices);
            cubeIndices = QuadBatcher.getIndicesForQuadCount(6);
        }

        /*generates a new model using copies of this models arrays.*/
        public static Model getNewModel()
        {
            Vertex[] verticesCopy = new Vertex[cubeVertices.Length];
            Array.Copy(cubeVertices, verticesCopy, cubeVertices.Length);
            return new Model(verticesCopy);
        }
        public static ModelDrawable getNewModelDrawable()
        {
            Vertex[] verticesCopy = new Vertex[cubeVertices.Length];
            uint[] indicesCopy = new uint[cubeIndices.Length];
            Array.Copy(cubeVertices, verticesCopy, cubeVertices.Length);
            Array.Copy(cubeIndices, indicesCopy, cubeIndices.Length);
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
