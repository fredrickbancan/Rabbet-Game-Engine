namespace RabbetGameEngine
{
    public struct Voxel
    {
        public static readonly int SIZE_IN_BYTES = sizeof(uint);
        public static readonly float VOXEL_PHYSICAL_SIZE = 0.25F;

        public uint data;

        /// <summary>
        /// Information for voxels to be rendered. 
        /// Containts unsigned int for holding the voxel data.
        /// First 8 bits is voxel id. Next 18 bits is chunk index. Next 6 bits is lighting level. 
        /// </summary>
        /// <param name="id">id of this voxel</param>
        /// <param name="chunkIndex">index into the chunk this voxel sits. Ranges from 0 to 262,143</param>
        /// <param name="lightLevel">light level of this voxel. Ranges from 0 to 63</param>
        public Voxel(byte id, int chunkIndex, byte lightLevel)
        {
            data = 0;
            setID(id);
            setChunkIndex(chunkIndex);
            setLightLevel(lightLevel);
        }
        
        public void setID(byte id)
        {
            data &= ~0xFF000000;//clear id bits
            data |= (uint)id << 24;//set id bits
        }

        public void setChunkIndex(int index)
        {
            data &= 0xFF00003F;//clear index bits
            data |= (uint)index << 6;//set index bits
        }

        public void setLightLevel(byte lightLevel)
        {
            data &= 0xFFFFFFC0;//clear light bits
            data |= lightLevel;//set light bits
        }

        public byte getID()
        {
            return (byte)(data >> 24);
        }

        public int getChunkIndex()
        {
            return (int)((data & 0x00FFFFFF) >> 6);
        }

        public byte getLightLevel()
        {
            return (byte)(data & 0x3F);
        }

        public static void configureLayout(VertexBufferLayout vbl)
        {
            vbl.add(OpenTK.Graphics.OpenGL.VertexAttribIntegerType.UnsignedInt, 1);
        }
    }
}
