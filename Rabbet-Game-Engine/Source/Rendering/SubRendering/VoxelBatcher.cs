namespace RabbetGameEngine
{
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class VoxelBatcher
    {
        public static readonly int MAX_CHUNK_VERTEX_COUNT = 3145728;
        public static readonly int CHUNK_VERTEX_INDICES_COUNT = MAX_CHUNK_VERTEX_COUNT / 4 * 6;
        private static uint[] VOXEL_INDICES;
        private static IndexBufferObject VOXEL_VERTEX_IBO;
        public static void init()
        {
            //fill voxel indices
            VOXEL_INDICES = new uint[CHUNK_VERTEX_INDICES_COUNT];
            uint offset = 0;
            //Building indicies array, will work with any number of quads under the max amount.
            //Assuming all quads are actually quads.
            for (uint i = 0; i < CHUNK_VERTEX_INDICES_COUNT; i += 6)
            {
                VOXEL_INDICES[i + 0] = 0 + offset;
                VOXEL_INDICES[i + 1] = 1 + offset;
                VOXEL_INDICES[i + 2] = 2 + offset;
                VOXEL_INDICES[i + 3] = 1 + offset;
                VOXEL_INDICES[i + 4] = 3 + offset;
                VOXEL_INDICES[i + 5] = 2 + offset;
                offset += 4;
            }
            VOXEL_VERTEX_IBO = new IndexBufferObject();
            VOXEL_VERTEX_IBO.initStatic(VOXEL_INDICES);
        }

        public static void onClosing()
        {
            VOXEL_VERTEX_IBO.delete();
        }
        private Chunk parentChunk = null;
        private VertexArrayObject voxelsVAO = null;
        private VoxelVertex[] voxelBuffer = null;
        private bool vaoNeedsUpdate = true;
        private int addedVoxelVertexCount = 0;

        public VoxelBatcher(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            voxelBuffer = new VoxelVertex[MAX_CHUNK_VERTEX_COUNT];
            buildVAO();
        }

        private void buildVAO()
        {
            voxelsVAO = new VertexArrayObject();
            voxelsVAO.bind();
            voxelsVAO.beginBuilding();
            VertexBufferLayout vbl = new VertexBufferLayout();
            VoxelVertex.configureLayout(vbl);
            voxelsVAO.addBufferDynamic(Chunk.CHUNK_SIZE_CUBED * VoxelVertex.SIZE_IN_BYTES, vbl);
            voxelsVAO.finishBuilding();
        }

        /// <summary>
        /// Updates the voxel buffer based on visible voxels for optimisation.
        /// Should be called whenever the parent chunk's voxels change such as voxels added or removed.
        /// </summary>
        public void updateVoxelMesh()
        {
            addedVoxelVertexCount = 0;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {

                    }
            vaoNeedsUpdate = true;
        }

        public void bindVAO()
        {
            voxelsVAO.bind();
            VOXEL_VERTEX_IBO.bind();
            if (vaoNeedsUpdate)
            {
                voxelsVAO.updateBuffer(0, voxelBuffer, addedVoxelVertexCount * VoxelVertex.SIZE_IN_BYTES);
                vaoNeedsUpdate = false;
            }
        }


        public void deleteVAO()
        {
            voxelsVAO.delete();
        }

        public bool needsUpdate
        { get => vaoNeedsUpdate; }

        public int visibleVoxelCount
        { get => addedVoxelVertexCount; }
    }
}
