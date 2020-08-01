using OpenTK.Graphics.OpenGL;
using System;

namespace FredrickTechDemo.Models
{
    /*This class represents a 2D GUI Model consisting of quads which have been batched together.*/
    class ModelDrawableGUI : ModelDrawable
    {
        /*takes in directory for the shader and texture for this model*/
        public ModelDrawableGUI(String shaderFile, String textureFile, float[] vertexPositions, float[] vertexColour, float[] vertexUV, UInt32[] indices) : base(shaderFile, textureFile, vertexPositions, vertexColour, vertexUV, indices)
        {
        }

        /*Draws this model. If its the first draw call, and firtst bind call, the model will be initialized.*/
        public virtual void draw()
        {
            bind();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
    }
}