namespace RabbetGameEngine
{
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class VoxelBatcher
    {
        private Chunk parentChunk = null;
        private VertexArrayObject voxelsVAO = null;
        private Voxel[] voxelBuffer = null;
        private bool vaoNeedsUpdate = true;
        private int addedVoxelItterator = 0;

        public VoxelBatcher(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            voxelBuffer = new Voxel[Chunk.CHUNK_SIZE_CUBED];
            buildVAO();
        }

        private void buildVAO()
        {
            voxelsVAO = new VertexArrayObject();
            voxelsVAO.bind();
            voxelsVAO.beginBuilding();
            VertexBufferLayout vbl = new VertexBufferLayout();
            Voxel.configureLayout(vbl);
            voxelsVAO.addBufferDynamic(Chunk.CHUNK_SIZE_CUBED * Voxel.SIZE_IN_BYTES, vbl);
            voxelsVAO.finishBuilding();
        }

        /// <summary>
        /// Updates the voxel buffer based on visible voxels for optimisation.
        /// Should be called whenever the parent chunk's voxels change such as voxels added or removed.
        /// </summary>
        public void updateVoxelMesh()
        {
            addedVoxelItterator = 0;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        CheapVoxel v = parentChunk.getCheapVoxelAt(x, y, z);
                        VoxelType vt = VoxelType.getVoxelById(v.id);
                        if (vt == null) 
                            continue;

                        voxelBuffer[addedVoxelItterator].setID(v.id);
                        voxelBuffer[addedVoxelItterator].setChunkIndex(x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y);
                        voxelBuffer[addedVoxelItterator].setLightLevel(v.lightLevel);
                        addedVoxelItterator++;
                    }
            vaoNeedsUpdate = true;
        }

        public void bindVAO()
        {
            voxelsVAO.bind();
            if (vaoNeedsUpdate)
            {
                voxelsVAO.updateBuffer(0, voxelBuffer, addedVoxelItterator * Voxel.SIZE_IN_BYTES);
                //update vao n stuff
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
        { get => addedVoxelItterator; }
    }
}
