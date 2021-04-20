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
        /// <param name="metadata">the metadata of this voxel. Used for texture atlas pages indexing. range = 0 to 7</param>
        /// <param name="orientation">the orientation of this face. Range = 0 to 5. 0 to 2 is positive facing xyz faces. 3 to 5 is negative facing xyz faces.</param>
        /// <param name="damage">the damage of this face. Range = 0 to 3.</param>
        public VoxelFace(byte x, byte y, byte z, byte lightLevel, byte orientation,  byte voxelID)
        {
            data = 0U;
            setPos(x, y, z);
            setLightLevel(lightLevel);
            setOrientation(orientation);
            setVoxelID(voxelID);
        }

        public void setPos(byte x, byte y, byte z)
        {
            data &= 0x0001FFFFU;//clear pos bits
            data |= (uint)(x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y) << 17;
        }
        public void setLightLevel(byte level)
        {
            data &= 0xFFFFC0FFU;//clear light level bits
            data |= (uint)level << 11;
        }

        public void setOrientation(byte o)
        {
            data &= 0xFFFFFFE3U;//clear orientation bits
            data |= (uint)o << 8;
        }

        public void setVoxelID(byte id)
        {
            data &= 0xFFFFFF00U;
            data |= (uint)id;
        }
    }
}
