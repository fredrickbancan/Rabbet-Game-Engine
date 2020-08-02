using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a mesh to be
      rendered.*/
    public class Model
    {
        protected Vertex[] vertices;
        protected UInt32[] indices;

        protected Model()
        {

        }
        public Model(Vertex[] vertices)
        {
            this.vertices = vertices;
        }
        public Model(Vertex[] vertices, UInt32[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }

        /*returns number of vertices in this model based on xyz coordinates*/
        public int getVertexCount()
        {
            return vertices.Length;
        }

        public Vertex[] getVertexArray()
        {
            return vertices;
        }


        /*sets all vertex colors to this color*/
        public Model setColor(ColourF color)
        {
            return this.setColor(color.normalVector4F());
        }
        public Model setColor(Vector4F color)
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].color = color;
            }
            return this;
        }
        
        /*Changes the vertices in the float arrays before rendering. Usefull for copying an already layed out model
          and batch rendering it in multiple different locations with different transformations.*/
        public Model transformVertices(Vector3F scale, Vector3F rotate, Vector3F translate)
        {
            for(int i = 0; i < vertices.Length; i ++)
            {
                Matrix4F.scaleXYZFloats(scale, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
                Matrix4F.rotateXYZFloats(rotate, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
                Matrix4F.translateXYZFloats(translate, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
            }
            return this;
        }
        public Model scaleVertices(Vector3F scale)
        {
            for (int i = 0; i < vertices.Length; i ++)
            {
                Matrix4F.scaleXYZFloats(scale, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
            }
            return this;
        }
        public Model rotateVertices(Vector3F rotate)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Matrix4F.rotateXYZFloats(rotate, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
            }
            return this;
        }
        public Model translateVertices(Vector3F translate)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Matrix4F.translateXYZFloats(translate, vertices[i].pos.x, vertices[i].pos.y, vertices[i].pos.z, out vertices[i].pos.x, out vertices[i].pos.y, out vertices[i].pos.z);
            }
            return this;
        }

    }
}
