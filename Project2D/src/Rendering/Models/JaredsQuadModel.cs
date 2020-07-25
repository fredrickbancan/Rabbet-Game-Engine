using FredsMath;
using OpenTK;
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
        public static Matrix4 modelMatrix;
        public static Matrix4 rotationMatrix;
        public static Texture quadTexture;

        public static float[] jaredsQuadVertices = new float[]
        {  /*x      y     z     r     g     b     u     v */
           -0.5F, -0.5F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, //*vertex 0 bottom left*//*
            0.5F, -0.5F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, //*vertex 1 bottom right*//*
           -0.5F,  0.5F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, //*vertex 2 top left*//*
            0.5F,  0.5F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F  //*vertex 3 top right*//*

        };
        public static UInt32[] jaredsQuadIndices = new UInt32[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };

        public static void init()
        {
            modelMatrix = Matrix4.Identity;
            rotationMatrix = Matrix4.CreateRotationY((float)MathUtil.radians(5.0D));
            quadTexture = new Texture(@"..\..\res\aie.png", true);
            genBuffers();
        }
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
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);//for X,y,z coords
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));//for rgb
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, 8 * sizeof(float), 6 * sizeof(float));//for uv
            GL.EnableVertexAttribArray(2);
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
            quadTexture.use();
            GL.DrawElements(PrimitiveType.Triangles,jaredsQuadIndices.Length,DrawElementsType.UnsignedInt, 0);
        }
        
        public static void rotateQuad()
        {
            modelMatrix *= rotationMatrix;
        }

    }
}
