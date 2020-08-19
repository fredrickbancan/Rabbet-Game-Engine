using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using OpenTK.Graphics.OpenGL;
using System;

namespace FredrickTechDemo.SubRendering
{
    /*This static class will be used for rendering scenes to an off-screen buffer for super sampling and other effects.*/
    public static class OffScreen
    {
        private static ModelDrawable screenQuad;
        private static Texture screenTexture;
        private static Shader screenShader;
        private static readonly String screenShaderDir = ResourceHelper.getShaderFileDir("Offscreen.shader");
        private static int textureID;
        private static int defaultFrameBuffer;
        private static int depthBuffer;
        private static int frameBuffer;
        private static int width;
        private static int height;
        private static DrawBuffersEnum[] drawBuffers;
        private static FramebufferErrorCode errorCode;

        public static void init(float superSamplingMultiplyer = 1.0F)
        {
            frameBuffer = GL.GenFramebuffer();
            depthBuffer = GL.GenRenderbuffer();
            textureID = GL.GenTexture();

            width = (int)(GameInstance.gameWindowWidth/* * superSamplingMultiplyer*/);//*2 on width and height for 4x super sampling
            height = (int)(GameInstance.gameWindowHeight/* * superSamplingMultiplyer*/);

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            screenTexture = new Texture(textureID);
            screenShader = new Shader(screenShaderDir);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, width, height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureID, 0);
            GL.ClearColor(1.0F, 0, 0, 1.0F);//setting clear color for this framebuffer to red because it should not be able to be seen

            drawBuffers = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0};
            GL.DrawBuffers(drawBuffers.Length, drawBuffers);

            
            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.error("Offscreen could not initialize offscreen frame buffers!");
            }
            else if(errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.debug("Offscreen frame buffer successfully initialized!");
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            screenQuad = (ModelDrawable)QuadPrefab.getNewModelDrawable().scaleVertices(new Vector3F(2.0F, 2.0F, 1.0F));
            screenQuad.setNewShader(screenShader);
        }

        public static void prepareForRender()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            //GL.Viewport(0, 0, width, height);
        }

        public static void postGameRender()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
           // GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            GL.ClearColor(1.0F, 0, 0, 1.0F);//setting clear color for this framebuffer to red because it should not be able to be seen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            screenQuad.draw(textureID);
        }
    }
}
