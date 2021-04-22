using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RabbetGameEngine
{
    public class ChunkRendererSorter : IComparer<ChunkRenderer>
    {
        private Vector3i centralCoordinate;
        public int Compare([AllowNull] ChunkRenderer x, [AllowNull] ChunkRenderer y)
        {
            //return null as larger
            int xDist = MathUtil.manhattanDist(x.pos, centralCoordinate);
            int yDist = MathUtil.manhattanDist(y.pos, centralCoordinate);
            return x == null ? 1 : (y == null ? -1 : (xDist > yDist ? 1 : -1));
        }

        public ChunkRendererSorter setCenter(Vector3i cent)
        {
            centralCoordinate = cent;
            return this;
        }
    }
}
