namespace RabbetGameEngine
{
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class VoxelBatcher
    {
        public static readonly int MAX_CHUNK_FACE_COUNT = 786432;//maciumum voxel faces that can be visible in a chunk
        public static readonly int CHUNK_VERTEX_INDICES_COUNT = MAX_CHUNK_FACE_COUNT * 4;
        public static uint[] VOXEL_INDICES_BUFFER;//0000 1111 2222 3333 4444 5555 6666
        private static IndexBufferObject VOXEL_VERTEX_IBO;

        public static void init()
        {
            VOXEL_INDICES_BUFFER = new uint[CHUNK_VERTEX_INDICES_COUNT];
            for (uint i = 0; i < CHUNK_VERTEX_INDICES_COUNT; i += 4U)
            {
                for (int j = 0; j < 4; j++) VOXEL_INDICES_BUFFER[i + j] = i / 4U;
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
        private VoxelFace[] voxelFaceBuffer = null;
        private bool vaoNeedsUpdate = true;
        private int addedVoxelFaceCount = 0;

        public VoxelBatcher(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            voxelFaceBuffer = new VoxelFace[MAX_CHUNK_FACE_COUNT];
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
            int faceMask;//bottom 6 bits used as booleans for each voxel face.
            int faceCountCounter;
            int faceCount;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        VoxelType vt = VoxelType.getVoxelById(parentChunk.getVoxelAt(x, y, z));
                        if (vt == null) continue;
                        /*  faceMask = 0;
                          faceCount = 0;
                          faceCountCounter = 0;
                          //get orientation bits
                          faceCountCounter = faceMask = getVisibleFacesForVoxel(x, y, z);

                          //count faces shown
                          while (faceCountCounter != 0) { faceCount += faceMask & 1; faceCountCounter >>= 1; }

                          //add faces with orientation and values
                          for(faceCountCounter = 0; faceCountCounter < faceCount; faceCountCounter++)
                          {
                              voxelFaceBuffer[addedVoxelFaceCount++] = new VoxelFace(vt.id, (byte)x, (byte)y, (byte)z, 0, 0, 0);
                          }*/
                        voxelFaceBuffer[addedVoxelFaceCount] = new VoxelFace(1,(byte)x,(byte)y, (byte)z,(byte)GameInstance.rand.Next(0,64),0,0);
                        voxelFaceBuffer[addedVoxelFaceCount] = new VoxelFace(1,(byte)x,(byte)y, (byte)z,(byte)GameInstance.rand.Next(0,64),0,1);
                        addedVoxelFaceCount++;
                    }
            vaoNeedsUpdate = true;
        }

        /// <summary>
        /// returns a bit mask for faces shown on the voxel at the provided coords
        /// </summary>
        private int getVisibleFacesForVoxel(int x, int y, int z)
        {
            int result = 0;
            result |= 1 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x + 1, y, z)));//check posx face
            result |= 2 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x, y + 1, z)));//check posy face
            result |= 4 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x, y, z + 1)));//check posz face
            result |= 8 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x - 1, y, z)));//check negx face
            result |= 16 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x, y - 1, z)));//check negy face
            result |= 32 * System.Convert.ToInt32(VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(x, y, z - 1)));//check negz face
            return result;
        }

        public void bindVAO()
        {
            voxelsVAO.bind();
            VOXEL_VERTEX_IBO.bind();
            if (vaoNeedsUpdate)
            {
                voxelsVAO.updateBuffer(0, voxelFaceBuffer, addedVoxelFaceCount * VoxelFace.SIZE_IN_BYTES);
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
