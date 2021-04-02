using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    /// <summary>
    /// Simple static object for quickly rendering a full screen quad. Usefull for post processing.
    /// </summary>
    public static class FullScreenQuad
    {
        private static Vertex[] vertices;
        private static VertexArrayObject vao;

        public static void init()
        {
            vertices = new Vertex[]
            {
                new Vertex(-1F, -1F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F),
                new Vertex(1F, -1F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F),
                new Vertex(-1F, 1F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F),
                new Vertex(1F, 1F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F)
            };
            VertexBufferLayout vbl = new VertexBufferLayout();
            Vertex.configureLayout(vbl);
            vao = new VertexArrayObject();
            vao.beginBuilding();
            vao.addBuffer<Vertex>(vertices, Vertex.SIZE_BYTES, vbl);
            vao.finishBuilding();
        }

        public static void bindVao()
        {
            vao.bind();
        }

        /// <summary>
        /// Draws the full screen quad. A seperate shader and texture MUST be bound before calling this function.
        /// </summary>
        public static void draw()
        {
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        }

        public static void delete()
        {
            vao.delete();
        }
    }
}
