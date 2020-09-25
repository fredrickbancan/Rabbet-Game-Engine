using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    /*abstraction of opengl VBO*/
    public class VertexBufferObject
    {
        private int id;
        //TODO: impliment

        public void delete()
        {
            GL.DeleteBuffer(id);
        }
    }
}
