using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine
{
    public static class FrameBuffer
    {
        public static float barrelDistortionSuperSampleModifyer = 0.15F;
        private static int texColorBuffer;
        private static int depthBuffer;
        private static int frameBuffer;
        private static int width;
        private static int height;
        private static FramebufferErrorCode errorCode;

        private static Model screenQuad;
        private static VertexArrayObject screenQuadVAO;
        private static Shader screenQuadShader;
        private static Texture ditherTex;

        public static void init()
        {
            frameBuffer = GL.GenFramebuffer();
            depthBuffer = GL.GenRenderbuffer();
            texColorBuffer = GL.GenTexture();

            width = (int)(GameInstance.realScreenWidth * (1.0F + GameSettings.barrelDistortionStrength * barrelDistortionSuperSampleModifyer));
            height = (int)(GameInstance.realScreenHeight * (1.0F + GameSettings.barrelDistortionStrength * barrelDistortionSuperSampleModifyer));

            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texColorBuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.error("Offscreen could not initialize offscreen frame buffers!");
            }
            else if (errorCode == FramebufferErrorCode.FramebufferComplete)
            {
                Application.infoPrint("Offscreen frame buffer successfully initialized!");
            }

            setUpScreenQuad();
        }

        public static void setUpScreenQuad()
        {
            screenQuad = QuadPrefab.copyModel().scaleVertices(new Vector3(2, 2, 1)).setColor(Color.white);
            screenQuadVAO = new VertexArrayObject();
            screenQuadVAO.beginBuilding();
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            screenQuadVAO.addBuffer(screenQuad.vertices, Vertex.vertexByteSize, l);
            screenQuadVAO.addIndicesBuffer(screenQuad.indices);
            screenQuadVAO.finishBuilding();
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferName, out screenQuadShader);
            screenQuadShader.use();
            screenQuadShader.setUniform1I("ditherTex", 1);
            TextureUtil.tryGetTexture("dither", out ditherTex);
        }

        public static void onResize()
        {
            width = (int)(GameInstance.gameWindowWidth * (1.0F + GameSettings.barrelDistortionStrength * barrelDistortionSuperSampleModifyer));
            height = (int)(GameInstance.gameWindowHeight * (1.0F + GameSettings.barrelDistortionStrength * barrelDistortionSuperSampleModifyer));

            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texColorBuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if ((errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                Application.error("Framebuffer could not resize!");
            }

        }

        public static void prepareToRenderToOffScreenTexture()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        public static void renderOffScreenTexture()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            screenQuadVAO.bind();
            screenQuadShader.use();
            screenQuadShader.setUniformVec3F("cameraFrontVec", GameInstance.get.thePlayer.getCamera().getFrontVector());
            if (GameInstance.gameWindowWidth > 0 && GameInstance.gameWindowHeight > 0)
                screenQuadShader.setUniform1F("aspectRatio", GameInstance.gameWindowWidth / GameInstance.gameWindowHeight);
            screenQuadShader.setUniform1F("height", MathF.Tan(MathUtil.radians(GameSettings.defaultFov) / 2.0F)); 
            screenQuadShader.setUniform1F("barrelDistortion", GameSettings.barrelDistortionStrength); 
            screenQuadShader.setUniform1F("cylRatio", GameSettings.barrelDistortionCylRatio); 
            screenQuadShader.setUniform1F("brightness", GameSettings.defaultBrightness); 
            GL.ActiveTexture(TextureUnit.Texture1);
            ditherTex.use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.DrawElements(PrimitiveType.Triangles, screenQuad.indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        public static Vector2 size { get => new Vector2(width, height); }
        public static int getWidth { get => width; }
        public static int getHeight { get => height; }
    }
}