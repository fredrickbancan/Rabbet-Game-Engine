using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    /*Abstraction of a VAO for use with rendering.*/
    public class VertexArrayObject
    {
        //TODO: impliment
        private int id;
        private VertexBufferObject vbo;
        private IndicesBufferObject ibo;
        private bool dynamic = false;
        private bool hasInitialized = false;
        private BatchType type;
        private Vertex[] verticesReference;
        private uint[] indicesReference;

        public VertexArrayObject(BatchType type, Vertex[] vertices, uint[] indices)//for static 
        {
            if((int)type > (int)BatchType.lines)//if type requires dynamic VAO
            {
                Application.warn("VAO is being initialized as static with a BatchType which requires a dynamic VAO!");
            }

            this.type = type;
            verticesReference = vertices;
            indicesReference = indices;
        }
        public VertexArrayObject(BatchType type, int maxVertices)//for dynamic
        {
            this.type = type;
        }

        //can be called to initialize this vao with the provided data
        //this binds the vao
        //if vao is dynamic, null can be passed.
        private void initialize(Vertex[] vertices, uint[] indices)
        {
            if (hasInitialized) return;

            //TODO: impliment

            hasInitialized = true;
        }

        private void initializeDynamic(int maxVertices)
        {
            if (hasInitialized) return;

            //TODO: impliment

            hasInitialized = true;
        }

        public void bind()
        {
            if(!hasInitialized)
            {
                return;
            }
            GL.BindVertexArray(id);
        }

        public bool isDynamic()
        {
            return dynamic;
        }

        /*deallocates this vao. Should be called when deconstructing any parent object.*/
        public void delete()
        {
            vbo.delete();
            ibo.delete();
            GL.DeleteVertexArray(id);
        }
//##############################################################################################################################################
        /*abstraction of opengl VBO*/
        private class VertexBufferObject
        {
            private int id;
            //TODO: impliment

            public void delete()
            {
                GL.DeleteBuffer(id);
            }
        }
//##############################################################################################################################################
        /*abstraction of opengl IBO*/
        private class IndicesBufferObject
        {
            //TODO: impliment
            private int id;

            public void delete()
            {
                GL.DeleteBuffer(id);
            }
        }
    }
}
