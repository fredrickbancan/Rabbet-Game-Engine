namespace RabbetGameEngine
{
    public struct VoxelVertex
    {
        public static readonly int SIZE_IN_BYTES = 5;
        private uint data;
        public byte id;

        /// <summary>
        /// Tightly packed vertex information for a voxel face vertex.
        /// </summary>
        /// <param name="voxelID">the voxel ID of this voxel. used for texture indexing. range = 0 to 255</param>
        /// <param name="x">the x position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="y">the y position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="z">the z position of this vertex in this chunk. Chunk relative. range = 0 to 63</param>
        /// <param name="corner">signifies which corner of the quad face this vertex is. range = 0 to 3. 0 = top right, 1 = top left, 2 = bottom right, 3 = bottom left.</param>
        /// <param name="normal">the normal of the face this vertex is on. range = 0 to 5 . 0 to 2 is positive x,y,z. 3 to 5 is negative x,y,z</param>
        /// <param name="lightLevel">the light level of the face this vertex is on. range = 0 to 63.</param>
        /// <param name="metadata">the metadata of this voxel. Used for texture atlas pages indexing. range = 0 to 7</param>
        public VoxelVertex(byte voxelID, byte x, byte y, byte z, byte corner, byte normal, byte lightLevel, byte metadata)
        {
            data = 0U;
            id = voxelID;
            setPos(x, y, z);
            setCorner(corner);
            setNormal(normal);
            setLightLevel(lightLevel);
            setMetaData(metadata);
        }

        public void setPos(byte x, byte y, byte z)
        {
            data &= 0x00003FFFU;//clear pos bits
            data |= (uint)(x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y) << 14;
        }

        public void setCorner(byte c)
        {
            data &= 0xFFFFCFFFU;//clear corner bits
            data |= (uint)c << 12;
        }

        public void setNormal(byte n)
        {
            data &= 0xFFFFF1FFU;//clear normal bits
            data |= (uint)n << 9;
        }

        public void setLightLevel(byte level)
        {
            data &= 0xFFFFFE07U;//clear light level bits
            data |= (uint)level << 3;
        }

        public void setMetaData(byte md)
        {
            data &= 0xFFFFFFF8U;//clear metadata bits
            data |= (uint)md;
        }

        public static void configureLayout(VertexBufferLayout vbl)
        {
            vbl.add(OpenTK.Graphics.OpenGL.VertexAttribIntegerType.UnsignedInt, 1);
            vbl.add(OpenTK.Graphics.OpenGL.VertexAttribIntegerType.UnsignedByte, 1);
        }
    }
}
