using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
namespace RabbetGameEngine
{
    public class VertexBufferLayout
    {
        public List<VertexBufferElement> elements = null;

        

        private int stride = 0;

        public void add(VertexAttribPointerType type, int count, bool normalized = false)
        {
            elements.Add(new VertexBufferElement(type, count, normalized));
            stride += VertexBufferElement.getSizeOfType(type);
        }

        public int getStride()
        {
            return stride;
        }
    }
}
