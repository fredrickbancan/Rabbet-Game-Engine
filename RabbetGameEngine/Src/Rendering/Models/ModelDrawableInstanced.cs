using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine.Models
{
    /*This class is for rendering the same model data many times with different transforms, colors and textures.
      It is more efficient to use this instead of ModelDrawableDynamic if its just multiple instances of one mesh.*/
    public class ModelDrawableInstanced
    {
        /*this array of shader directories can be indexed with the ModelDrawType enum.
          each shader in this array must be a shader specifically made for interpolating a dynamic
          model. */
        /*  private static string[] shaders = new string[] {
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.triangles 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.points 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.singlePoint 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.lines 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.billboardSpherical 
              ResourceUtil.getShaderFileDir("")//ModelDrawType.billboardCylindrical 
          };*/
        protected int maxInstances;
        protected bool hasInitialized = false;
        protected int vertexAOID;
        protected int vertexBOID;
        protected int indicesBOID;
        protected int matricesBOID;
        protected Texture texture;
        protected Shader shader;
        protected Vertex[] vertices;
        protected uint[] indices;
        protected Matrix4[] matrices;
        protected int matricesItterator = 0;
        protected ModelDrawType drawType;

         //TODO: set shader based on draw type (ModelDrawableInstanced)
        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(Vertex[] vertices, uint[] indices, ModelDrawType drawType, int maxInstances = 1000)
        {
            this.indices = new uint[indices.Length];
            this.vertices = new Vertex[vertices.Length];
            Array.Copy(indices, this.indices, indices.Length);
            Array.Copy(vertices, this.vertices, vertices.Length);
            this.drawType = drawType;
            this.maxInstances = maxInstances;
            matrices = new Matrix4[maxInstances];
            ShaderUtil.tryGetShader("ColorTextureFogInstanced3D.shader", out shader);
            TextureUtil.tryGetTexture("sand.png", out texture);
        }

        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(ModelDrawable instance, ModelDrawType drawType, int maxInstances = 1000)
        {
            indices = new uint[instance.indices.Length];
            vertices = new Vertex[instance.vertices.Length];
            Array.Copy(instance.indices, indices, instance.indices.Length);
            Array.Copy(instance.vertices, vertices, instance.vertices.Length);
            this.drawType = drawType;
            this.maxInstances = maxInstances;
            matrices = new Matrix4[maxInstances];
            ShaderUtil.tryGetShader("ColorTextureFogInstanced3D.shader", out shader);
            TextureUtil.tryGetTexture("sand.png", out texture);
        }

        protected virtual void init()
        {
            vertexAOID = GL.GenVertexArray();
            GL.BindVertexArray(vertexAOID);

            indicesBOID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBOID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

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


            //matrices
            matricesBOID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, matricesBOID);
            GL.BufferData(BufferTarget.ArrayBuffer, maxInstances * sizeof(float) * 16, matrices, BufferUsageHint.StaticDraw);
          
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 16, VertexAttribPointerType.Float, false, maxInstances * sizeof(float) * 16, 0);
            GL.VertexAttribDivisor(3, 1);

            
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
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBOID);
            // GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBOID);
        }

        /*when called adds a transform to the list and when this model is drawn, an
          instance will be drawn with the provided transform.*/
        public virtual void addRenderAt(Matrix4 transform)
        {
            matrices[matricesItterator++] = transform;
        }

        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            bind();
            texture.use();
            shader.use();
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformVec3F("fogColor", fogColor);

            switch (drawType)
            {
                case ModelDrawType.points:
                    GL.DrawArraysInstanced(PrimitiveType.Points, 0, vertices.Length, matricesItterator);
                    break;

                case ModelDrawType.lines:
                    GL.DrawElementsInstanced(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, System.IntPtr.Zero, matricesItterator);
                    break;

                default:
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, System.IntPtr.Zero, matricesItterator);
                    break;
            }
        }

        /*Can be called to prepare for instance requests from other parts of the game engine.*/
        public virtual void prepare()
        {
            //this must be called so any matrices that arent requested wont exist in the array for the next draw call. Prevents rendering ghost models.
            clearTransforms();
        }

        protected virtual void clearTransforms()
        {
            Array.Clear(matrices, 0, matricesItterator - 1);
            matricesItterator = 0;
        }

        public virtual void delete()
        {
            GL.DeleteBuffer(indicesBOID);
            GL.DeleteBuffer(matricesBOID);
            GL.DeleteBuffer(vertexBOID);
            GL.DeleteVertexArray(vertexAOID);
        }
        
    }
}
