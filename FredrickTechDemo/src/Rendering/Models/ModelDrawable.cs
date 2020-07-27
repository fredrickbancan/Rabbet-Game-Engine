using FredrickTechDemo.FredsMath;
using OpenTK.Graphics.OpenGL;
using System;


namespace FredrickTechDemo.Models
{
    /*Base class for models that wont be batched and can be drawn individually with additional draw calls and have
     *individual VAO's.*/
    class ModelDrawable : Model
    {
        protected bool hasInitialized = false;
        private int indicesBufferObject;
        private int VAO;
        protected Texture texture;
        protected Shader shader;
        protected Matrix4F prevModelMatrix;
        protected Matrix4F modelMatrix;


        /*takes in directory for the shader and texture for this model*/
        protected ModelDrawable(String shaderFile, String textureFile, float[] vertexPositions, float[] vertexColour, float[] vertexUV, UInt32[] indices) : base(vertexPositions, vertexColour, vertexUV, indices)
        {
            texture = new Texture(textureFile, true);
            shader = new Shader(shaderFile);
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
            
            bindIndicesBuffer(); //for indices
            storeDataInAttributeList(0, 3, vertexXYZ);//for X,y,z coords
            storeDataInAttributeList(1, 3, vertexRGB);//for rgb
            storeDataInAttributeList(2, 2, vertexUV);//for uv
        }

        /*Binds the models texture and shader, can be used for more*/
        protected virtual void bind()
        {
            if (!hasInitialized) 
            { 
               init();
            }
            GL.BindVertexArray(VAO);//must be bound first before indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
            texture.use();
            shader.use();
        }

        /*Draws this model. If its the first draw call, and firtst bind call, the model will be initialized.*/
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix)
        {
            bind();
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformMat4F("modelMatrix", prevModelMatrix + (modelMatrix - prevModelMatrix) * TicksAndFps.getPercentageToNextTick());//interpolating model matrix between ticks

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            unBind();
        }

        /*Unbinds this model so the renderer can render different things.*/
        protected virtual void unBind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        /*Binds the indicie buffer to the VAO*/
        private void bindIndicesBuffer()
        {
            indicesBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(UInt32), indices, BufferUsageHint.StaticDraw);
        }

        /*Binds the provided data to the VAO using the provided information*/
        private void storeDataInAttributeList(int attributeNumber, int coordinateCount, float[] data)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, GL.GenBuffer());//TODO store buffer id's in an array so they can be deleted on model unload
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            /*Stride: the size in bytes between the start of one vertex to the start of another.
              Offset: the size in byts between the start of the vertex to the first bit of information for this specific attribute.*/
            GL.VertexAttribPointer(attributeNumber, coordinateCount, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(attributeNumber);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
