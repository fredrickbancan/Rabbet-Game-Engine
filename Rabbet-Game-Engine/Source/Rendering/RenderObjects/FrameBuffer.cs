using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine
{
    //TODO: Add HDR and Bloom
    //TODO: Make into a non static object so multiple framebuffers can be used for things like multi stage post processing (guassian blur)
    public static class FrameBuffer
    {
        private static readonly int NUM_MAIN_COLOR_ATTACHMENTS = 2;
        private static readonly int NUM_PINGPONG_FRAMEBUFFERS = 2;
        private static int[] pingPongFBOs = new int[NUM_PINGPONG_FRAMEBUFFERS]; 
        private static int[] pingPongColorBuffers = new int[NUM_PINGPONG_FRAMEBUFFERS]; 
        private static int[] mainColorBuffers = new int[NUM_MAIN_COLOR_ATTACHMENTS]; 
        private static int mainFrameBuffer;
        private static int mainDepthBuffer;
        private static int mainFrameWidth;
        private static int mainFrameHeight;
        private static FramebufferErrorCode errorCode;

        private static Model screenQuad;
        private static VertexArrayObject screenQuadVAO;
        private static Shader mainFrameBufferShader;
        private static Texture ditherTex;

        public static void init()
        {
            mainFrameWidth = (int)(GameInstance.realScreenWidth * GameSettings.renderScale.floatValue);
            mainFrameHeight = (int)(GameInstance.realScreenHeight * GameSettings.renderScale.floatValue);

            GL.ActiveTexture(TextureUnit.Texture0);

            setUpMainFrameBuffer();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.ActiveTexture(TextureUnit.Texture0);

            setUpPingPongBuffers();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            setUpScreenQuad();
        }

        private static void setUpMainFrameBuffer()
        {
            GL.GenTextures(NUM_MAIN_COLOR_ATTACHMENTS, mainColorBuffers);
            mainFrameBuffer = GL.GenFramebuffer();
            mainDepthBuffer = GL.GenRenderbuffer();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mainDepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, mainFrameWidth, mainFrameHeight);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, mainDepthBuffer);
         
           
            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, mainColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, mainFrameWidth, mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mainColorBuffers[i], 0);
            }

            DrawBuffersEnum[] attachments = new DrawBuffersEnum[NUM_MAIN_COLOR_ATTACHMENTS];
            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++) attachments[i] = DrawBuffersEnum.ColorAttachment0 + i;
            GL.DrawBuffers(NUM_MAIN_COLOR_ATTACHMENTS, attachments);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.warn("Main framebuffer status incomplete!");
            }
            else if (errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.infoPrint("Main FrameBuffer inizialized.");
            }
        }

        private static void setUpPingPongBuffers()
        {
            GL.GenFramebuffers(NUM_PINGPONG_FRAMEBUFFERS, pingPongFBOs);
            GL.GenTextures(NUM_PINGPONG_FRAMEBUFFERS, pingPongColorBuffers);
            for (int i = 0; i < NUM_PINGPONG_FRAMEBUFFERS; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingPongFBOs[i]);
                GL.BindTexture(TextureTarget.Texture2D, pingPongColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, mainFrameWidth, mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pingPongColorBuffers[i], 0);
            }

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.warn("pingpong framebuffers status incomplete!");
            }
            else if (errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.infoPrint("pingpong FrameBuffers initialized.");
            }
        }

        public static void setUpScreenQuad()
        {
            screenQuad = QuadPrefab.copyModel().scaleVertices(new Vector3(2, 2, 1)).setColor(Color.white);
            screenQuadVAO = new VertexArrayObject();
            screenQuadVAO.beginBuilding();
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            screenQuadVAO.addBuffer(screenQuad.vertices, Vertex.SIZE_BYTES, l);
            screenQuadVAO.addIndicesBuffer(screenQuad.indices);
            screenQuadVAO.finishBuilding();
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferMainName, out mainFrameBufferShader);
            mainFrameBufferShader.use();
            mainFrameBufferShader.setUniform1I("ditherTex", 1);
            mainFrameBufferShader.setUniform1F("height", MathF.Tan(MathUtil.radians(GameSettings.fov.floatValue) / 2.0F));
            mainFrameBufferShader.setUniform1F("barrelDistortion", GameSettings.barrelDistortion.floatValue * GameSettings.fov.floatValue * 0.01F);
            mainFrameBufferShader.setUniform1F("cylRatio", GameSettings.barrelDistortionCylRatio);
            mainFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            TextureUtil.tryGetTexture("dither", out ditherTex);
        }

        public static void onResize()
        {
            //PROBLEM: framebuffers render black unless this func is called, then all framebuffer color outputs are the same:/
            mainFrameWidth = (int)(GameInstance.gameWindowWidth * GameSettings.renderScale.floatValue);
            mainFrameHeight = (int)(GameInstance.gameWindowHeight * GameSettings.renderScale.floatValue);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mainDepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, mainFrameWidth, mainFrameHeight);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer); 
            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, mainColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, mainFrameWidth, mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mainColorBuffers[i], 0);
            }
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, mainDepthBuffer);
           
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            for (int i = 0; i < NUM_PINGPONG_FRAMEBUFFERS; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingPongFBOs[i]);
                GL.BindTexture(TextureTarget.Texture2D, pingPongColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, mainFrameWidth, mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pingPongColorBuffers[i], 0);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.warn("Framebuffers could not resize!");
            }
            mainFrameBufferShader.use();
            if (mainFrameWidth > 0 && mainFrameHeight > 0)
                mainFrameBufferShader.setUniform1F("aspectRatio", mainFrameWidth / mainFrameHeight);
        }

        public static void bind()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer);
            GL.Viewport(0, 0, mainFrameWidth, mainFrameHeight);
        }

        public static void doFinalScreenQuadRender()
        {
            //TODO: do all post processing and render result texture instead of main framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            GL.Disable(EnableCap.DepthTest); 
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mainColorBuffers[1]);
            screenQuadVAO.bind();
            mainFrameBufferShader.use();
            mainFrameBufferShader.setUniformVec3F("cameraFrontVec", GameInstance.get.thePlayer.getCamera().getFrontVector());
            GL.DrawElements(PrimitiveType.Triangles, screenQuad.indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public static void onVideoSettingsChanged()
        {
            onResize();
            mainFrameBufferShader.use();
            mainFrameBufferShader.setUniform1F("height", MathF.Tan(MathUtil.radians(GameSettings.fov.floatValue) / 2.0F));
            mainFrameBufferShader.setUniform1F("barrelDistortion", GameSettings.barrelDistortion.floatValue * GameSettings.fov.floatValue * 0.01F);
            mainFrameBufferShader.setUniform1F("cylRatio", GameSettings.barrelDistortionCylRatio);
            mainFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
        }

        public static void onClosing()
        {
            GL.DeleteFramebuffer(mainFrameBuffer);
            GL.DeleteRenderbuffer(mainDepthBuffer);
            GL.DeleteFramebuffers(NUM_PINGPONG_FRAMEBUFFERS, pingPongFBOs);
            GL.DeleteTextures(NUM_PINGPONG_FRAMEBUFFERS, pingPongColorBuffers);
            GL.DeleteTextures(NUM_MAIN_COLOR_ATTACHMENTS, mainColorBuffers);
            screenQuadVAO.delete();
        }

        public static Vector2 size { get => new Vector2(mainFrameWidth, mainFrameHeight); }
        public static int getWidth { get => mainFrameWidth; }
        public static int getHeight { get => mainFrameHeight; }
    }
}