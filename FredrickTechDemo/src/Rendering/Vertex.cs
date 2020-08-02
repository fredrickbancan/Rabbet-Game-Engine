using FredrickTechDemo.FredsMath;
using System.Runtime.InteropServices;

namespace FredrickTechDemo
{
    public struct Vertex
    {
        public Vector3F pos;
        public Vector4F color;
        public Vector2F uv;

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
        public static readonly int vertexByteSize = positionSize + colorSize + uvSize;

        public Vertex(float x, float y, float z, float r, float g, float b, float a, float u, float v)
        {
            pos.x = x;
            pos.y = y;
            pos.z = z;
            color.x = r;
            color.y = g;
            color.z = b;
            color.w = a;
            uv.x = u;
            uv.y = v;
        }
        public Vertex(Vector3F pos, Vector4F color, Vector2F uv)
        {
            this.pos = pos;
            this.color = color;
            this.uv = uv;
        }
    }
}
