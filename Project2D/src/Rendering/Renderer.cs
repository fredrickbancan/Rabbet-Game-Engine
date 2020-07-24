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
        public Renderer(GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
        }
        
        public void init()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            JaredsQuadModel.genBuffers();
            shader = new Shader("..\\..\\src\\Rendering\\Shaders\\ColourShader.shader");
        }
        public void preRender(float r = 0, float g = 0, float b = 0)
        {
            GL.ClearColor(r, g, b, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
        }
        public void renderJaredsQuad()
        {
            JaredsQuadModel.draw(shader);
        }

        public void postRender()
        {
            gameInstance.SwapBuffers();
        }
    }
}
