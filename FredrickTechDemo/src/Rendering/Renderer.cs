using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
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
        private static ModelDrawable cactusModel;
        private static ModelDrawable cactusTopFaceModel;
        private static Matrix4F projectionMatrix;
        private static Vector3F fogColour = ColourF.lightBlossom.normalVector3F();
        private static Vector3F skyColour = ColourF.skyBlue.normalVector3F();
        
        /*Called before any rendering is done*/
        public static void init(GameInstance game)
        {
            Renderer.gameInstance = game;
            gameInstance.MakeCurrent();
            privateTextRenderer2D = new TextRenderer2D("Consolas", 512);
            setClearColor(skyColour);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Viewport(gameInstance.ClientRectangle);
            Model[] temp = new Model[5];
            temp[0] = QuadPrefab.getNewModel().translateVertices(new Vector3F(0.0F, 0.5F, 0.4375F)).setColor(new Vector4F(0.8F,0.8F,0.8F,1));
            temp[1] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1,1,1), new Vector3F(0, 90, 0), new Vector3F(-0.4375F, 0.5F, 0)).setColor(new Vector4F(0.5F, 0.5F, 0.5F, 1)); 
            temp[2] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, 180, 0), new Vector3F(0F, 0.5F, -0.4375F)).setColor(new Vector4F(0.8F, 0.8F, 0.8F, 1)); 
            temp[3] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, -90, 0), new Vector3F(0.4375F, 0.5F, 0)).setColor(new Vector4F(0.5F, 0.5F, 0.5F, 1));
            cactusModel = QuadBatcher.batchQuadModels(temp, QuadPrefab.getShaderDir(), QuadPrefab.getTextureDir());
            cactusTopFaceModel = QuadBatcher.batchQuadModels(new Model[] { QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(90, 0, 0), new Vector3F(0, 1F, 0)) }, QuadPrefab.getShaderDir(), ResourceHelper.getTextureFileDir("cactus_top.png")); 
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
            gameInstance.thePlayer.onCameraUpdate();
            gameInstance.currentPlanet.getSkyboxModel().draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, skyColour, fogColour);
            gameInstance.currentPlanet.getTerrainModel().draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, fogColour);
            cactusModel.draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, fogColour);
            cactusTopFaceModel.draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, fogColour);
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
 