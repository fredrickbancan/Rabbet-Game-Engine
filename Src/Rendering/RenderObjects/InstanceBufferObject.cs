using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    public class InstanceBufferObject
    {
        public int id;

        public InstanceBufferObject()
        {
            id = GL.GenBuffer();
        }


        public void init<T2>(T2[] data, int typeByteSize) where T2 : struct
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * typeByteSize, data, BufferUsageHint.StaticDraw);
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
