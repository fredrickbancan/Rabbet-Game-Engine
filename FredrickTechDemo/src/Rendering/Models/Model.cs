namespace FredrickTechDemo.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a mesh to be
      rendered.*/
    class Model
    {
        protected float[] vertexXYZ;
        protected float[] vertexRGB;
        protected float[] vertexUV;

        public Model(float[] vertexPositions, float[] vertexColour, float[] vertexUV)
        {
            this.vertexXYZ = vertexPositions;
            this.vertexRGB = vertexColour;
            this.vertexUV = vertexUV;
        }

        /*returns number of vertices in this model based on xyz coordinates*/
        public int getVertexCount()
        {
            return vertexXYZ.Length / 3;
        }

        public float[] getVertexXYZ()
        {
            return vertexXYZ;
        }

        public float[] getVertexRGB()
        {
            return vertexRGB;
        }

        public float[] getVertexUV()
        {
            return vertexUV;
        }
    }
}
