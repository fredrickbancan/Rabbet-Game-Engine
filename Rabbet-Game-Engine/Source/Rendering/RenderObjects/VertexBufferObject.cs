using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine
{
    public class VertexBufferObject
    {
        public int id;
        public bool isDynamic = false;
        public VertexBufferLayout layout;

        public VertexBufferObject(VertexBufferLayout l)
        {
            id = GL.GenBuffer();
            layout = l;
        }

        public void initStatic<T2>(T2[] data, int typeByteSize) where T2 : struct
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * typeByteSize, data, BufferUsageHint.StaticDraw);
        }

        public void initDynamic(int initialByteSize)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, initialByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            isDynamic = true;
        }

        public void updateBuffer<T2>(T2[] data, int sizeToUpdate) where T2 : struct
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeToUpdate, data);
        }
        /// <summary>
        /// Must be called to resize the vbo before submitting a larger array of data.
        /// This function will clear all data currently in the vbo.
        /// </summary>
        public void resizeBuffer(int newSizeBytes)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, newSizeBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        }

        public void delete()
        {
            GL.DeleteBuffer(id);
        }
    }
}
