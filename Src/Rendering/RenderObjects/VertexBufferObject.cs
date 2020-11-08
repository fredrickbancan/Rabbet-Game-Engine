using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine
{
    public class VertexBufferObject
    {
        public int id;
        public bool isDynamic = false;

        public VertexBufferObject()
        {
            id = GL.GenBuffer();
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
            GL.BufferData(BufferTarget.ArrayBuffer, sizeToUpdate, data, BufferUsageHint.DynamicDraw);
        }

        public void bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        }

        public void delete()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(id);
        }
    }
}
