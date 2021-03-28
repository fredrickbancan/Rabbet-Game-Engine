using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace RabbetGameEngine
{
    public struct Vertex
    {
        public Vector3 pos;
        public Vector4 color;
        public Vector2 uv;

        /*these are only in bytes to save memory. They shouldnt ever get even close to 256*/
        public static readonly byte positionLength = 3;
        public static readonly byte colorLength = 4;
        public static readonly byte uvLength = 2;
        public static readonly byte positionSize = 3 * sizeof(float);
        public static readonly byte colorSize = 4 * sizeof(float);
        public static readonly byte uvSize= 2 * sizeof(float);
        public static readonly byte positionOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "pos");
        public static readonly byte colorOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "color");
        public static readonly byte uvOffset = (byte)Marshal.OffsetOf(typeof(Vertex), "uv");
        public static readonly int SIZE_BYTES = positionSize + colorSize + uvSize;

        public Vertex(float x, float y, float z, float r, float g, float b, float a, float u, float v)
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
        }
        public Vertex(Vector3 pos, Vector4 color, Vector2 uv)
        {
            this.pos = pos;
            this.color = color;
            this.uv = uv;
        }

        public static void configureLayout(VertexBufferLayout l)
        {
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 3);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 4);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 2);
        }
    }
}
