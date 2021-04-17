namespace RabbetGameEngine
{
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class VoxelBatcher
    {
        public static readonly int MAX_CHUNK_FACE_COUNT = 786432;
        public static readonly int CHUNK_VERTEX_INDICES_COUNT = MAX_CHUNK_FACE_COUNT * 6;
        public static uint[] VOXEL_INDICES_BUFFER;//000000 111111 222222 333333 4444444 555555 666666
        private static uint[] VOXEL_CORNER_ID_BUFFER;//012132 012132 012132 012132 012132 012132 012132
        private static IndexBufferObject VOXEL_VERTEX_IBO;
        private static VertexBufferObject VOXEL_CORNER_ID_VBO;
        public static void init()
        {
            VOXEL_INDICES_BUFFER = new uint[CHUNK_VERTEX_INDICES_COUNT];
            for (int i = 0; i < CHUNK_VERTEX_INDICES_COUNT; i += 6)
            {
                for (uint j = 0; j < 6; j++) VOXEL_INDICES_BUFFER[i + j] = j;
            }
            VOXEL_VERTEX_IBO = new IndexBufferObject();
            VOXEL_VERTEX_IBO.initStatic(VOXEL_INDICES_BUFFER);
        }

        public static void onClosing()
        {
            VOXEL_VERTEX_IBO.delete();
        }
        private Chunk parentChunk = null;
        private VertexArrayObject voxelsVAO = null;
        private VoxelFace[] voxelBuffer = null;
        private bool vaoNeedsUpdate = true;
        private int addedVoxelFaceCount = 0;

        public VoxelBatcher(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            voxelBuffer = new VoxelFace[MAX_CHUNK_FACE_COUNT];
            voxelsVAO = new VertexArrayObject();
            voxelsVAO.bind();
            voxelsVAO.beginBuilding();
            VertexBufferLayout vbl = new VertexBufferLayout();
            VoxelFace.configureLayout(vbl);
            voxelsVAO.addBufferDynamic(Chunk.CHUNK_SIZE_CUBED * VoxelFace.SIZE_IN_BYTES, vbl);
            voxelsVAO.finishBuilding();
        }

        /// <summary>
        /// Updates the voxel buffer based on visible voxels for optimisation.
        /// Should be called whenever the parent chunk's voxels change such as voxels added or removed.
        /// </summary>
        public void updateVoxelMesh()
        {
            addedVoxelFaceCount = 0;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {

                    }
            voxelBuffer[0] = new VoxelFace(1, 0, 0, 0, 7, 0, 0);
            voxelBuffer[1] = new VoxelFace(1, 0, 1, 0, 15, 0, 0);
            voxelBuffer[2] = new VoxelFace(1, 0, 2, 0, 31, 0, 0);
            voxelBuffer[3] = new VoxelFace(1, 0, 3, 0, 63, 0, 0);
            addedVoxelFaceCount = 4; 
            vaoNeedsUpdate = true;
        }

        public void bindVAO()
        {
            voxelsVAO.bind();
            VOXEL_VERTEX_IBO.bind();
            if (vaoNeedsUpdate)
            {
                voxelsVAO.updateBuffer(0, voxelBuffer, addedVoxelFaceCount * VoxelFace.SIZE_IN_BYTES);
                vaoNeedsUpdate = false;
            }
        }


        public void deleteVAO()
        {
            voxelsVAO.delete();
        }

        public bool needsUpdate
        { get => vaoNeedsUpdate; }

        public int visibleVoxelFaceCount
        { get => addedVoxelFaceCount; }
    }
}
