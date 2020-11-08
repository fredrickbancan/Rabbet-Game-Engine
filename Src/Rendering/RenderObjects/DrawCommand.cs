using System.Runtime.InteropServices;

namespace RabbetGameEngine
{
    /// <summary>
    /// A struct for containing draw command parameters for glMultiDrawElementsIndirect
    /// </summary>
    public struct DrawCommand
    {
        public static readonly int sizeInBytes = 5 * sizeof(uint);
        public static readonly byte countOffset = (byte)Marshal.OffsetOf(typeof(DrawCommand), "count");
        public static readonly byte instanceCountOffset = (byte)Marshal.OffsetOf(typeof(DrawCommand), "instanceCount");
        public static readonly byte firstIndexOffset = (byte)Marshal.OffsetOf(typeof(DrawCommand), "firstIndex");
        public static readonly byte baseVertexOffset = (byte)Marshal.OffsetOf(typeof(DrawCommand), "baseVertex");
        public static readonly byte baseInstanceOffset = (byte)Marshal.OffsetOf(typeof(DrawCommand), "baseInstance");

        uint count;
        uint instanceCount;
        uint firstIndex;
        uint baseVertex;
        uint baseInstance;

        public DrawCommand(uint count, uint instanceCount, uint firstIndex, uint baseVertex, uint baseInstance)
        {
            this.count = count;
            this.instanceCount = instanceCount;
            this.firstIndex = firstIndex;
            this.baseVertex = baseVertex;
            this.baseInstance = baseInstance;
        }
    }
}
