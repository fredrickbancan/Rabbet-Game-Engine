using OpenTK;
using RabbetGameEngine.SubRendering;
using System;
namespace RabbetGameEngine.Models
{
    public static class CubePrefab
    {
        public static readonly string shaderName = "EntityWorld_F";
        public static readonly string textureName = "Explosion";

        public static readonly Vertex[] cubeVertices;

        public static readonly uint[] cubeIndices;

        static CubePrefab()
        {
            Model[] boxSides = new Model[6];

            for (int i = 0; i < boxSides.Length; i++)
            {
                boxSides[i] = QuadPrefab.getNewModel();
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
        public static Model copyModel()
        {
            Vertex[] verticesCopy = new Vertex[cubeVertices.Length];
            Array.Copy(cubeVertices, verticesCopy, cubeVertices.Length);
            return new Model(verticesCopy);
        }
        public static ModelDrawable copyModelDrawable()
        {
            Vertex[] verticesCopy = new Vertex[cubeVertices.Length];
            uint[] indicesCopy = new uint[cubeIndices.Length];
            Array.Copy(cubeVertices, verticesCopy, cubeVertices.Length);
            Array.Copy(cubeIndices, indicesCopy, cubeIndices.Length);
            return new ModelDrawable(shaderName, textureName, verticesCopy, indicesCopy);
        }

    }
}
