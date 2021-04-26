using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class Chunk
    {
        public static readonly int CHUNK_SIZE = 32;
        public static readonly int CHUNK_SIZE_MINUS_ONE = CHUNK_SIZE - 1;
        public static readonly int CHUNK_SIZE_MINUS_TWO = CHUNK_SIZE - 2;
        public static readonly int CHUNK_SIZE_SQUARED = 1024;
        public static readonly int CHUNK_SIZE_CUBED = 32768;
        public static readonly float VOXEL_PHYSICAL_SIZE = 0.5F;
        public static readonly float VOXEL_PHYSICAL_OFFSET = 0.25F;
        public static readonly float CHUNK_PHYSICAL_SIZE = CHUNK_SIZE * VOXEL_PHYSICAL_SIZE;
        public static readonly int X_SHIFT = 10;
        public static readonly int Z_SHIFT = 5;
        public static Vector3i worldToChunkPos(Vector3 vec)
        {
            return MathUtil.rightShift((Vector3i)(vec / Chunk.VOXEL_PHYSICAL_SIZE), Z_SHIFT);
        }
        public static Vector3i worldToVoxelPos(Vector3 vec)
        {
            return (Vector3i)(vec / Chunk.VOXEL_PHYSICAL_SIZE);
        }

        private byte[] voxels = null;
        private LightMap lightMap = null;
        public bool isMarkedForRemoval = false;
        public bool isMarkedForRenderUpdate = false;
        public bool isScheduledForPopulation = false;
        public bool isEmpty
        { get; private set; }
        public Vector3i coord { get; private set; }
        public Vector3i worldCoord { get; private set; }

        public Chunk(Vector3i coord)
        {
            this.isEmpty = true;
            this.coord = coord;
            worldCoord = coord * CHUNK_SIZE;
            lightMap = new LightMap(CHUNK_SIZE_CUBED);
        }

        /// <summary>
        /// Should be called apon first time of adding a non-air voxel to this chunk.
        /// Not initializing the byte buffer on construction saves memory if this chunk doesnt
        /// contain any voxels yet anyway.
        /// </summary>
        public void init()
        { 
            voxels = new byte[CHUNK_SIZE_CUBED];
            isEmpty = false;
        }

        public void setVoxelAt(int x, int y, int z, byte id)
        {
            if (x < 0 || y < 0 || z < 0) return;
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            if (index < CHUNK_SIZE_CUBED) voxels[index] = id;
        }
        public void setLightLevelAt(int x, int y, int z, byte level)
        {
            lightMap.setLightLevelAt(x, y, z, level);
        }

        public byte getLightLevelAt(int x, int y, int z)
        {
            return lightMap.getLightLevelAt(x, y, z);
        }

        public int getVoxelAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return 0;
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            return index >= CHUNK_SIZE_CUBED ? 0 : voxels[index];
        }
        public byte[] getVoxels()
        {
            return voxels;
        }
    }
}
