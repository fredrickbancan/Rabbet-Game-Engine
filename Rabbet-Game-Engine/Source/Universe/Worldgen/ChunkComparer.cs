using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RabbetGameEngine
{
    public class ChunkComparer : IComparer<Chunk>
    {
        private Vector3i centralCoordinate;
        public int Compare([AllowNull] Chunk a, [AllowNull] Chunk b)
        {
            //y * 2 for bias towards horizontal priority
            int xDist = MathUtil.manhattanDistHorizontalBias(a.coord, centralCoordinate, 2);
            int yDist = MathUtil.manhattanDistHorizontalBias(b.coord, centralCoordinate, 2);
            return a == null ? 1 : (b == null ? -1 : (xDist > yDist ? 1 : (xDist == yDist ? 0 : -1)));
        }

        public ChunkComparer setCenter(Vector3i cent)
        {
            centralCoordinate = cent;
            return this;
        }
    }
}
