using Coictus.Debugging;
using Coictus.GUI;
using Coictus.SubRendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Coictus
{

    /*This class will be responsable for most of the games rendering requests. It will then send the requests to the suitable sub renderers.
      e.g, when the game requests text to be rendered on the screen, the renderer will send a request to the TextRenderer2D.
      e.g, when the game requests entity models to be rendered in the world, the renderer will send a request to the model draw function.
      This class also contains the projection matrix.*/
    public static class Renderer
    {
        private static Matrix4 projectionMatrix;
        /*Called before any rendering is done*/
        public static void init()
        {
            setClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.VertexProgramPointSize);//allows shaders for GL_POINTS to change size of points.
            GL.Enable(EnableCap.PointSprite);           //allows shaders for GL_POINTS to change point fragments (opentk exclusive)
           /* GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.SampleAlphaToCoverage);
            GL.Enable(EnableCap.SampleCoverage);
            GL.Enable(EnableCap.SampleAlphaToOne);
            GL.SampleCoverage(1.0F, false);*/
            GL.Viewport(GameInstance.get.ClientRectangle);
            GL.LineWidth(3);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            OffScreen.init(1F);
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(GameInstance.get.ClientRectangle);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            GUIHandler.onWindowResize();
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            OffScreen.prepareToRenderToOffScreenTexture();
            setClearColor(Color.SkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        
        public static void renderAll()
        {
            Profiler.beginEndProfile(Profiler.renderingName);
            preRender();
            updateCameraAndRenderWorld();
            GUIHandler.drawCurrentGUIScreen();
            postRender();
            Profiler.beginEndProfile(Profiler.renderingName);
        }

        private static void updateCameraAndRenderWorld()
        {
            GameInstance.get.thePlayer.onCameraUpdate();//do this first
            GameInstance.get.currentPlanet.drawEntities(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix);
            GameInstance.get.currentPlanet.drawVFX(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix);
            GameInstance.get.currentPlanet.getGroundModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getFogColor());
            GameInstance.get.currentPlanet.getWallsModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getFogColor());
            GameInstance.get.currentPlanet.getSkyboxModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getSkyColor(), GameInstance.get.currentPlanet.getFogColor());
            if(GameSettings.drawHitboxes)GameInstance.get.currentPlanet.drawDebugHitboxes();
            
        }
        /*Called after all draw calls*/
        private static void postRender()
        {
            OffScreen.renderOffScreenTexture();
            GameInstance.get.SwapBuffers();
        }

        public static void setClearColor(Color color)
        {
            float r = MathUtil.normalize(0, 255, color.R);
            float g = MathUtil.normalize(0, 255, color.G);
            float b = MathUtil.normalize(0, 255, color.B);
            GL.ClearColor(r, g, b, 1.0f);
        }
        public static void setClearColor(Vector3 colorNormalized)
        {
            GL.ClearColor(colorNormalized.X, colorNormalized.Y, colorNormalized.Z, 1.0f);
        }

        public static Matrix4 projMatrix { get => projectionMatrix; }
    }
}
 