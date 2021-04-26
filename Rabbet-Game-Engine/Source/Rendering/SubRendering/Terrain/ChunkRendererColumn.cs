using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class ChunkRendererColumn
    {
        private ChunkRenderer[] verticalChunkRenderers = new ChunkRenderer[ChunkColumn.NUM_CHUNKS_HEIGHT];
        public AABB columnBounds
        { get; private set; }

        public Vector2i coord
        { get; private set; }

        public bool isInFrustum = false;

        public ChunkColumn correspondingChunkColumn { get; private set; }

        public ChunkRendererColumn(ChunkColumn correspondingChunkColumn, int x, int z)
        {
            this.correspondingChunkColumn = correspondingChunkColumn;
            for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++) verticalChunkRenderers[y] = new ChunkRenderer(correspondingChunkColumn.getChunkAtYChunkCoord(y));
            coord = new Vector2i(x, z);
            x *= Chunk.CHUNK_SIZE;
            z *= Chunk.CHUNK_SIZE;
            columnBounds = AABB.fromBounds(new Vector3(x, 0, z), new Vector3(x + Chunk.CHUNK_SIZE, ChunkColumn.NUM_VOXELS_HEIGHT, z + Chunk.CHUNK_SIZE));
        }

        public ChunkRendererColumn(ChunkColumn correspondingChunkColumn, Vector2i pos)
        {
            this.correspondingChunkColumn = correspondingChunkColumn;
            for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++) verticalChunkRenderers[y] = new ChunkRenderer(correspondingChunkColumn.getChunkAtYChunkCoord(y));
            coord = pos;
            pos *= Chunk.CHUNK_SIZE;
            columnBounds = AABB.fromBounds(new Vector3(pos.X, 0, pos.Y), new Vector3(pos.X + Chunk.CHUNK_SIZE, ChunkColumn.NUM_VOXELS_HEIGHT, pos.Y + Chunk.CHUNK_SIZE));
        }

        public ChunkRenderer getChunkRendererAtYChunkCoord(int y)
        {
            return verticalChunkRenderers[y];
        }

        public ChunkRenderer[] getVerticalChunkRenderers()
        {
            return verticalChunkRenderers;
        }
    }
}
