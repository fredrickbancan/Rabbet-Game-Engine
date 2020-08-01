using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a mesh to be
      rendered.*/
    public class Model
    {
        protected float[] vertexXYZ;
        protected float[] vertexRGBA;
        protected float[] vertexUV;
        protected UInt32[] indices;

        protected Model()
        {

        }
        public Model(float[] vertexPositions, float[] vertexColour, float[] vertexUV)
        {
            this.vertexXYZ = vertexPositions;
            this.vertexRGBA = vertexColour;
            this.vertexUV = vertexUV;
        }
        public Model(float[] vertexPositions, float[] vertexColour, float[] vertexUV, UInt32[] indices)
        {
            this.vertexXYZ = vertexPositions;
            this.vertexRGBA = vertexColour;
            this.vertexUV = vertexUV;
            this.indices = indices;
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

        public float[] getVertexRGBA()
        {
            return vertexRGBA;
        }

        public float[] getVertexUV()
        {
            return vertexUV;
        }

        /*sets all vertex colors to this color*/
        public Model setColor(ColourF color)
        {
            return this.setColor(color.normalize());
        }
        public Model setColor(Vector3F color)
        {
            for(int i = 2; i < vertexRGBA.Length; i+=3)
            {
                vertexRGBA[i - 2] = color.x;
                vertexRGBA[i - 1] = color.y;
                vertexRGBA[i - 0] = color.z;
            }
            return this;
        }
        
        /*Changes the vertices in the float arrays before rendering. Usefull for copying an already layed out model
          and batch rendering it in multiple different locations with different transformations.*/
        public Model transformVertices(Vector3F scale, Vector3F rotate, Vector3F translate)
        {
            for(int i = 2; i < vertexXYZ.Length; i += 3)
            {
                Matrix4F.scaleXYZFloats(scale, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
                Matrix4F.rotateXYZFloats(rotate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
                Matrix4F.translateXYZFloats(translate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i   ], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i   ]);
            }
            return this;
        }
        public Model scaleVertices(Vector3F scale)
        {
            for (int i = 2; i < vertexXYZ.Length; i += 3)
            {
                Matrix4F.scaleXYZFloats(scale, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i]);
            }
            return this;
        }
        public Model rotateVertices(Vector3F rotate)
        {
            for (int i = 2; i < vertexXYZ.Length; i += 3)
            {
                Matrix4F.rotateXYZFloats(rotate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i]);
            }
            return this;
        }
        public Model translateVertices(Vector3F translate)
        {
            for (int i = 2; i < vertexXYZ.Length; i += 3)
            {
                Matrix4F.translateXYZFloats(translate, vertexXYZ[i - 2], vertexXYZ[i - 1], vertexXYZ[i], out vertexXYZ[i - 2], out vertexXYZ[i - 1], out vertexXYZ[i]);
            }
            return this;
        }

    }
}
