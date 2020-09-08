using OpenTK;
using System.Drawing;

namespace Coictus.Models
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
        public Model setColor(Color color)
        {
            return this.setColor(MathUtil.colorToNormalVec4(color));
        }
        public Model setColor(Vector4 color)
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].color = color;
            }
            return this;
        }
        
        /*Changes the vertices in the float arrays before rendering. Usefull for copying an already layed out model
          and batch rendering it in multiple different locations with different transformations.*/
        public Model transformVertices(Vector3 scale, Vector3 rotate, Vector3 translate)
        {
            for(int i = 0; i < vertices.Length; i ++)
            {
                MathUtil.scaleXYZFloats(scale, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
                MathUtil.rotateXYZFloats(rotate, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
                MathUtil.translateXYZFloats(translate, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
            }
            return this;
        }
        public Model scaleVertices(Vector3 scale)
        {
            for (int i = 0; i < vertices.Length; i ++)
            {
                MathUtil.scaleXYZFloats(scale, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
            }
            return this;
        }
        public Model rotateVertices(Vector3 rotate)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                MathUtil.rotateXYZFloats(rotate, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
            }
            return this;
        }
        public Model translateVertices(Vector3 translate)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                MathUtil.translateXYZFloats(translate, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
            }
            return this;
        }

        public Model scaleUV(Vector2 scale)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].uv *= scale;
            }
            return this;
        }

        public Model scaleVerticesAndUV(Vector3 scale)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                MathUtil.scaleXYZFloats(scale, vertices[i].pos.X, vertices[i].pos.Y, vertices[i].pos.Z, out vertices[i].pos.X, out vertices[i].pos.Y, out vertices[i].pos.Z);
                vertices[i].uv.X *= scale.X;
                vertices[i].uv.Y *= scale.Z;
            }
            return this;
        }

    }
}
