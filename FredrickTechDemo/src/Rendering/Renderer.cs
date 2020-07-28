using FredrickTechDemo.FredsMath;
using OpenTK.Graphics.OpenGL;

namespace FredrickTechDemo
{
    /*This class will be responsable for most of the games rendering requests. It will then send the requests to the suitable sub renderers.
      e.g, when the game requests text to be rendered on the screen, the renderer will send a request to the TextRenderer2D.
      e.g, when the game requests entity models to be rendered in the world, the renderer will send a request to the model draw function.
      This class also contains the projection matrix.*/
    class Renderer
    {
        private GameInstance gameInstance;
        private Matrix4F projectionMatrix;
        public Renderer(GameInstance game)
        {
            this.gameInstance = game;
        }
        
        /*Called before any rendering is done*/
        public void init()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), (float)gameInstance.Width / (float)gameInstance.Height, 0.001F, 1000.0F);
        }

        /*Called each time the game window is resized*/
        public void onResize()
        {
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), (float)gameInstance.Width / (float)gameInstance.Height, 0.001F, 1000.0F);
        }

        /*Called before each draw call*/
        public void preRender(float r = 0, float g = 0, float b = 0)
        {
            GL.ClearColor(r, g, b, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        
        /*Called after each draw call*/
        public void postRender()
        {
            gameInstance.SwapBuffers();
        }

        /*render jareds quad*/
        public void renderJaredsQuad() 
        {
            preRender();
            gameInstance.thePlayer.onCameraUpdate();
            gameInstance.jaredsQuad.draw(gameInstance.thePlayer.getCamera().getViewMatrix(), projectionMatrix);
            postRender();
        }

        
    }
}
 