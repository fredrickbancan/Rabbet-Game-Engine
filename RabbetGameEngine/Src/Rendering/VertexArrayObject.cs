using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    /*Abstraction of a VAO for use with rendering.*/
    public class VertexArrayObject
    {
        //TODO: impliment
        private int id;
        private VertexBufferObject vbo;
        private IndicesBufferObject ibo;

        public VertexArrayObject()
        {

        }

        public void delete()
        {
            vbo.delete();
            ibo.delete();
            GL.DeleteVertexArray(id);
        }
    }
}
