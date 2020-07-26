using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using FredsMath;
using System.Security.AccessControl;
using OpenTK.Graphics.OpenGL;
using FredrickTechDemo.src.Rendering.Models;
using FredrickTechDemo.src.Rendering;

namespace FredrickTechDemo
{
    class Renderer
    {
        private GameInstance gameInstance;
        private Shader shader;
        private Matrix4F projectionMatrix;
        public Renderer(GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
        }
        
        public void init()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            JaredsQuadModel.init();
            shader = new Shader(@"..\..\src\Rendering\Shaders\ColourTextureShader3D.shader");
            GL.Viewport(gameInstance.ClientRectangle);
            //identical
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), (float)gameInstance.Width / (float)gameInstance.Height, 0.001F, 1000.0F);
            
        }
        public void preRender(float r = 0, float g = 0, float b = 0)
        {
            GL.ClearColor(r, g, b, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public void renderJaredsQuad()
        {
            shader.Use();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", gameInstance.thePlayer.getCamera().getViewMatrix());
            shader.setUniformMat4F("modelMatrix", JaredsQuadModel.modelMatrix);// JaredsQuadModel.prevModelMatrix + (JaredsQuadModel.modelMatrix - JaredsQuadModel.prevModelMatrix) * (float)gameInstance.getPercentageNextTick());//interpolating model matrix between rotations 
            JaredsQuadModel.draw();
        }

        public void postRender()
        {
            gameInstance.SwapBuffers();
        }

        public void onResize()
        {
            GL.Viewport(gameInstance.ClientRectangle);
            projectionMatrix = Matrix4F.createPerspectiveMatrix((float)MathUtil.radians(GameSettings.fov), (float)gameInstance.Width / (float)gameInstance.Height, 0.001F, 1000.0F);
        }
    }
}
