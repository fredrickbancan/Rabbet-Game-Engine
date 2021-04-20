﻿using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
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

        public static readonly Vector2[] quadVertexPositions2D = new Vector2[]
        {
            new Vector2(-0.5F, -0.5F),
            new Vector2(0.5F, -0.5F),
            new Vector2(-0.5F, 0.5F),
            new Vector2(0.5F, 0.5F)
        };
        public static readonly uint[] quadIndices = new uint[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
         {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
         };

        /*generates a new model using copies of this models arrays.*/
        public static Model copyModel()
        {
            Vertex[] verticesCopy = new Vertex[quadVertices.Length];
            uint[] indicesCopy = new uint[quadIndices.Length];
            Array.Copy(quadIndices, indicesCopy, quadIndices.Length);
            Array.Copy(quadVertices, verticesCopy, quadVertices.Length);
            return new Model(verticesCopy, indicesCopy);
        }
    }
}
