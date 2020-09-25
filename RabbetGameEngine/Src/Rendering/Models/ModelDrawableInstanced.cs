using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine.Models
{
    /*This class is for rendering the same model data many times with different transforms, colors and textures.
      It is more efficient to use this instead of ModelDrawableDynamic if its just multiple instances of one mesh.
      ModelDrawableInstanced should only be used when repeatidly rendering the same mesh as triangles. lines and points
      benefit more from dynamic rendering instead of instanced.*/
    public class ModelDrawableInstanced
    {
        public static readonly string shaderName = "EntityWorld_FTI";
        protected int maxInstances;
        protected bool hasInitialized = false;
        protected bool needsToSubmit = true;
        protected int vertexAOID;
        protected int vertexBOID;
        protected int matricesBOID;
        protected int indicesBOID;
        protected Texture texture;
        protected Shader shader;
        protected Vertex[] vertices;
        protected uint[] indices;
        protected Matrix4[] matrices;
        protected int matricesItterator = 0;

         
        /*the provided vertices and indices will be re-drawn at each transform*/
        public ModelDrawableInstanced(Vertex[] vertices, uint[] indices, int maxInstances = 1000)
        {
            this.indices = new uint[indices.Length];
            this.vertices = new Vertex[vertices.Length];
            Array.Copy(indices, this.indices, indices.Length);
            Array.Copy(vertices, this.vertices, vertices.Length);
            this.maxInstances = maxInstances;
            matrices = new Matrix4[maxInstances];
            TextureUtil.tryGetTexture("debug", out texture);
            ShaderUtil.tryGetShader(shaderName, out shader);
        }

        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(ModelDrawable instance, int maxInstances = 1000)
        {
            indices = new uint[instance.indices.Length];
            vertices = new Vertex[instance.vertices.Length];
            Array.Copy(instance.indices, indices, instance.indices.Length);
            Array.Copy(instance.vertices, vertices, instance.vertices.Length);
            this.maxInstances = maxInstances;
            matrices = new Matrix4[maxInstances];
            TextureUtil.tryGetTexture("debug", out texture);
            ShaderUtil.tryGetShader(shaderName, out shader);
        }

        protected virtual void init()
        {
            vertexAOID = GL.GenVertexArray();
            GL.BindVertexArray(vertexAOID);
            
            vertexBOID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBOID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.vertexByteSize, vertices, BufferUsageHint.StaticDraw);
            
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);

            matricesBOID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, matricesBOID);
            GL.BufferData(BufferTarget.ArrayBuffer, matrices.Length * 4 * 16, matrices, BufferUsageHint.DynamicDraw);
          
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false,  4 * 16, 0);
            GL.VertexAttribDivisor(3, 1);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 4 * 16, 16);
            GL.VertexAttribDivisor(4, 1);

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 4 * 16, 32);
            GL.VertexAttribDivisor(5, 1);

            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, 4 * 16, 48);
            GL.VertexAttribDivisor(6, 1);


            indicesBOID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBOID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


            hasInitialized = true;
        }
        protected virtual void bind()
        {
            if (!hasInitialized)
            {
                init();
            }
            else
            {
                GL.BindVertexArray(vertexAOID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBOID);
            }
        }

        /*when called adds a transform to the list and when this model is drawn, an
          instance will be drawn with the provided transform.*/
        public virtual void addRenderAt(Matrix4 transform)
        {
            matrices[matricesItterator++] = transform;
        }

        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            if(matricesItterator < 1)
            {
                return;
            }

            if(needsToSubmit)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, matricesBOID);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, matrices.Length * 4 * 16, matrices);
                needsToSubmit = false;
            }

            bind();
            texture.use();
            shader.use();
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniform1I("frame", Renderer.frame);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, System.IntPtr.Zero, matricesItterator);
            Renderer.totalDraws++;

        }

        /*Can be called to prepare for instance requests from other parts of the game engine.*/
        public virtual void prepare()
        {
            //this must be called so any matrices that arent requested wont exist in the array for the next draw call. Prevents rendering ghost models.
            clearTransforms();
            needsToSubmit = true;
        }

        protected virtual void clearTransforms()
        {
            if (matricesItterator >= 1)
            {
                Array.Clear(matrices, 0, matricesItterator - 1);
                matricesItterator = 0;
            }
        }

        public virtual ModelDrawableInstanced setTexture(Texture tex)
        {
            this.texture = tex;
            return this;
        }
        public virtual void delete()
        {
            GL.DeleteBuffer(indicesBOID);
            GL.DeleteBuffer(vertexBOID);
            GL.DeleteBuffer(matricesBOID);
            GL.DeleteVertexArray(vertexAOID);
        }
        
    }
}
