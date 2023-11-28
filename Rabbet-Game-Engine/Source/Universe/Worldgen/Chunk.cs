using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public enum ChunkNeighborDirection
    {
        POSX,
        POSY,
        POSZ,
        NEGZ,
        NEGY,
        NEGX
    };

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

        /*
         * Voxel integer data layout is as follows:
         *
         * SLDD DDDB BBBB GGGG GRRR RRVV VVVV VVVV
         *
         * V: Voxel id 0-1023
         * R: block light red value 0-31
         * G: block light green value 0-31
         * B: block light blue value 0-31
         * D: distance field value for accelerating raytracing 0-31
         * L: voxel lit by natural light (sun or moon) 0-1
         * S: voxel exposed to sky (air or non-solid block above the heightmap) 0-1
         */

        private uint[] voxels = null;

        public ChunkMesh localRenderer
        {
            get; private set;
        }

        private Chunk[] neighborChunks = null;
        public bool isMarkedForRemoval = false;
        public bool isMarkedForRenderUpdate = false;
        public bool isOnWorldEdge = false;
        public bool isScheduledForPopulation = false;

        public bool allNeighborsPopulated
        {
            get; private set;
        }

        public Vector3i coord
        {
            get; private set;
        }

        public Vector3i worldCoord
        {
            get; private set;
        }

        public Vector3 worldPhysicalCoord
        {
            get; private set;
        }

        public Matrix4 translationMatrix
        {
            get; private set;
        }

        private AABB chunkBounds;

        public Chunk(Vector3i coord)
        {
            this.coord = coord;
            Vector3i voxelMinBounds = coord * CHUNK_SIZE;
            Vector3i voxelMaxBounds = voxelMinBounds + new Vector3i(CHUNK_SIZE);
            chunkBounds = AABB.fromBounds((Vector3)voxelMinBounds, (Vector3)voxelMaxBounds);
            worldCoord = coord * CHUNK_SIZE;
            worldPhysicalCoord = (Vector3)coord * Chunk.CHUNK_PHYSICAL_SIZE;
            translationMatrix = Matrix4.CreateTranslation(worldPhysicalCoord);
            voxels = new uint[CHUNK_SIZE_CUBED];
            neighborChunks = new Chunk[6];
            localRenderer = new ChunkMesh(this);
        }

        /// <summary>
        /// notifies this chunk when a neighboring chunk changes in a relative manner.
        /// Such as a neighbor chunk being added, populated, edited, removed, having a lighting change etc.
        /// If a neighbor chunk is deleted, this should be called with null as the chunk.
        /// </summary>
        /// <param name="neighbor">the chunk which has changed next to this one.</param>
        /// <param name="dir">the direction relative to this chunk in which the neighbor chunk is situated.</param>
        public void notifyChunkOfNeighborChange(Chunk neighbor, ChunkNeighborDirection dir)
        {
            neighborChunks[(int)dir] = neighbor;

            if (!isMarkedForRenderUpdate)
                return;
            if (neighbor != null && neighbor.isScheduledForPopulation)
            {
                allNeighborsPopulated = false;
                return;
            }
            allNeighborsPopulated = true;
            for (int i = 0; i < 6; i++)
            {
                Chunk neighborC = neighborChunks[i];
                if (neighborC == null)
                    continue;
                if (neighborChunks[i].isScheduledForPopulation)
                {
                    allNeighborsPopulated = false;
                    return;
                }
            }
        }

        public void setVoxelIDAt(int x, int y, int z, int id)
        {
            if (x < 0 || y < 0 || z < 0)
                return;
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            if (index >= CHUNK_SIZE_CUBED)
                return;
            uint vAt = voxels[index];
            vAt &= 0b1111_1111_1111_1111_1111_1100_0000_0000;
            vAt |= (uint)id;
            voxels[index] = vAt;
        }

        public void setVoxelLightLevelAt(int x, int y, int z, int r, int g, int b)
        {
            if (x < 0 || y < 0 || z < 0)
                return;
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            if (index >= CHUNK_SIZE_CUBED)
                return;
            uint vAt = voxels[index];
            vAt &= 0b1111_1110_0000_0000_0000_0011_1111_1111;
            vAt |= (uint)(r << 10 | g << 15 | b << 20);
            voxels[index] = vAt;
        }

        public Vector3i getVoxelLightLevelAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0)
                return Vector3i.Zero;
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            if (index >= CHUNK_SIZE_CUBED)
                return Vector3i.Zero;
            uint vAt = voxels[index];
            uint r = (vAt >> 10) & 0b0000_0000_0000_0000_0000_0000_0001_1111;
            uint g = (vAt >> 15) & 0b0000_0000_0000_0000_0000_0000_0001_1111;
            uint b = (vAt >> 20) & 0b0000_0000_0000_0000_0000_0000_0001_1111;
            return new Vector3i((int)r, (int)g, (int)b);
        }

        public uint getVoxelAt(int x, int y, int z)
        {
            int index = x << X_SHIFT | z << Z_SHIFT | y;
            return index >= CHUNK_SIZE_CUBED ? 0 : voxels[index];
        }

        public ref AABB getBoundsRef()
        {
            return ref chunkBounds;
        }

        public AABB getBounds()
        {
            return chunkBounds;
        }
    }
}