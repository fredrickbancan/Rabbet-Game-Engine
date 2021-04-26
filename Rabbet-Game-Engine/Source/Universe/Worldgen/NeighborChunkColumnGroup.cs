using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    /// <summary>
    /// Stores a chunk and its neighbor chunks for quickly accessing voxels which may be influenced by direct neighbor voxels
    /// </summary>
    public class NeighborChunkColumnGroup
    {
        private ChunkColumn[] cache = null;
        private int minChunkRangeCoordX;
        private int minChunkRangeCoordZ;
        private int middleChunkY;
        private int middleChunkYVoxels;
        private int maxChunkRangeCoordX;
        private int maxChunkRangeCoordZ;
        public NeighborChunkColumnGroup(Terrain t, Vector3i middleChunkPos)
        {
            cache = new ChunkColumn[9];
            minChunkRangeCoordX = middleChunkPos.X - 1;
            minChunkRangeCoordZ = middleChunkPos.Z - 1;
            this.middleChunkY = middleChunkPos.Y;
            middleChunkYVoxels = middleChunkPos.Y * Chunk.CHUNK_SIZE;
            maxChunkRangeCoordX = middleChunkPos.X + 1;
            maxChunkRangeCoordZ = middleChunkPos.Z + 1;
            for (int x = minChunkRangeCoordX, locX = 0; x <= maxChunkRangeCoordX; x++, locX++)
                for (int z = minChunkRangeCoordZ, locZ = 0; z <= maxChunkRangeCoordZ; z++, locZ++)
                {
                    cache[locX * 3 + locZ] =  t.getChunkColumnAtChunkCoords(x,z);
                }
        }

        public Chunk getChunkAtLocalVoxelCoords(int x, int y, int z)
        {
            if (middleChunkYVoxels + y < 0 || middleChunkYVoxels + y >= ChunkColumn.NUM_VOXELS_HEIGHT) return null;
            int nx = (x >> Chunk.Z_SHIFT) + 1;
            int ny = (y >> Chunk.Z_SHIFT) + middleChunkY;
            int nz = (z >> Chunk.Z_SHIFT) + 1;
            if (nx < 0 || nx > 2 || nz < 0 || nz > 2) return null;
            ChunkColumn c = cache[nx * 3 + nz];
            return c == null ? null : c.getChunkAtYChunkCoord(ny);
        }

        public int getVoxelAtLocalVoxelCoords(int x, int y, int z)
        {
            Chunk localC = getChunkAtLocalVoxelCoords(x, y, z);
            return localC == null || localC.isEmpty ? 0 : localC.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }

        public int getLightLevelAtVoxelCoords(int x, int y, int z)
        {
            Chunk localC = getChunkAtLocalVoxelCoords(x, y, z);
            return localC == null ? 63 : localC.getLightLevelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }
        public ChunkColumn getChunkColumnAtChunkCoords(int x, int z)
        {
            x++; z++;
            if (x < 0 || x > 2 || z < 0 || z > 2) return null;
            return cache[x * 3 + z];
        }
    }
}
