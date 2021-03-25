using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace RabbetGameEngine
{
    public struct PointParticle
    {
        public Vector3 pos;
        public Vector4 color;
        public float radius;
        public float aoc;//0 if false, 1 if true;

        /*these are only in bytes to save memory. They shouldnt ever get even close to 255*/
        public static readonly byte positionLength = 3;
        public static readonly byte colorLength = 4;
        public static readonly byte radiusLength = 1;
        public static readonly byte aocLength = 1;
        public static readonly byte positionSize = 3 * sizeof(float);
        public static readonly byte colorSize = 4 * sizeof(float);
        public static readonly byte radiusSize = sizeof(float);
        public static readonly byte aocSize = sizeof(float);
        public static readonly byte positionOffset = (byte)Marshal.OffsetOf(typeof(PointParticle), "pos");
        public static readonly byte colorOffset = (byte)Marshal.OffsetOf(typeof(PointParticle), "color");
        public static readonly byte radiusOffset = (byte)Marshal.OffsetOf(typeof(PointParticle), "radius");
        public static readonly byte aocOffset = (byte)Marshal.OffsetOf(typeof(PointParticle), "aoc");
        public static readonly int pParticleByteSize = positionSize + colorSize + radiusSize + aocSize;

        public PointParticle(float x, float y, float z, float r, float g, float b, float a, float radius, bool aoc)
        {
            pos.X = x;
            pos.Y = y;
            pos.Z = z;
            color.X = r;
            color.Y = g;
            color.Z = b;
            color.W = a;
            this.radius = radius;
            this.aoc = aoc ? 1.0F : 0.0F;
        }
        public PointParticle(Vector3 pos, Vector4 color, float radius, bool aoc)
        {
            this.pos = pos;
            this.color = color;
            this.radius = radius;
            this.aoc = aoc ? 1.0F : 0.0F;
        }

        public static void configureLayout(VertexBufferLayout l)
        {
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 3);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 4);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 1);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 1);
        }
    }
}
