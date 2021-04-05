using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public static class PostProcessing
    {
        private static HdrFrameBuffer mainFBO;
        private static GaussianBlurFilter blurFilter;
        private static int mainFBWidth;
        private static int mainFBHeight;
        private static Shader mainFrameBufferShader;
        private static Shader finalFrameBufferShader;

        public static void init()
        {
            FrameBufferQuad.init();
            mainFBWidth = (int)(GameInstance.realScreenWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.realScreenHeight * GameSettings.renderScale.floatValue);
            mainFBO = new HdrFrameBuffer(mainFBWidth, mainFBHeight, TextureMinFilter.Linear, TextureMagFilter.Nearest, true, 2);//2 outputs. One is for bloom texture to be blurred
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferMainName, out mainFrameBufferShader);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferFinalName, out finalFrameBufferShader);
            finalFrameBufferShader.use();
            finalFrameBufferShader.setUniform1I("bloomTexture", 1);
            finalFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            finalFrameBufferShader.setUniform1F("exposure", GameSettings.exposure.floatValue);
            blurFilter = new GaussianBlurFilter();
            blurFilter.init();
        }
        public static void onResize()
        {
            mainFBWidth = (int)(GameInstance.gameWindowWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.gameWindowHeight * GameSettings.renderScale.floatValue);
            mainFBO.resize(mainFBWidth, mainFBHeight);
        }

        public static void beforeRender()
        {
            GL.Enable(EnableCap.DepthTest);
            mainFBO.use();//render scene to main fbo
        }

        public static void doPostProcessing()
        {
            GL.Disable(EnableCap.DepthTest);

            //draw scene on full screen quad to get outputs
            mainFBO.use();
            mainFBO.bindOutputTexture(0);
            mainFrameBufferShader.use();
            FrameBufferQuad.bindVao();
            FrameBufferQuad.draw();

            int blurredBloomTex = blurFilter.processImage(mainFBO.getOutputTexture(1));


            //Render final result to full screen quad at full res
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            finalFrameBufferShader.use();
            GL.ActiveTexture(TextureUnit.Texture0);
            mainFBO.bindOutputTexture(0);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, blurredBloomTex);
            GL.ActiveTexture(TextureUnit.Texture0);
            FrameBufferQuad.draw();
        }

        public static void onVideoSettingsChanged()
        {
            onResize();
            finalFrameBufferShader.use();
            finalFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            finalFrameBufferShader.setUniform1F("exposure", GameSettings.exposure.floatValue);
        }

        public static void onClosing()
        {
            FrameBufferQuad.delete();
            mainFBO.delete();
            blurFilter.delete();
        }

        public static Vector2 viewPortSize { get => new Vector2(mainFBWidth, mainFBHeight); }
    }
}