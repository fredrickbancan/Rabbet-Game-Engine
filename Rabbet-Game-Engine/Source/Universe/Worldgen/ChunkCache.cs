using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    /// <summary>
    /// Stores a chunk and its neighbor chunks for quickly accessing voxels which may be influenced by direct neighbor voxels
    /// </summary>
    public class ChunkCache
    {
        private Chunk[] cache = null;
        private int chunksDiameter;
        private int chunksDiameterSq;
        private int chunksDiameterQu;
        private int minChunkRangeCoordX;
        private int minChunkRangeCoordY;
        private int minChunkRangeCoordZ;
        private int maxChunkRangeCoordX;
        private int maxChunkRangeCoordY;
        private int maxChunkRangeCoordZ;
        public ChunkCache(Terrain t, Vector3i centerChunkPos, int chunkRangeRadius, bool includeDiagonals)
        {
            chunkRangeRadius = chunkRangeRadius <= 0 ? 1 : chunkRangeRadius;
            chunksDiameter = chunkRangeRadius * 2 + 1;// + 1 for center chunk
            chunksDiameterSq = chunksDiameter * chunksDiameter;
            chunksDiameterQu = chunksDiameter * chunksDiameter * chunksDiameter;
            cache = new Chunk[chunksDiameterQu];
            minChunkRangeCoordX = centerChunkPos.X - chunkRangeRadius;
            minChunkRangeCoordY = centerChunkPos.Y - chunkRangeRadius;
            minChunkRangeCoordZ = centerChunkPos.Z - chunkRangeRadius;
            maxChunkRangeCoordX = centerChunkPos.X + chunkRangeRadius;
            maxChunkRangeCoordY = centerChunkPos.Y + chunkRangeRadius;
            maxChunkRangeCoordZ = centerChunkPos.Z + chunkRangeRadius;

            //fill chunk cache from terrain
            if (includeDiagonals)
            {
                for (int x = minChunkRangeCoordX; x <= maxChunkRangeCoordX; x++)
                    for (int z = minChunkRangeCoordZ; z <= maxChunkRangeCoordZ; z++)
                        for (int y = minChunkRangeCoordY; y <= maxChunkRangeCoordY; y++)
                        {
                            setChunk(x, y, z, t.getChunkAtChunkCoords(x, y, z));
                        }
            }
            else
            {
                for (int x = minChunkRangeCoordX; x <= maxChunkRangeCoordX; x++) setChunk(x, centerChunkPos.Y, centerChunkPos.Z, t.getChunkAtChunkCoords(x, centerChunkPos.Y, centerChunkPos.Z));
                for (int z = minChunkRangeCoordZ; z <= maxChunkRangeCoordZ; z++) setChunk(centerChunkPos.X, centerChunkPos.Y, z, t.getChunkAtChunkCoords(centerChunkPos.X, centerChunkPos.Y, z));
                for (int y = minChunkRangeCoordY; y <= maxChunkRangeCoordY; y++) setChunk(centerChunkPos.X, y, centerChunkPos.Z, t.getChunkAtChunkCoords(centerChunkPos.X, y, centerChunkPos.Z));
            }
        }

        /// <summary>
        /// x,y,z are in local coordinates to this cache. 0 is min, (chunksdiameter) is max
        /// </summary>
        private void setChunk(int x, int y, int z, Chunk c)
        {
            int localX = x - minChunkRangeCoordX;
            int localY = y - minChunkRangeCoordY;
            int localZ = z - minChunkRangeCoordZ;
            cache[localX * chunksDiameterSq + localZ * chunksDiameter + localY] = c;
        }

        public Chunk getChunkAtVoxelCoords(int x, int y, int z)
        {
            int nx = x >> Chunk.CHUNK_Z_SHIFT;
            int ny = y >> Chunk.CHUNK_Z_SHIFT;
            int nz = z >> Chunk.CHUNK_Z_SHIFT;
            if (nx < minChunkRangeCoordX || nx > maxChunkRangeCoordX || ny < minChunkRangeCoordY || ny > maxChunkRangeCoordY || nz < minChunkRangeCoordZ || nz > maxChunkRangeCoordZ) return null;
            int localX = nx - minChunkRangeCoordX;
            int localY = ny - minChunkRangeCoordY;
            int localZ = nz - minChunkRangeCoordZ;
            return cache[localX * chunksDiameterSq + localZ * chunksDiameter + localY];
        }

        public byte getVoxelAtVoxelCoords(int x, int y, int z)
        {
            Chunk localC = getChunkAtVoxelCoords(x, y, z);
            return localC == null ? (byte)0 : localC.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }

        public byte getLightLevelAtVoxelCoords(int x, int y, int z)
        {
            Chunk localC = getChunkAtVoxelCoords(x, y, z);
            return localC == null ? (byte)63 : localC.getLightLevelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }
    }
}
