using OpenTK;
using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;
using RabbetGameEngine.VFX;
using System.Drawing;

namespace RabbetGameEngine
{
    //TODO: REWORK SHADERS FOR NEWLY IMPLIMENTED BATCHING PIPELINE

    /*This class will be responsable for most of the games rendering requests. It will then send the requests to the suitable sub renderers.
      e.g, when the game requests text to be rendered on the screen, the renderer will send a request to the TextRenderer2D.
      e.g, when the game requests entity models to be rendered in the world, the renderer will send a request to the model draw function.
      This class also contains the projection matrix.*/
    public static class Renderer
    {
        private static int privateTotalDrawCallCount;
        private static Matrix4 projectionMatrix;
        public static readonly bool useOffScreenBuffer = false;
        private static int renderFrame;//used to animate textures (noise texture for now)
        private static Rectangle preFullScreenSize;//used to store the window dimentions before going into full screen

        /*Called before any rendering is done*/
        public static void init()
        {
            ShaderUtil.loadAllFoundShaderFiles();
            TextureUtil.loadAllFoundTextureFiles();
            ModelUtil.loadAllFoundModelFiles();
            Application.infoPrint("OpenGL Version: " + GL.GetString(StringName.Version));
            Application.infoPrint("OpenGL Vendor: " + GL.GetString(StringName.Vendor));
            Application.infoPrint("Shading Language Version: " + GL.GetString(StringName.ShadingLanguageVersion));
            Application.debugPrint("Loaded " + ShaderUtil.getShaderCount() + " shaders.");
            Application.debugPrint("Loaded " + TextureUtil.getTextureCount() + " textures.");
            Application.debugPrint("Loaded " + ModelUtil.getModelCount() + " models.");

            GL.Viewport(preFullScreenSize = GameInstance.get.ClientRectangle);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.VertexProgramPointSize);//allows shaders for GL_POINTS to change size of points.
            GL.Enable(EnableCap.PointSprite);           //allows shaders for GL_POINTS to change point fragments (opentk exclusive)
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(3);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            if(useOffScreenBuffer) OffScreen.init();
            Application.debugPrint("\n\n\n_______SYSTEM RENDERING CAPABILITIES_______");
            RenderAbility.setMaxUniformComponents(GL.GetInteger(GetPName.MaxVertexUniformComponents));
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(GameInstance.get.ClientRectangle);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            GUIHandler.onWindowResize();
        }

        /*called once per frame*/
        public static void onFrame()
        {
            
        }

        public static void requestRender(BatchType type, Texture tex, Model mod)
        {
            Profiler.beginEndProfile("batching");
            BatchManager.requestRender(type, tex, mod);
            Profiler.beginEndProfile("batching");
        }

        public static void onTickStart()
        {
            BatchManager.updateAll();
        }
        public static void onTickEnd()
        {
            BatchManager.prepareAll();
        }
        public static void renderAll()
        {
            preRender();
            renderWorld();
            BatchManager.drawAll(projectionMatrix, GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
            GUIHandler.drawCurrentGUIScreen();
            postRender();
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            if (useOffScreenBuffer) OffScreen.prepareToRenderToOffScreenTexture();
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }
        
        private static void renderWorld()//TODO: obselete,  impliment batcher pipeline
        {
            privateTotalDrawCallCount = 0;
            GameInstance.get.currentPlanet.getSkyboxModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getSkyColor(), GameInstance.get.currentPlanet.getFogColor());
            if(GameSettings.drawHitboxes)HitboxRenderer.renderAll(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix);
            GameInstance.get.currentPlanet.getGroundModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getFogColor());
            GameInstance.get.currentPlanet.getWallsModel().draw(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix, GameInstance.get.currentPlanet.getFogColor());
            GameInstance.get.currentPlanet.drawEntities(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix);
            GameInstance.get.currentPlanet.drawVFX(GameInstance.get.thePlayer.getViewMatrix(), projectionMatrix);
        }



        /*Called after all draw calls*/
        private static void postRender()
        {
            if (useOffScreenBuffer) OffScreen.renderOffScreenTexture();
            GameInstance.get.SwapBuffers();
            renderFrame++;
            renderFrame %= 4096;
        }

        /*render requests will add a new entry to a suitable batch*/
        public static void requestRenderObject(PositionalObject obj, Model viewModel, uint[] indices, BatchType BatchType)
        {
        }

        public static void requestRenderObject(PositionalObject obj, ModelDrawable viewModel, BatchType BatchType)
        {
        }

        public static void requestRenderVFX(VFXBase obj, ModelDrawable viewModel, BatchType BatchType)
        {
        }

        public static int getAndResetTotalDrawCount()
        {
            int result = privateTotalDrawCallCount;
            privateTotalDrawCallCount = 0;
            return result;
        }

        public static void onToggleFullscreen()
        {
            if (GameSettings.fullscreen)
            {
                preFullScreenSize = GameInstance.get.ClientRectangle;
                GameInstance.get.WindowState = WindowState.Fullscreen;
            }
            else
            {
                GameInstance.get.WindowState = WindowState.Normal;
                GameInstance.get.ClientRectangle = preFullScreenSize;
            }

        }

        /*deletes all loaded opengl assets*/
        public static void onClosing()
        {
            BatchManager.deleteAll();
            ShaderUtil.deleteAll();
            TextureUtil.deleteAll();
            ModelUtil.deleteAll();
            OffScreen.onClose();
        }

        public static Matrix4 projMatrix { get => projectionMatrix; }
        public static int frame { get => renderFrame; }
        public static int totalDraws { get { return privateTotalDrawCallCount; } set { privateTotalDrawCallCount = value; } }

    }
}
 