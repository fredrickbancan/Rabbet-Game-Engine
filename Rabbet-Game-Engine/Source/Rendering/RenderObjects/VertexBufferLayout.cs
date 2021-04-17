using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
namespace RabbetGameEngine
{
    public class VertexBufferLayout
    {
        public List<VertexBufferElement> elements = null;

        /// <summary>
        /// should be true if the buffer used with this layout should be accessed using glvertexattribdivior.
        /// </summary>
        public bool instancedData = false;
        public VertexBufferLayout()
        {
            elements = new List<VertexBufferElement>();
        }

        private int stride = 0;

        public void add(VertexAttribPointerType type, int count, bool normalized = false, int divisor = 1)
        {
            elements.Add(new VertexBufferElement(type, count, normalized, divisor));
            stride += VertexBufferElement.getSizeOfType(type) * count;
        }

        public void add(VertexAttribIntegerType type, int count, int divisor = 1)
        {
            elements.Add(new VertexBufferElement(type, count, divisor));
            stride += VertexBufferElement.getSizeOfType(type) * count;
        }

        public int getStride()
        {
            return stride;
        }
    }
}
