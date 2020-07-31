using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering;
using OpenTK.Graphics.OpenGL;
using System;

namespace FredrickTechDemo
{
    /*This class will be responsable for most of the games rendering requests. It will then send the requests to the suitable sub renderers.
      e.g, when the game requests text to be rendered on the screen, the renderer will send a request to the TextRenderer2D.
      e.g, when the game requests entity models to be rendered in the world, the renderer will send a request to the model draw function.
      This class also contains the projection matrix.*/
    class Renderer
    {
        private GameInstance gameInstance;
        private TextRenderer2D textRenderer2D;
        private Matrix4F projectionMatrix;
        private ModelDrawable quads;
        private Vector3F fogColour = ColourF.black.normalize();
        private Vector3F skyColour = ColourF.black.normalize();
        public Renderer(GameInstance game)
        {
            this.gameInstance = game;
        }
        
        /*Called before any rendering is done*/
        public void init()
        {
            gameInstance.MakeCurrent();
            textRenderer2D = new TextRenderer2D("consolasNative");
            textRenderer2D.addNewTextPanel("test", new String[] { "Lorem ipsum dolor sit amet,", "consectetur adipiscing elit,", "sed do eiusmod tempor incididunt ","ut labore et dolore magna aliqua." }, new Vector2F(0.0F,0.0F));
            textRenderer2D.addNewTextPanel("ayy",  "ABCDEFGHIJKLMNOPQRSTUVWXYZ", new Vector2F(0.0F,0.1F), ColourF.red, 2F);
            textRenderer2D.addNewTextPanel("ayylmao",  "abcdefghijklmnopqrstuvwxyz", new Vector2F(0.0F,0.15F), ColourF.green, 3F);
            textRenderer2D.addNewTextPanel("ayyxd",  "1234567890", new Vector2F(0.0F,0.2F), ColourF.lightBlue, 4F);
            textRenderer2D.addNewTextPanel("ayywtf",  "\"!`?'.,;:()[]{}<>|/@\\^&-%+=#_&~* ", new Vector2F(0.0F,0.25F), ColourF.yellow, 5F);
            textRenderer2D.addNewTextPanel("ayywtsf",  "HELLO WORLD.", new Vector2F(0.0F,0.35F), ColourF.orange, 5F);
            Renderer.setClearColor(skyColour);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.AlphaFunc(AlphaFunction.Greater, 0.01F);
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);



           // long constructStart = TicksAndFps.getMiliseconds();
            Model[] filler = new Model[32768];
            for(int z = 0; z < 32; z++ )
            {
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        filler[z * 1024 + x * 32 + y] = JaredsQuadPrefab.getNewModel().transformVertices(new Vector3F(1), new Vector3F(0,0,0), new Vector3F(x - 16, y, -z));
                    }
                }
            }
           // Application.debug("test mesh quad count: " + filler.Length);
            //Application.debug("test mesh triangle count: " + filler.Length * 2);
           // Application.debug("test mesh vertex count: " + filler.Length * 4);
           // Application.debug("test mesh took " + (TicksAndFps.getMiliseconds() - constructStart) + " miliseconds to construct.");
           // long batchStart = TicksAndFps.getMiliseconds();
            quads = QuadBatcher.batchQuadModels3D(filler, JaredsQuadPrefab.getShaderDir(), JaredsQuadPrefab.getTextureDir());
           // Application.debug("test mesh took " + (TicksAndFps.getMiliseconds() - batchStart) + " miliseconds to batch.");
        }

        /*Called each time the game window is resized*/
        public void onResize()
        {
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            textRenderer2D.onWindowResize();
        }

        /*Called before all draw calls*/
        private void preRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        
        public void renderAll()
        {
            preRender();
            renderGui();
            updateCameraAndRenderWorld();
            postRender();
        }

        private void updateCameraAndRenderWorld()
        {
            gameInstance.thePlayer.onCameraUpdate();
            quads.draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix, fogColour);
        }

        private void renderGui()
        {
            GL.Disable(EnableCap.CullFace);
            textRenderer2D.renderAnyText();
            GL.Enable(EnableCap.CullFace);
        }

        /*Called after all draw calls*/
        private void postRender()
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
    }
}
 