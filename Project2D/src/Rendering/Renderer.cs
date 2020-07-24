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

namespace FredrickTechDemo
{
    class Renderer
    {
        private GameInstance gameInstance;
        public Renderer(GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
        }
        
        public void init()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            JaredsQuadModel.genBuffers();
            GL.FrontFace(FrontFaceDirection.Ccw);
        }
        public void preRender(Colour clearColour)
        {
            /*opengl requires a value between 0 and 1 for its colors, so i use my normalize function to convert the values.*/
           // GL.ClearColor(MathUtil.normalize(0, 255, clearColour.GetRed()), MathUtil.normalize(0, 255, clearColour.GetGreen()), MathUtil.normalize(0, 255, clearColour.GetBlue()), MathUtil.normalize(0, 255, clearColour.GetAlpha()));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
        }
        public void renderJaredsQuad()
        {
            JaredsQuadModel.draw();
        }

        public void postRender()
        {
            gameInstance.SwapBuffers();
        }
    }
}
