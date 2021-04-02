using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    //TODO: add Blurring and bloom as an object oriented class for cleaner and faster code.
    //TODO: Add HDR and Bloom
    public static class PostProcessing
    {
        private static int mainFBWidth;
        private static int mainFBHeight;
        private static Shader mainFrameBufferShader;
        private static Shader resultFrameBufferShader;

        public static void init()
        {
            FullScreenQuad.init();
            mainFBWidth = (int)(GameInstance.realScreenWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.realScreenHeight * GameSettings.renderScale.floatValue);
        }
        public static void onResize()
        {
            mainFBWidth = (int)(GameInstance.gameWindowWidth * GameSettings.renderScale.floatValue);
            mainFBHeight = (int)(GameInstance.gameWindowHeight * GameSettings.renderScale.floatValue);
        }

        public static void beforeRender()
        {
            GL.Enable(EnableCap.DepthTest);

        }

        public static void doPostProcessing()
        {
            GL.Disable(EnableCap.DepthTest);

        }

        public static void onVideoSettingsChanged()
        {
            onResize();
            resultFrameBufferShader.use();
            resultFrameBufferShader.setUniform1F("gamma", GameSettings.gamma.floatValue);
            resultFrameBufferShader.setUniform1F("exposure", GameSettings.exposure.floatValue);
        }

        public static void onClosing()
        {
            FullScreenQuad.delete();
        }

        public static Vector2 viewPortSize { get => new Vector2(mainFBWidth, mainFBHeight); }
    }
}