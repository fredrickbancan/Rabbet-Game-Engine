
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class ChunkColumn
    {
        public static readonly int NUM_CHUNKS_HEIGHT = 4;
        public static readonly int NUM_VOXELS_HEIGHT = NUM_CHUNKS_HEIGHT * Chunk.CHUNK_SIZE;
        private Chunk[] verticalChunks = null;
        public AABB columnBounds
        { get; private set; }

        public ChunkColumn(int x, int z)
        {
            x *= Chunk.CHUNK_SIZE;
            z *= Chunk.CHUNK_SIZE;
            columnBounds = AABB.fromBounds(new Vector3(x,0,z), new Vector3(x + Chunk.CHUNK_SIZE, NUM_VOXELS_HEIGHT, z + Chunk.CHUNK_SIZE));
            verticalChunks = new Chunk[NUM_CHUNKS_HEIGHT];
        }

        public Chunk getChunkAtYChunkCoord(int y)
        {
            return verticalChunks[y];
        }
    }
}
