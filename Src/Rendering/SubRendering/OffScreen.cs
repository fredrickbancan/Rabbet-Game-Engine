using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine.SubRendering
{
    /*This static class will be used for rendering scenes to an off-screen buffer for super sampling and other effects.*/
    public static class OffScreen
    {
        private static readonly int blendFrames = 2;//number of frames to store and blend together must be atleast 1
        private static uint[] screenQuadIndices;
        private static int screenQuadVAO;
        private static int screenQuadVBO;
        private static int screenQuadIBO;
        private static Model screenQuad;
        private static Shader screenShader;
        private static readonly string screenShaderName = "Offscreen";
        private static int[] frameBuffers;//for storing the previous frame's image data, useful for frame blending. frameBuffers[0] will always be the main FBO render target.
        private static int[] colorBuffers;//for storing the previous frame's image data, useful for frame blending. frameBuffers[0] will always be the main FBO render target.
        private static int depthBuffer;
        private static int width;
        private static int height;
        private static FramebufferErrorCode errorCode;

        public static void init(int width, int height)
        {
            Application.debugPrint("Rendering is utilizing offscreen buffer!");

            frameBuffers = new int[blendFrames];
            colorBuffers = new int[blendFrames];

            GL.GenFramebuffers(blendFrames, frameBuffers);
            GL.GenTextures(blendFrames, colorBuffers);
            depthBuffer = GL.GenRenderbuffer();

            OffScreen.width = width;
            OffScreen.height = height;

            GL.BindTexture(TextureTarget.Texture2D, colorBuffers[0]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);


            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, width, height);

            //binding texture and depth buffer to frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffers[0]);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorBuffers[0], 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,RenderbufferTarget.Renderbuffer, depthBuffer);
            
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //setting up the previous frame frame buffer
            for (int i = 1; i < blendFrames; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, colorBuffers[i]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffers[i]);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorBuffers[i], 0);

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.error("Offscreen could not initialize offscreen frame buffers!");
            }
            else if(errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.debugPrint("Offscreen frame buffer successfully initialized!");
            }

            //set up quad for rendering
            ShaderUtil.tryGetShader(screenShaderName, out screenShader);
            screenShader.use();
            screenShader.setUniform1I("frameTexture0", 0);
            screenShader.setUniform1I("frameTexture1", 1);
            screenShader.setUniform1I("frameTexture2", 2);
            screenShader.setUniform1I("frameTexture3", 3);
            screenShader.setUniform1I("frameTexture4", 4);
            screenShader.setUniform1I("frameTexture5", 5);
            screenShader.setUniform1I("frameTexture6", 6);
            screenShader.setUniform1I("frameTexture7", 7);
            screenQuad = QuadPrefab.copyModel().scaleVertices(new Vector3(2.0F, 2.0F, 1.0F));//scale quad to fit screen
            screenQuadIndices = QuadCombiner.getIndicesForQuadCount(1);

            //VAO
            screenQuadVAO = GL.GenVertexArray();
            GL.BindVertexArray(screenQuadVAO);

            //IBO
            screenQuadIBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, screenQuadIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, screenQuadIndices.Length * sizeof(uint), screenQuadIndices, BufferUsageHint.StaticDraw);

            //VBO
            screenQuadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, screenQuadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, screenQuad.vertices.Length * Vertex.vertexByteSize, screenQuad.vertices, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);

        }

        public static void prepareToRenderToOffScreenTexture()
        {
            for (int i = blendFrames - 2; i > -1; i--)
            {
                GL.BlitNamedFramebuffer(0, frameBuffers[i + 1], 0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffers[0]);
            
           // GL.BlitNamedFramebuffer(frameBuffers[0], frameBuffers[1], 0, 0, width, height, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
            GL.Viewport(0, 0, width, height);
        }

        public static void renderOffScreenTexture()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(screenQuadVAO);
            GL.BindTexture(TextureTarget.Texture2D, colorBuffers[0]);
            for(int i = 1; i < blendFrames; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, colorBuffers[i]);
            }
            screenShader.use();
            GL.DrawElements(PrimitiveType.Triangles, screenQuadIndices.Length, DrawElementsType.UnsignedInt, 0); 
            Renderer.totalDraws++;
            GL.ActiveTexture(TextureUnit.Texture0);

        }


        public static void onClose()
        {
            if (!Renderer.useOffScreenBuffer)
                return;
            GL.DeleteRenderbuffer(depthBuffer);
            GL.DeleteTextures(blendFrames - 1, colorBuffers);
            GL.DeleteFramebuffers(blendFrames - 1, frameBuffers);
            GL.DeleteBuffer(screenQuadIBO);
            GL.DeleteBuffer(screenQuadVBO);
            GL.DeleteVertexArray(screenQuadVAO);
        }
        public static int getWidth { get => width; }
        public static int getHeight { get => height; }
    }
}
