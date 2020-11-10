using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine
{
    public class IndexBufferObject
    {
        public int id;
        public bool isDynamic = false;

        public IndexBufferObject()
        {
            id = GL.GenBuffer();
        }

        public void initStatic(uint[] indices)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }

        public void initDynamic(int initialByteSize)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, initialByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            isDynamic = true;
        }

        public void updateData(uint[] data, int count)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(uint), data);
        }

        /// <summary>
        /// resizes indices buffer, removes all data currently in buffer.
        /// </summary>
        public void resizeBuffer(int count)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, count * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
        }

        public void delete()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(id);
        }
    }
}
