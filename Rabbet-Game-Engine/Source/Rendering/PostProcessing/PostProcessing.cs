using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public static class PostProcessing
    {
        private static HdrFrameBuffer offScreenFBO;//The FBO that the scene is rendered onto
        private static HdrFrameBuffer mainFBO;//The FBO that the offscreen FBO is rendered onto a quad to get color outputs
        private static GaussianBlurFilter blurFilter;
        private static int mainFBWidth;
        private static int mainFBHeight;
        private static Shader mainFrameBufferShader;
        private static Shader finalFrameBufferShader;
        private static bool initialized = false;

        public static void init()
        {
            FrameBufferQuad.init();
            mainFBWidth = (int)(GameInstance.realScreenWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.realScreenHeight * GameSettings.renderScale.floatValue);
            offScreenFBO = new HdrFrameBuffer(mainFBWidth, mainFBHeight, TextureMinFilter.Linear, TextureMagFilter.Nearest, true);
            mainFBO = new HdrFrameBuffer(mainFBWidth, mainFBHeight, TextureMinFilter.Linear, TextureMagFilter.Nearest, false, 2);//2 outputs. One is for bloom texture to be blurred
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferMainName, out mainFrameBufferShader);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferFinalName, out finalFrameBufferShader);
            finalFrameBufferShader.use();
            finalFrameBufferShader.setUniform1I("bloomTexture", 1);
            finalFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            finalFrameBufferShader.setUniform1F("exposure", GameSettings.exposure.floatValue);
            blurFilter = new GaussianBlurFilter();
            blurFilter.init(mainFBWidth, mainFBHeight);
            initialized = true;
        }

        public static void onResize()
        {
            if (!initialized) return;
            mainFBWidth = (int)(GameInstance.gameWindowWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.gameWindowHeight * GameSettings.renderScale.floatValue);
            offScreenFBO.resize(mainFBWidth, mainFBHeight);
            mainFBO.resize(mainFBWidth, mainFBHeight);
        }

        public static void beforeRender()
        {
            if (!initialized) return;
            GL.Enable(EnableCap.DepthTest);
            offScreenFBO.use();//render scene to offscreen fbo
        }

        public static void doPostProcessing()
        {
            if (!initialized) return;
            GL.Disable(EnableCap.DepthTest);

            //draw scene on full screen quad to get outputs
            mainFBO.use();
            offScreenFBO.bindOutputTexture();
            mainFrameBufferShader.use();
            FrameBufferQuad.bindVao();
            FrameBufferQuad.draw();

            int blurredBloomTex = blurFilter.processImage(mainFBO.getOutputTexture(1), mainFBWidth, mainFBHeight);

            //Render final result to full screen quad at full res
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            finalFrameBufferShader.use();
            GL.ActiveTexture(TextureUnit.Texture0);
            mainFBO.bindOutputTexture();
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, blurredBloomTex);
            GL.ActiveTexture(TextureUnit.Texture0);
            FrameBufferQuad.draw();

            GL.Enable(EnableCap.DepthTest);
        }

        public static void onVideoSettingsChanged()
        {
            if (!initialized) return;
            onResize();
            finalFrameBufferShader.use();
            finalFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            finalFrameBufferShader.setUniform1F("exposure", GameSettings.exposure.floatValue);
        }

        public static void onClosing()
        {
            if (!initialized) return; 
            FrameBufferQuad.delete();
            offScreenFBO.delete();
            mainFBO.delete();
            blurFilter.delete();
        }

        public static Vector2 viewPortSize { get => new Vector2(mainFBWidth, mainFBHeight); }
    }
}