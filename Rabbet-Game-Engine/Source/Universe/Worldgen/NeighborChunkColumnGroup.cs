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
        private int maxChunkRangeCoordX;
        private int maxChunkRangeCoordZ;
        public NeighborChunkColumnGroup(Terrain t, int middleChunkX, int middleChunkY, int middleChunkZ)
        {
            cache = new ChunkColumn[9];
            minChunkRangeCoordX = middleChunkX - 1;
            minChunkRangeCoordZ = middleChunkZ - 1;
            this.middleChunkY = middleChunkY;
            maxChunkRangeCoordX = middleChunkX + 1;
            maxChunkRangeCoordZ = middleChunkZ + 1;
            for (int x = minChunkRangeCoordX, locX = 0; x <= maxChunkRangeCoordX; x++, locX++)
                for (int z = minChunkRangeCoordZ, locZ = 0; z <= maxChunkRangeCoordZ; z++, locZ++)
                {
                    cache[locX * 3 + locZ] =  t.getChunkColumnAtChunkCoords(x,z);
                }
        }

        public Chunk getChunkAtLocalVoxelCoords(int x, int y, int z)
        {
            if (y < 0 || y > ChunkColumn.NUM_VOXELS_HEIGHT) return null;
            int nx = (x >> Chunk.Z_SHIFT) + 1;
            int ny = y >> Chunk.Z_SHIFT + middleChunkY;
            int nz = (z >> Chunk.Z_SHIFT) + 1;
            if (nx < 0 || nx > 2 || nz < 0 || nz > 2) return null;
            ChunkColumn c = cache[nx * 3 + nz];
            return c == null ? null : c.getChunkAtYChunkCoord(ny);
        }

        public int getVoxelAtLocalVoxelCoords(int x, int y, int z)
        {
            Chunk localC = getChunkAtLocalVoxelCoords(x, y, z);
            return localC == null ? 0 : localC.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
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
