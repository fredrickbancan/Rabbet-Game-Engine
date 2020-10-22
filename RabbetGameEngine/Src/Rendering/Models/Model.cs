using OpenTK;
using System;

namespace RabbetGameEngine.Models
{
    /*Model base class. This class is intended to hold the vertex, color, uv etc data for a model to be
      rendered. This has to be a class not a struct because we need to null check it*/
    public class Model
    {
        public Vertex[] vertices = null;
        public uint[] indices = null;
        public Matrix4 modelMatrix;
        public Matrix4 prevModelMatrix;
        public Model(Vertex[] vertices, uint[] indices = null)
        {
            this.vertices = vertices;
            this.indices = indices;
        }

        /*sets all vertex colors to this color*/
        public Model setColor(CustomColor color)
        {
            return this.setColor(color.toNormalVec4());
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

        /*generates a new model using copies of this models arrays.*/
        public Model copyModel()
        {
            Vertex[] verticesCopy = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopy, vertices.Length);
            return new Model(verticesCopy);
        }

        /// <summary>
        /// Used to set the ID for this set of data for when it will be drawn.
        /// </summary>
        /// <param name="f">ID of this object. Will be treated as integer index by shaders.</param>
        /// <returns>This (builder method)</returns>
        public Model setObjectID(float f)
        {
            for(int i = 0; i < vertices.Length; ++i)
            {
                vertices[i].objectID = f;
            }
            return this;
        }

        public Model setModelMatrix(Matrix4 m)
        {
            this.modelMatrix = m;
            return this;
        }

        public Model setPrevModelMatrix(Matrix4 m)
        {
            this.prevModelMatrix = m;
            return this;
        }

    }
}
