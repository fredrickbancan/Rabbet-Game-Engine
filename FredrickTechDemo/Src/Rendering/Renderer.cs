using FredrickTechDemo.FredsMath;
using FredrickTechDemo.SubRendering;
using OpenTK.Graphics.OpenGL;

namespace FredrickTechDemo
{
    /*This class will be responsable for most of the games rendering requests. It will then send the requests to the suitable sub renderers.
      e.g, when the game requests text to be rendered on the screen, the renderer will send a request to the TextRenderer2D.
      e.g, when the game requests entity models to be rendered in the world, the renderer will send a request to the model draw function.
      This class also contains the projection matrix.*/
    public static class Renderer
    {
        private static GameInstance gameInstance;
        private static TextRenderer2D privateTextRenderer2D;
        private static Matrix4F projectionMatrix;
        
        /*Called before any rendering is done*/
        public static void init(GameInstance game)
        {
            Renderer.gameInstance = game;
            gameInstance.MakeCurrent();
            privateTextRenderer2D = new TextRenderer2D("Trebuchet", 512);
            setClearColor(ColourF.black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            privateTextRenderer2D.onWindowResize();
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        
        public static void renderAll()
        {
            Profiler.beginEndProfile(Profiler.renderingName);
            preRender();
            updateCameraAndRenderWorld();
            renderGui();
            postRender();
            Profiler.beginEndProfile(Profiler.renderingName);
        }

        private static void updateCameraAndRenderWorld()
        {
            gameInstance.thePlayer.onCameraUpdate();//do this first
            gameInstance.currentPlanet.drawEntities(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix);
            gameInstance.currentPlanet.getTerrainModel().draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, gameInstance.currentPlanet.getFogColor());
            gameInstance.currentPlanet.getSkyboxModel().draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, gameInstance.currentPlanet.getSkyColor(), gameInstance.currentPlanet.getFogColor());
           
        }

        private static void renderGui()
        {
            privateTextRenderer2D.renderAnyText();
        }

        /*Called after all draw calls*/
        private static void postRender()
        {
            gameInstance.SwapBuffers();
        }

        public static void setClearColor(ColourF color)
        {
            float r = MathUtil.normalize(0, 255, color.GetRed());
            float g = MathUtil.normalize(0, 255, color.GetGreen());
            float b = MathUtil.normalize(0, 255, color.GetBlue());
            GL.ClearColor(r, g, b, 1.0f);
        }
        public static void setClearColor(Vector3F colorNormalized)
        {
            GL.ClearColor(colorNormalized.x, colorNormalized.y, colorNormalized.z, 1.0f);
        }

        public static TextRenderer2D textRenderer2D
        { get { return privateTextRenderer2D; } }
    }
}
 