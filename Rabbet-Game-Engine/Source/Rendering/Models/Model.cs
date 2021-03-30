using OpenTK.Mathematics;
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
        public Vector3 worldPos;
        public Vector3 prevWorldPos;
        public Model(Vertex[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }

        public Model setTextureIndex(int id)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].textureIndex = id;
            }
            return this;
        }

        /*sets all vertex colors to this color*/
        public Model setColor(Color color)
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
            if(indices != null)
            {
                Vertex[] verticesCopya = new Vertex[vertices.Length];
                uint[] indicesCopy = new uint[indices.Length];
                Array.Copy(indices, indicesCopy, indices.Length);
                Array.Copy(vertices, verticesCopya, vertices.Length);
                return new Model(verticesCopya, indicesCopy).setModelPos(worldPos).setModelPrevPos(prevWorldPos);
            }
            Vertex[] verticesCopy = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopy, vertices.Length);
            return new Model(verticesCopy, null).setModelPos(worldPos).setModelPrevPos(prevWorldPos);
        }


        public Model setModelPos(Vector3 pos)
        {
            this.worldPos = pos;
            return this;
        }

        public Model setModelPrevPos(Vector3 pos)
        {
            this.prevWorldPos = pos;
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
