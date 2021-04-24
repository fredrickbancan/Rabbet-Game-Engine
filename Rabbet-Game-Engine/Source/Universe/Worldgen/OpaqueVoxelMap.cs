using System;

namespace RabbetGameEngine
{
    /// <summary>
    /// Class for containing information on a chunks opaque voxels.
    /// Contains one bit per voxel position. Each bit will be 1 if the voxel at that corrosponding position is an opaque voxel.
    /// </summary>
    public class OpaqueVoxelMap
    {
        /// <summary>
        /// each int is a y column of voxels (Works best with 32 voxel size chunks)
        /// </summary>
        private int[] data = null;

        public OpaqueVoxelMap()
        {
            data = new int[Chunk.CHUNK_SIZE_SQUARED];
        }
        
        public void loadFromVoxelData(byte[] voxels)
        {
            Array.Clear(data, 0, data.Length);
            for (int i = 0; i < Chunk.CHUNK_SIZE_CUBED; i++)
            {
                int isOpaqueInt = Convert.ToInt32(VoxelType.isVoxelOpaque(voxels[i]));
                data[i >> 5] |= isOpaqueInt << (i & 31);
            }
        }

        public bool isVoxelOpaque(int x, int y, int z)
        {
            int index = x << Chunk.X_SHIFT | z << Chunk.Z_SHIFT | y;
            return Convert.ToBoolean(data[index >> 5] & (1 << y));
        }

        public void setVoxelOpaque(int x, int y, int z, bool opaque)
        {
            int index = x << Chunk.X_SHIFT | z << Chunk.Z_SHIFT | y;
            if (opaque) { data[index >> 5] |= 1 << y; return; }
            data[index >> 5] &= ~(1 << y);
        }

    }
}
