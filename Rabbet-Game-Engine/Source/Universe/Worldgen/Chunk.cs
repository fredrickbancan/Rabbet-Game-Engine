using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class Chunk
    {
        /// <summary>
        /// Size of chunk width, height and depth.
        /// </summary>
        public static readonly int CHUNK_SIZE = 64;
        public static readonly int CHUNK_SIZE_MINUS_ONE = CHUNK_SIZE - 1;
        public static readonly int CHUNK_SIZE_SQUARED = 4096;
        public static readonly int CHUNK_SIZE_CUBED = 262144;

        /// <summary>
        /// useful for indexing flat 3d array
        /// </summary>
        public static readonly int CHUNK_X_SHIFT = 12;

        /// <summary>
        /// useful for indexing flat 3d array
        /// </summary>
        public static readonly int CHUNK_Z_SHIFT = 6;


        private CheapVoxel[] chunkData;
        private Vector3i chunkPos;
        private VoxelBatcher voxelBatcher = null;

        public Chunk(int x, int y, int z)
        {
            chunkPos.X = x;
            chunkPos.Y = y;
            chunkPos.Z = z;
            chunkData = new CheapVoxel[CHUNK_SIZE_CUBED];
            voxelBatcher = new VoxelBatcher(this);
            setVoxelAt(0, 0, 0, 1);
            voxelBatcher.updateVoxelMesh();
        }

        /// <summary>
        /// sets the voxel at the provided chunk-relative coordinates to the provided id.
        /// Indexes array by x * (size * size) + z * size + y
        /// </summary>
        public void setVoxelAt(int x, int y, int z, byte id)
        {
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            if(index < CHUNK_SIZE_CUBED) chunkData[index].id = id;
        }

        /// <summary>
        /// returns the voxel id at the provided chunk-relative coordinates 
        /// Indexes array by x * (size * size) + z * size + y
        /// </summary>
        public byte getVoxelAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return 0;
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            return index >= CHUNK_SIZE_CUBED ? (byte)0 : chunkData[index].id;
        }
        
        public bool isVoxelVisible(int x, int y, int z)
        {
            return !(VoxelType.isVoxelOpaque(getVoxelAt(x + 1, y, z)) &&
                VoxelType.isVoxelOpaque(getVoxelAt(x - 1, y, z)) &&
                VoxelType.isVoxelOpaque(getVoxelAt(x, y + 1, z)) &&
                VoxelType.isVoxelOpaque(getVoxelAt(x, y - 1, z)) &&
                VoxelType.isVoxelOpaque(getVoxelAt(x, y, z + 1)) && 
                VoxelType.isVoxelOpaque(getVoxelAt(x, y, z - 1)));
        }

        public void unLoad()
        {
            voxelBatcher.deleteVAO();
        }

        public Vector3i chunkCoord
        { get => chunkPos; }

        public VoxelBatcher voxelMesh
        { get => voxelBatcher; }
    }
}
