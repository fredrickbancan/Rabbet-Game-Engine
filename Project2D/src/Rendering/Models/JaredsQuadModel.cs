using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo.src.Rendering.Models
{
    static class JaredsQuadModel
    {
        public static int jaredsQuadIBO;
        public static int jaredsQuadVBO;
        public static int jaredsQuadVAO;

        public static float[] jaredsQuadVertices = new float[]
        {   //*x*/  /*y*/  /*R*/  /*G*/  /*B*//*
           -0.5F, -0.5F, 1.0F, 0.0F, 0.0F, //*vertex 0 bottom left*//*
            0.5F, -0.5F, 0.0F, 1.0F, 0.0F, //*vertex 1 bottom right*//*
           -0.5F,  0.5F, 0.0F, 0.0F, 1.0F, //*vertex 2 top left*//*
            0.5F,  0.5F, 1.0F, 1.0F, 1.0F  //*vertex 3 top right*//*

        };
        public static UInt32[] jaredsQuadIndices = new UInt32[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };
        public static void genBuffers()
        {
            jaredsQuadVAO = GL.GenVertexArray();
            jaredsQuadIBO = GL.GenBuffer();
            jaredsQuadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, jaredsQuadIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, jaredsQuadIndices.Length * sizeof(UInt32), jaredsQuadIndices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(jaredsQuadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, jaredsQuadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, jaredsQuadVertices.Length * sizeof(float), jaredsQuadVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, true, 5 * sizeof(float), 0);//stride is bytes from start of 1 vertex to the next
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, 5 * sizeof(float), 0);//stride is bytes from start of 1 vertex to the next
            GL.EnableVertexAttribArray(1);
        }

        public static void bindBuffers()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, jaredsQuadIBO);
            GL.BindVertexArray(jaredsQuadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, jaredsQuadVBO);
        }

        public static void draw()
        {
            bindBuffers();
            GL.BindVertexArray(jaredsQuadVAO);
            GL.DrawElements(PrimitiveType.Triangles,jaredsQuadIndices.Length,DrawElementsType.UnsignedInt, 0);
        }
        

    }
}
