using OpenTK.Graphics.OpenGL;
namespace RabbetGameEngine
{
    public class BufferTexture
    {
        private int texID;
        private int bufferID;
        private int byteSize;
        private SizedInternalFormat format;
        private BufferUsageHint bufferUsage;

        public BufferTexture(int byteSize, SizedInternalFormat format, BufferUsageHint bufferUsage)
        {
            this.byteSize = byteSize;
            this.format = format;
            this.bufferUsage = bufferUsage;
            bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.TextureBuffer, bufferID);
            GL.BufferData(BufferTarget.TextureBuffer, byteSize, System.IntPtr.Zero, bufferUsage);
            texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureBuffer, texID);
            GL.TexBuffer(TextureBufferTarget.TextureBuffer, format, bufferID);
            GL.BindBuffer(BufferTarget.TextureBuffer, 0);
        }

        /// <summary>
        /// Binds this texture buffer at provided index.
        /// This index can be shared with regular textures
        /// </summary>
        public void bind(int index = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.TextureBuffer, texID);
        }

        public void updateBuffer<T2>(T2[] data, int bytes) where T2 : struct
        {
            if (bufferUsage < BufferUsageHint.DynamicDraw || bytes > byteSize) return;
            GL.BindBuffer(BufferTarget.TextureBuffer, bufferID);
            GL.BufferSubData(BufferTarget.TextureBuffer, System.IntPtr.Zero, bytes, data);
        }


        public void delete()
        {
            GL.DeleteBuffer(bufferID);
            GL.DeleteTexture(texID);
        }

    }
}
