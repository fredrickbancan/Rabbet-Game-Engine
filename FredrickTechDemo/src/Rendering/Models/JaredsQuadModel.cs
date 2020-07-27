using FredrickTechDemo.FredsMath;
using OpenTK.Graphics.OpenGL;
using System;

namespace FredrickTechDemo.Model
{
    static class JaredsQuadModel
    {
        private static bool hasInitialized = false;
        private static int jaredsQuadIndicesBufferObject;
        private static int jaredsQuadVBO;
        private static int jaredsQuadVAO;
        private static Matrix4F modelMatrix = new Matrix4F(1.0F);
        private static Matrix4F prevModelMatrix;
        private static Matrix4F rotationMatrix;
        private static Texture quadTexture;
        private static Shader shader;

        private static float[] jaredsQuadVertices = new float[]
        {  /*x      y     z     r     g     b     u     v */
           -0.5F, -0.5F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, //*vertex 0 bottom left*//*
            0.5F, -0.5F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, //*vertex 1 bottom right*//*
           -0.5F,  0.5F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, //*vertex 2 top left*//*
            0.5F,  0.5F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F  //*vertex 3 top right*//*

        };
        private static UInt32[] jaredsQuadIndices = new UInt32[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };

        public static void init()
        {
            rotationMatrix.SetRotateY((float)MathUtil.radians(2.0D));
            quadTexture = new Texture(@"..\..\res\texture\aie.png", true);
            shader = new Shader(@"..\..\src\Rendering\Shaders\ColourTextureShader3D.shader");
            genBuffers();
            onTick(); // this is done in init so the model is correctly transformed on the first few frames of display
            hasInitialized = true;
        }
        private static void genBuffers()
        {
            jaredsQuadVAO = GL.GenVertexArray();
            jaredsQuadIndicesBufferObject = GL.GenBuffer();
            jaredsQuadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, jaredsQuadIndicesBufferObject);
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private static void bind()
        {
            if (!hasInitialized) init();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, jaredsQuadIndicesBufferObject);
            GL.BindVertexArray(jaredsQuadVAO);
            quadTexture.use();
            shader.use();
        }

        public static void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, float percentageNextTick)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", prevModelMatrix + (modelMatrix - prevModelMatrix) * percentageNextTick);//interpolating model matrix between ticks

            GL.DrawElements(PrimitiveType.Triangles ,jaredsQuadIndices.Length, DrawElementsType.UnsignedInt, 0);
            unbind();
        }
        
        private static void unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
        }
        
        public static void onTick()
        {
            prevModelMatrix = modelMatrix;//store current state in previous model matrix for interpolation

            modelMatrix *= rotationMatrix;
        }

    }
}
