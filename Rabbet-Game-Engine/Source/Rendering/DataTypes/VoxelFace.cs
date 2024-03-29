﻿using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public struct VoxelFace
    {
        public static readonly int SIZE_IN_BYTES = 4;
        private uint data;

        /// <summary>
        /// Tightly packed vertex information for a voxel face vertex.
        /// </summary>
        /// <param name="voxelID">the voxel ID of this voxel. used for texture indexing. range = 0 to 255</param>
        /// <param name="x">the x position of this vertex in this chunk. Chunk relative. range = 0 to 31</param>
        /// <param name="y">the y position of this vertex in this chunk. Chunk relative. range = 0 to 31</param>
        /// <param name="z">the z position of this vertex in this chunk. Chunk relative. range = 0 to 31</param>
        /// <param name="lightLevel">the light level of the face this vertex is on. range = 0 to 63.</param>
        /// <param name="orientation">the orientation of this face. Range = 0 to 5. 0 to 2 is positive facing xyz faces. 3 to 5 is negative facing xyz faces.</param>
        public VoxelFace(int x, int y, int z, Vector3i lightLevel, int orientation, int voxelID)
        {
            data = 0U;
            setPos(x, y, z);
            setLightLevel(lightLevel);
            setOrientation(orientation);
            setVoxelID(voxelID);
        }

        public void setPos(int x, int y, int z)
        {
            data &= 0x0001FFFFU;//clear pos bits
            data |= (uint)(x << Chunk.X_SHIFT | z << Chunk.Z_SHIFT | y) << 17;
        }

        public void setLightLevel(Vector3i level)
        {
            data &= 0xFFFE07FFU;//clear light level bits
            data |= (uint)MathUtil.max3(level.X, level.Y, level.Z) << 11;
        }

        public void setOrientation(int o)
        {
            data &= 0xFFFFF8FFU;//clear orientation bits
            data |= (uint)o << 8;
        }

        public void setVoxelID(int id)
        {
            data &= 0xFFFFFF00U;
            data |= (uint)id;
        }
    }
}