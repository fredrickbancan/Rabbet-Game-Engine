using OpenTK;
using System;

namespace RabbetGameEngine.Models
{
    /// <summary>
    /// a collection of model data (vertices, indices, model matrix and previous model matix.)
    /// </summary>
    public class Model
    {
        public Vertex[] vertices = null;
        public uint[] indices = null;
        public Matrix4 modelMatrix = Matrix4.Identity;
        public Matrix4 prevModelMatrix = Matrix4.Identity;
        public Model(Vertex[] vertices, uint[] indices)
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
        
        /// <summary>
        /// transforms the vertices of this model by the provided model matrix.
        /// This must be used for models which will be drawn without sending the model matrix to the shader.
        /// I.E: Non-lerp batch types.
        /// </summary>
        /// <param name="modelMatrix">The transformation matrix</param>
        /// <returns>this (builder method)</returns>
        public Model transformVertices(Matrix4 modelMatrix)
        {
            for(int i = 0; i < vertices.Length; ++i)
            {
                vertices[i].pos = Vector3.TransformPerspective(vertices[i].pos, modelMatrix);
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
            uint[] indicesCopy = new uint[indices.Length];
            Array.Copy(indices, indicesCopy, indices.Length);
            Array.Copy(vertices, verticesCopy, vertices.Length);
            return new Model(verticesCopy, indicesCopy);
        }

        /// <summary>
        /// Used to set the ID for this set of data for when it will be drawn.
        /// This must be used as an identifier for each vertex of the individual objects
        /// in a batch so the shader can determine which model matrices to use to transform it.
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
