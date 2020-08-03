using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a model to be
      rendered. This has to be a class not a struct because we need to null check it*/
    public class Model
    {
        public Vertex[] vertices = null;
        public Model(Vertex[] vertices)
        {
            this.vertices = vertices;
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
