using FredrickTechDemo.FredsMath;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.Models
{
    /*Base class for models that wont be batched and can be drawn individually with additional draw calls and have
     *individual VAO's.*/
    public class ModelDrawable : Model
    {
        protected UInt32[] indices;
        protected bool hasInitialized = false;
        protected int indicesBufferObject;
        protected List<int> VBOS = new List<int>();
        protected int VAO;
        protected Texture texture;
        protected Shader shader;
        protected Matrix4F modelMatrix = new Matrix4F(1.0F);
        protected Matrix4F prevModelMatrix = new Matrix4F(1.0F);


        /*takes in directory for the shader and texture for this model*/
        public ModelDrawable(String shaderFile, String textureFile, Vertex[] vertices, UInt32[] indices) : base(vertices)
        {
            this.indices = indices;
            texture = new Texture(textureFile, false);
            shader = new Shader(shaderFile);
        }

        public virtual void setUniformVec3(String name, Vector3F vec)
        {
            shader.setUniformVec3F(name, vec);
        }
        /*called when model is first bound. can be used for more*/
        protected virtual void init()
        {
            hasInitialized = true;
            genBuffers();
        }

        /*Generates the attributes for each array of data and stores them in this model's VAO.*/
        protected virtual void genBuffers()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            
            genAndBindIndices(); //for indices
            storeVertexDataInAttributeList();
        }

        /*Binds the models texture and shader, can be used for more*/
        public virtual void bind()
        {
            if (!hasInitialized)
            {
                init();
            }
            else
            {
                GL.BindVertexArray(VAO);//must be bound first before indices
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
            texture.use();
            shader.use();
        }

        /*Draws this model. If its the first draw call, and firtst bind call, the model will be initialized.*/
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColour)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", prevModelMatrix + (modelMatrix - prevModelMatrix) * TicksAndFps.getPercentageToNextTick());//interpolating model matrix between ticks
            shader.setUniformVec3F("fogColour", fogColour);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", prevModelMatrix + (modelMatrix - prevModelMatrix) * TicksAndFps.getPercentageToNextTick());//interpolating model matrix between ticks

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }
        public virtual void draw()
        {
            bind();
            GL.DrawElements(PrimitiveType.LineLoop, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }

        /*Unbinds this model so the renderer can render different things.*/
        public virtual void unBind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        /*Binds the indicie buffer to the VAO*/
        protected virtual void genAndBindIndices()
        {
            indicesBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(UInt32), indices, BufferUsageHint.StaticDraw);
        }

        /*Binds the provided data to the VAO using the provided information*/
        [Obsolete("This method is depricated as now the system works with Vertex objects instead of float arrays. Use storeVertexDataInAttributeList() instead.")]
        protected virtual void storeDataInAttributeList(int attributeNumber, int coordinateCount, float[] data)
        {
            int vbo = GL.GenBuffer();
            VBOS.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);//TODO store buffer id's in an array so they can be deleted on model unload
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(attributeNumber);
            GL.VertexAttribPointer(attributeNumber, coordinateCount, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected virtual void storeVertexDataInAttributeList()
        {
            int vbo = GL.GenBuffer();
            VBOS.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);//TODO store buffer id's in an array so they can be deleted on model unload
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.vertexByteSize, vertices, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, Vertex.positionLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.positionOffset);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, Vertex.colorLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.colorOffset);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, Vertex.uvLength, VertexAttribPointerType.Float, false, Vertex.vertexByteSize, Vertex.uvOffset);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        /*Used for binding the vertex buffer object in the array at the given index, any vbo's get unbound in unbind()*/
        public virtual void bindVertexBuffer(int index = 0)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOS[0]);
        }

        /*Called when this model is no longer needed and will be replaced*/
        public virtual void delete()
        {
            GL.DeleteVertexArray(VAO);
            foreach (int vbo in VBOS)
            {
                GL.DeleteBuffer(vbo);
            }
            shader.Dispose();
            texture.Dispose();
        }
    }
}
