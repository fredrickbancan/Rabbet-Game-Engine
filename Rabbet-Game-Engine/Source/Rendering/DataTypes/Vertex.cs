using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace RabbetGameEngine
{
    public struct Vertex
    {
        public Vector3 pos;
        public Vector4 color;
        public Vector2 uv;
        public float textureIndex;

        /*these are only in bytes to save memory. They shouldnt ever get even close to 256*/
        public static readonly byte positionLength = 3;
        public static readonly byte colorLength = 4;
        public static readonly byte uvLength = 2;
        public static readonly byte positionSize = 3 * sizeof(float);
        public static readonly byte colorSize = 4 * sizeof(float);
        public static readonly byte uvSize= 2 * sizeof(float);
        public static readonly byte textureIndexSize= sizeof(float);
        public static readonly byte positionOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "pos");
        public static readonly byte colorOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "color");
        public static readonly byte uvOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "uv");
        public static readonly byte textureIndexOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "textureIndex");
        public static readonly int SIZE_BYTES = positionSize + colorSize + uvSize + textureIndexSize;

        public Vertex(float x, float y, float z, float r, float g, float b, float a, float u, float v, int texIndex = 0)
        {
            pos.X = x;
            pos.Y = y;
            pos.Z = z;
            color.X = r;
            color.Y = g;
            color.Z = b;
            color.W = a;
            uv.X = u;
            uv.Y = v;
            textureIndex = texIndex;
        }
        public Vertex(Vector3 pos, Vector4 color, Vector2 uv, int texIndex = 0)
        {
            this.pos = pos;
            this.color = color;
            this.uv = uv;
            textureIndex = texIndex;
        }

        public static void configureLayout(VertexBufferLayout l)
        {
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 3);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 4);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 2);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 1);
        }
    }
}
