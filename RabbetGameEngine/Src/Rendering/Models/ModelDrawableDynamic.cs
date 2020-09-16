using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine.Models
{
    /*A model which can have information changed in buffers dynamically instead of statically set at creation.*/
    public class ModelDrawableDynamic : ModelDrawable
    {
        private int maxVertexCount;
        /*ModelDrawableDynamics must be initialized at construction because they can not have data submittied to them unless the VBO's are made*/
        public ModelDrawableDynamic(string shaderFile, string textureFile, uint[] indices, int maxVertexCount = 4000) : base( shaderFile, textureFile, null, indices)
        {
            this.maxVertexCount = maxVertexCount;
            init();
        }

        /*Create a VAO with buffers accodmodating the size of the maxvertexcount in vertex sizes. Array is null pointerbecase we will dynamically add data.*/
        protected override void storeVertexDataInAttributeList()
        {
            int vbo = GL.GenBuffer();
            VBOS.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            
            GL.BufferData(BufferTarget.ArrayBuffer, maxVertexCount * Vertex.vertexByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
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

        /*A method which can be called frequently to change the vertex data in the vbo of the provided index without re building the whole model*/
        public virtual void submitData(Vertex[] data, int vboIndex = 0)
        {
            if (data.Length > maxVertexCount)
            {
                Application.error("ModelDrawableDynamic.submitData() recieved array of vertices greater than the models max vertex amount!");
            }
            else
            {
                this.bindVertexBuffer(vboIndex);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * Vertex.vertexByteSize, data);
            }
        }
    }
}
