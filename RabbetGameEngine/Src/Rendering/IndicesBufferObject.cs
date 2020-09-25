using OpenTK.Graphics.OpenGL;
namespace RabbetGameEngine
{
    /*abstraction of opengl IBO*/
    public class IndicesBufferObject
    {
        //TODO: impliment
        private int id;

        public void delete()
        {
            GL.DeleteBuffer(id);
        }
    }
}
