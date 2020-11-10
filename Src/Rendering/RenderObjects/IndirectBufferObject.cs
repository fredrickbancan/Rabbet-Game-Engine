using OpenTK.Graphics.OpenGL;
using System;

namespace RabbetGameEngine
{
    public class IndirectBufferObject
    {
        public int id;

        public IndirectBufferObject()
        {
            id = GL.GenBuffer();
        }

        public void init(int initialByteSize)
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, id);
            GL.BufferData(BufferTarget.DrawIndirectBuffer, initialByteSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void updateData(DrawCommand[] data, int count)
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, id);
            GL.BufferSubData(BufferTarget.DrawIndirectBuffer, IntPtr.Zero, count * DrawCommand.sizeInBytes, data);
        }

        public void resizeBuffer(int newCount)
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, id);
            GL.BufferData(BufferTarget.DrawIndirectBuffer, newCount * DrawCommand.sizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void bind()
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, id);
        }

        public void delete()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(id);
        }
    }
}
