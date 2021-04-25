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

        public Vector2i coord
        { get; private set; }

        public ChunkColumn(int x, int z)
        {
            coord = new Vector2i(x, z);
            x *= Chunk.CHUNK_SIZE;
            z *= Chunk.CHUNK_SIZE;
            columnBounds = AABB.fromBounds(new Vector3(x,0,z), new Vector3(x + Chunk.CHUNK_SIZE, NUM_VOXELS_HEIGHT, z + Chunk.CHUNK_SIZE));
            verticalChunks = new Chunk[NUM_CHUNKS_HEIGHT];
        }

        public ChunkColumn(Vector2i pos)
        {
            coord = pos;
            pos *= Chunk.CHUNK_SIZE;
            columnBounds = AABB.fromBounds(new Vector3(pos.X, 0, pos.Y), new Vector3(pos.X + Chunk.CHUNK_SIZE, NUM_VOXELS_HEIGHT, pos.Y + Chunk.CHUNK_SIZE));
            verticalChunks = new Chunk[NUM_CHUNKS_HEIGHT];
        }

        public Chunk getChunkAtYChunkCoord(int y)
        {
            return verticalChunks[y];
        }

        public Chunk[] getVerticalChunks()
        {
            return verticalChunks;
        }
    }
}
