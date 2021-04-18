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
        public static readonly float VOXEL_PHYSICAL_SIZE = 0.25F;
        public static readonly float VOXEL_PHYSICAL_OFFSET = 0.125F;
        public static readonly float CHUNK_PHYSICAL_SIZE = CHUNK_SIZE * VOXEL_PHYSICAL_SIZE;

        /// <summary>
        /// useful for indexing flat 3d array
        /// </summary>
        public static readonly int CHUNK_X_SHIFT = 12;

        /// <summary>
        /// useful for indexing flat 3d array
        /// </summary>
        public static readonly int CHUNK_Z_SHIFT = 6;
        private byte[] voxels;
        private LightMap lightMap;
        private VoxelBatcher voxelBatcher = null;
        private Terrain parentTerrain;
        private Vector3i coord;
        private Vector3i worldCoord;
        public Chunk(Vector3i coord, Terrain pt)
        {
            this.coord = coord;
            worldCoord = coord * CHUNK_SIZE;
            parentTerrain = pt;
            voxels = new byte[CHUNK_SIZE_CUBED];
            lightMap = new LightMap(CHUNK_SIZE_CUBED);
            voxelBatcher = new VoxelBatcher(this);
            parentTerrain.scheduleUpdateForChunk(this);
        }

        /// <summary>
        /// sets the voxel at the provided chunk-relative coordinates to the provided id.
        /// Indexes array by x * (size * size) + z * size + y
        /// </summary>
        public void setVoxelAt(int x, int y, int z, byte id)
        {
            if (x < 0 || y < 0 || z < 0) return;
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            if(index < CHUNK_SIZE_CUBED) voxels[index] = id;
        }

        /// <summary>
        /// sets the light level of the coord to provided light level. light level must range from 0 to 63
        /// </summary>
        public void setLightLevelAt(int x, int y, int z, byte level)
        {
            lightMap.setLightLevelAt(x, y, z, level);
        }


        public byte getLightLevelAt(int x, int y, int z)
        {
            return lightMap.getLightLevelAt(x, y, z);
        }

        public byte getLightLevelAtSafe(int x, int y, int z)
        {
            if (x >= CHUNK_SIZE || x < 0 || y >= CHUNK_SIZE || y < 0 || z >= CHUNK_SIZE || z < 0)
                return parentTerrain.getLightLevelAtVoxelCoord(worldCoord.X + x, worldCoord.Y + y, worldCoord.Z + z);
            return lightMap.getLightLevelAt(x, y, z);
        }

        public byte getLightLevelAtSafe(Vector3i pos)
        {
            if (pos.X >= CHUNK_SIZE || pos.X < 0 || pos.Y >= CHUNK_SIZE || pos.Y < 0 || pos.Z >= CHUNK_SIZE || pos.Z < 0)
                return parentTerrain.getLightLevelAtVoxelCoord(worldCoord.X + pos.X, worldCoord.Y + pos.Y, worldCoord.Z + pos.Z);
            return lightMap.getLightLevelAt(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// returns the voxel id at the provided chunk-relative coordinates 
        /// Indexes array by x * (size * size) + z * size + y
        /// </summary>
        public byte getVoxelAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return 0;
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            return index >= CHUNK_SIZE_CUBED ? (byte)0 : voxels[index];
        }

        /// <summary>
        /// returns the voxel id at the provided chunk-relative coordinates 
        /// Indexes array by x * (size * size) + z * size + y
        /// If coords are outside of this chunk, will query parent terrain for coordinate.
        /// </summary>
        public byte getVoxelAtSafe(int x, int y, int z)
        {
            if (x >= CHUNK_SIZE || x < 0 || y >= CHUNK_SIZE || y < 0 || z >= CHUNK_SIZE || z < 0)
                return parentTerrain.getVoxelIdAtVoxelCoord(worldCoord.X + x,worldCoord.Y + y,worldCoord.Z +  z);
            int index = x << CHUNK_X_SHIFT | z << CHUNK_Z_SHIFT | y;
            return voxels[index];
        }

        public byte getVoxelAtSafe(Vector3i pos)
        {
            if (pos.X >= CHUNK_SIZE || pos.X < 0 || pos.Y >= CHUNK_SIZE || pos.Y < 0 || pos.Z >= CHUNK_SIZE || pos.Z < 0)
            {
                pos += worldCoord;
                return parentTerrain.getVoxelIdAtVoxelCoord(pos.X, pos.Y, pos.Z);
            }
            int index = pos.X << CHUNK_X_SHIFT | pos.Z << CHUNK_Z_SHIFT | pos.Y;
            return voxels[index];
        }

        public void update()
        {
            voxelBatcher.updateVoxelMesh();
        }

        public void unLoad()
        {
            voxelBatcher.deleteVAO();
        }

        public byte[] getVoxels()
        {
            return voxels;
        }

        public Terrain terrainParent
        { get => parentTerrain; }

        public Vector3i coordinate
        { get => coord; }

        public VoxelBatcher voxelMesh
        { get => voxelBatcher; }
    }
}
