using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine
{
    //TODO: Add HDR and Bloom
    public static class PostProcessing
    {
        private static readonly int NUM_MAIN_COLOR_ATTACHMENTS = 2;
        private static readonly int NUM_PINGPONG_FRAMEBUFFERS = 2;
        private static int[] mainColorAttachments = new int[NUM_MAIN_COLOR_ATTACHMENTS]; 
        private static int[] blurPPFBOs = new int[NUM_PINGPONG_FRAMEBUFFERS]; 
        private static int[] blurPPColorBuffers = new int[NUM_PINGPONG_FRAMEBUFFERS]; 
        private static int mainFrameBuffer;
        private static int mainDepthBuffer;
        private static int mainFrameWidth;
        private static int mainFrameHeight;
        private static FramebufferErrorCode errorCode;

        private static Model screenQuad;
        private static VertexArrayObject screenQuadVAO;
        private static Shader mainFrameBufferShader;
        private static Shader resultFrameBufferShader;
        private static Shader gBlurFrameBufferShader;

        public static void init()
        {
            mainFrameWidth = (int)(GameInstance.realScreenWidth * GameSettings.renderScale.floatValue);
            mainFrameHeight = (int)(GameInstance.realScreenHeight * GameSettings.renderScale.floatValue);

            setUpVaoAndShaders();
            setUpMainFrameBuffer();
            setUpBlurBuffers(); 
        }

        private static void setUpMainFrameBuffer()
        {
            mainFrameBuffer = GL.GenFramebuffer();
            mainDepthBuffer = GL.GenRenderbuffer();
            GL.GenTextures(NUM_MAIN_COLOR_ATTACHMENTS, mainColorAttachments);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer);

            DrawBuffersEnum[] attachments = new DrawBuffersEnum[NUM_MAIN_COLOR_ATTACHMENTS];
            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++) attachments[i] = DrawBuffersEnum.ColorAttachment0 + i;
            GL.DrawBuffers(NUM_MAIN_COLOR_ATTACHMENTS, attachments);

            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, mainColorAttachments[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f,  mainFrameWidth,  mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mainColorAttachments[i], 0);
            }

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mainDepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, mainFrameWidth, mainFrameHeight);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, mainDepthBuffer);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.warn("Main framebuffer status incomplete!");
            }
            else if (errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.infoPrint("Main FrameBuffer inizialized.");
            }
        }

        private static void setUpBlurBuffers()
        {
            GL.GenFramebuffers(NUM_PINGPONG_FRAMEBUFFERS, blurPPFBOs);
            GL.GenTextures(NUM_PINGPONG_FRAMEBUFFERS, blurPPColorBuffers);
            for (int i = 0; i < NUM_PINGPONG_FRAMEBUFFERS; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, blurPPFBOs[i]);
                GL.BindTexture(TextureTarget.Texture2D, blurPPColorBuffers[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, 1024, 1024, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, blurPPColorBuffers[i], 0);
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

        public static void setUpVaoAndShaders()
        {
            screenQuad = QuadPrefab.copyModel().scaleVertices(new Vector3(2, 2, 1)).setColor(Color.white);
            screenQuadVAO = new VertexArrayObject();
            screenQuadVAO.beginBuilding();
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            screenQuadVAO.addBuffer(screenQuad.vertices, Vertex.SIZE_BYTES, l);
            screenQuadVAO.addIndicesBuffer(screenQuad.indices);
            screenQuadVAO.finishBuilding();
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferResultName, out resultFrameBufferShader);
            resultFrameBufferShader.use();
            resultFrameBufferShader.setUniform1I("bloomTexture", 1);
            resultFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferGBlurName, out gBlurFrameBufferShader);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferMainName, out mainFrameBufferShader);
        }

        public static void onResize()
        {
            mainFrameWidth = (int)(GameInstance.gameWindowWidth * GameSettings.renderScale.floatValue);
            mainFrameHeight = (int)(GameInstance.gameWindowHeight * GameSettings.renderScale.floatValue);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer);

            for (int i = 0; i < NUM_MAIN_COLOR_ATTACHMENTS; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, mainColorAttachments[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, mainFrameWidth, mainFrameHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, mainColorAttachments[i], 0);
            }

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mainDepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, mainFrameWidth, mainFrameHeight);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, mainDepthBuffer);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.warn("Framebuffers could not resize!");
            }
        }

        public static void prepare()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mainFrameBuffer);
            GL.Viewport(0, 0, mainFrameWidth, mainFrameHeight);
        }

        public static void doPostProcessing()
        {
            GL.Disable(EnableCap.DepthTest);
            screenQuadVAO.bind();

            //gausiann blur the bloom texture
            int blurredBloom = gaussianBlur(mainColorAttachments[0]);

            //TODO: do all post processing
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mainColorAttachments[0]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, blurredBloom);//Now blurred bloom tex
            resultFrameBufferShader.use();
            GL.DrawElements(PrimitiveType.Triangles, screenQuad.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;
        }

        /// <summary>
        /// Blurs the color buffer containing the bloom fragments using buffer ping ponging and recursion
        /// </summary>
        /// <param name="srcTex">The id of the source texture to be blurred</param>
        /// <param name="layer">The layer of blurring to apply.</param>
        /// <returns>The id of the colorbuffer containing the final result</returns>
        private static int gaussianBlur(int srcTex, int layer = 0)//TODO: This wont work. different resolution blurs need to be added at the end, not applied to each other!
        {
            //TODO: for some reason blur can make the image even brighter. figure out new way of blurring and bloom that doesnt increase luminosity.
            if (layer >= 3) return srcTex;

            //1024px * 16it * 5krn , 256px * 8it * 11krn, 64px * 4it * 21krn

            int size = 1024 >> (layer * 2);//decrease size for each layer for bigger kernel and less itterations

            GL.Viewport(0, 0, size, size);
            bool highLayer = Convert.ToBoolean(layer);//if the layer is more than 0 true

            bool firstIter = true;
            int evenItterBool = 0;//1 if itteration is even else 0
            int itterations = 16 >> layer;
            gBlurFrameBufferShader.use();
            gBlurFrameBufferShader.setUniform1I("layer", 0);

            for(int i = 0; i < itterations; i++)
            {
                evenItterBool = i & 1;
                gBlurFrameBufferShader.setUniform1I("verticalPass", evenItterBool);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, blurPPFBOs[evenItterBool]);//target framebuffer
                GL.BindTexture(TextureTarget.Texture2D, firstIter ? srcTex : blurPPColorBuffers[1-evenItterBool]);//source texture

                if(highLayer && i < 2)//If not layer 0, resize the target buffer. Do only twice so both buffers get resized.
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, size, size, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, blurPPColorBuffers[evenItterBool], 0);
                }

                GL.DrawElements(PrimitiveType.Triangles, screenQuad.indices.Length, DrawElementsType.UnsignedInt, 0);
                Renderer.totalDraws++;
                firstIter = false;
            }
            return blurPPColorBuffers[1-evenItterBool];//get the final color buffer result of the blurring
        }


        public static void onVideoSettingsChanged()
        {
            onResize();
            resultFrameBufferShader.use();
            resultFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
        }

        public static void onClosing()
        {
            GL.DeleteFramebuffer(mainFrameBuffer);
            GL.DeleteRenderbuffer(mainDepthBuffer);
            GL.DeleteFramebuffers(NUM_PINGPONG_FRAMEBUFFERS, blurPPFBOs);
            GL.DeleteTextures(NUM_PINGPONG_FRAMEBUFFERS, blurPPColorBuffers);
            GL.DeleteTextures(NUM_MAIN_COLOR_ATTACHMENTS, mainColorAttachments);
            screenQuadVAO.delete();
        }

        public static Vector2 size { get => new Vector2(mainFrameWidth, mainFrameHeight); }
        public static int getWidth { get => mainFrameWidth; }
        public static int getHeight { get => mainFrameHeight; }
    }
}