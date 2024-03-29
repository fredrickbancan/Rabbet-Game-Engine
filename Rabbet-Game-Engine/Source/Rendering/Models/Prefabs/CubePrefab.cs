﻿using OpenTK.Mathematics;
using System;
namespace RabbetGameEngine
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
                boxSides[i] = QuadPrefab.copyModel();
            }

            boxSides[0].rotateVertices(new Vector3(0, 180, 0)).translateVertices(new Vector3(0, 0, -0.5F));//negZ face
            boxSides[1].translateVertices(new Vector3(0, 0, 0.5F));//posZ face
            boxSides[2].rotateVertices(new Vector3(0, 90, 0)).translateVertices(new Vector3(-0.5F, 0, 0));//negX face
            boxSides[3].rotateVertices(new Vector3(0, -90, 0)).translateVertices(new Vector3(0.5F, 0, 0));//posX face
            boxSides[4].rotateVertices(new Vector3(-90, 0, 0)).translateVertices(new Vector3(0, -0.5F, 0));//negY face
            boxSides[5].rotateVertices(new Vector3(90, 0, 0)).translateVertices(new Vector3(0, 0.5F, 0));//posY face

            cubeVertices = QuadCombiner.combineData(boxSides);
            cubeIndices = QuadCombiner.getIndicesForQuadCount(6);
        }

        /*generates a new model using copies of this models arrays.*/
        public static Model copyModel()
        {
            Vertex[] verticesCopy = new Vertex[cubeVertices.Length];
            uint[] indicesCopy = new uint[cubeIndices.Length];
            Array.Copy(cubeIndices, indicesCopy, cubeIndices.Length);
            Array.Copy(cubeVertices, verticesCopy, cubeVertices.Length);
            return new Model(verticesCopy, indicesCopy);
        }

    }
}
