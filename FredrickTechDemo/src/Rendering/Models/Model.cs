using FredrickTechDemo.FredsMath;

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
        
        /*Changes the vertices in the float arrays before rendering. Usefull for copying an already layed out model
          and batch rendering it in multiple different locations with different transformations.*/
        public Model transformVertices(Vector3F scale, Vector3F rotate, Vector3F translate)
        {
            for(int i = 2; i < vertexXYZ.Length; i += 3)
            {
                Matrix4F.translateXYZFloats(translate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
                Matrix4F.rotateXYZFloats(rotate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
                Matrix4F.scaleXYZFloats(scale, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
            }
            return this;
        }

    }
}
