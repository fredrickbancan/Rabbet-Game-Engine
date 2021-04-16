namespace RabbetGameEngine
{
    public struct VoxelFace
    {
        public static readonly int SIZE_IN_BYTES = 5;
        private uint data;//last 2 bits are spare for now
        public byte id;

        /// <summary>
        /// Tightly packed vertex information for a voxel face vertex.
        /// </summary>
        /// <param name="voxelID">the voxel ID of this voxel. used for texture indexing. range = 0 to 255</param>
        /// <param name="x">the x position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="y">the y position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="z">the z position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="lightLevel">the light level of the face this vertex is on. range = 0 to 63.</param>
        /// <param name="metadata">the metadata of this voxel. Used for texture atlas pages indexing. range = 0 to 7</param>
        /// <param name="orientation">the orientation of this face. Range = 0 to 5. 0 to 2 is positive facing xyz faces. 3 to 5 is negative facing xyz faces.</param>
        public VoxelFace(byte voxelID, byte x, byte y, byte z, byte lightLevel, byte metadata, byte orientation)
        {
            data = 0U;
            id = voxelID;
            setPos(x, y, z);
            setLightLevel(lightLevel);
            setMetaData(metadata);
            setOrientation(orientation);
        }

        public void setPos(byte x, byte y, byte z)
        {
            data &= 0x00003FFFU;//clear pos bits
            data |= (uint)(x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y) << 14;
        }

        public void setOrientation(byte o)
        {
            data &= 0xFFFFFFE3U;//clear orientation bits
            data |= (uint)o << 2;
        }

        public void setLightLevel(byte level)
        {
            data &= 0xFFFFC0FFU;//clear light level bits
            data |= (uint)level << 8;
        }

        public void setMetaData(byte md)
        {
            data &= 0xFFFFFF1FU;//clear metadata bits
            data |= (uint)md << 5;
        }

        public static void configureLayout(VertexBufferLayout vbl)
        {
            vbl.add(OpenTK.Graphics.OpenGL.VertexAttribIntegerType.UnsignedInt, 1);
            vbl.add(OpenTK.Graphics.OpenGL.VertexAttribIntegerType.UnsignedByte, 1);
        }
    }
}
