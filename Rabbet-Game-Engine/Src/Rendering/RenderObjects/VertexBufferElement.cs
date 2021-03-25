using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    public struct VertexBufferElement
    {
        public VertexAttribPointerType type;
        public int count;
        public bool normalized;
        public VertexBufferElement(VertexAttribPointerType t, int c, bool n)
        {
            type = t;
            count = c;
            normalized = n;
        }

        public static int getSizeOfType(VertexAttribPointerType type)
        {
            switch(type)
            {
                case VertexAttribPointerType.Float: return 4;
                case VertexAttribPointerType.UnsignedInt: return 4;
                case VertexAttribPointerType.UnsignedByte: return 1;
                case VertexAttribPointerType.UnsignedShort: return 2;
                case VertexAttribPointerType.Double: return 8;
                case VertexAttribPointerType.Int: return 4;
                case VertexAttribPointerType.Byte: return 1;
                default: return 0;
            }
        }
    }
}
