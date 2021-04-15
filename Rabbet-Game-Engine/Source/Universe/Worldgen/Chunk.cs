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
        public static readonly float CHUNK_PHYSICAL_SIZE = CHUNK_SIZE * Voxel.VOXEL_PHYSICAL_SIZE;

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
        }

        /// <summary>
        /// sets the voxel at the provided chunk-relative coordinates to the provided id.
        /// Indexes array by x * (size * size) + z * size + y
        /// </summary>
        public void setVoxelAt(int x, int y, int z, byte id)
        {
            if (x < 0 || y < 0 || z < 0) return;
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            if(index < CHUNK_SIZE_CUBED) chunkData[index].id = id;
        }

        /// <summary>
        /// sets the light level of the coord to provided light level. light level must range from 0 to 63
        /// </summary>
        public void setLightLevelAt(int x, int y, int z, byte level)
        {
            if (x < 0 || y < 0 || z < 0) return;
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            if (index <= CHUNK_SIZE_CUBED) chunkData[index].lightLevel = (byte)(level & 63);
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
        
        /// <summary>
        /// gives the cheapvoxel at provided coords
        /// </summary>
        public CheapVoxel getCheapVoxelAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return new CheapVoxel();
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            return chunkData[index];
        }

        public void load()
        {

            voxelBatcher.updateVoxelMesh();
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
