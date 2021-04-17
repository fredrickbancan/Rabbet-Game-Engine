using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    public struct VertexBufferElement
    {
        public int type;
        public int count;
        public bool normalized;
        public bool isInteger;
        public int divisor;
        public VertexBufferElement(VertexAttribPointerType t, int c, bool n, int divisor = 1)
        {
            type = (int)t;
            count = c;
            normalized = n;
            isInteger = false;
            this.divisor = divisor;
        }
        public VertexBufferElement(VertexAttribIntegerType t, int c, int divisor = 1)
        {
            type = (int)t;
            count = c;
            normalized = false;
            isInteger = true;
            this.divisor = divisor;
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

        public static int getSizeOfType(VertexAttribIntegerType type)
        {
            switch(type)
            {
                case VertexAttribIntegerType.Byte: return 1;
                case VertexAttribIntegerType.Int: return 4;
                case VertexAttribIntegerType.Short: return 2;
                case VertexAttribIntegerType.UnsignedByte: return 1;
                case VertexAttribIntegerType.UnsignedInt: return 4;
                case VertexAttribIntegerType.UnsignedShort: return 2;
                default: return 0;
            }
        }
    }
}
