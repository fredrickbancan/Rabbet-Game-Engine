using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RabbetGameEngine
{
    public class ChunkRendererSorter : IComparer<ChunkRenderer>
    {
        private Vector3i centralCoordinate;
        public int Compare([AllowNull] ChunkRenderer a, [AllowNull] ChunkRenderer b)
        {
            //y * 2 for bias towards horizontal priority
            int xDist = MathUtil.manhattanDistHorizontalBias(a.pos, centralCoordinate, 2);
            int yDist = MathUtil.manhattanDistHorizontalBias(b.pos, centralCoordinate, 2);
            return a == null ? 1 : (b == null ? -1 : (xDist > yDist ? 1 : -1));
        }

        public ChunkRendererSorter setCenter(Vector3i cent)
        {
            centralCoordinate = cent;
            return this;
        }
    }
}
