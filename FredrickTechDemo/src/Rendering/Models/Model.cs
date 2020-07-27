using System;

namespace FredrickTechDemo.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a mesh to be
      rendered. The batch renderer will make a model to store data. */
    class Model
    {
        protected float[] vertexXYZ;
        protected float[] vertexRGB;
        protected float[] vertexUV;
        protected UInt32[] indices;

        public Model(float[] vertexPositions, float[] vertexColour, float[] vertexUV, UInt32[] indices)
        {
            this.vertexXYZ = vertexPositions;
            this.vertexRGB = vertexColour;
            this.vertexUV = vertexUV;
            this.indices = indices;
        }
    }
}
