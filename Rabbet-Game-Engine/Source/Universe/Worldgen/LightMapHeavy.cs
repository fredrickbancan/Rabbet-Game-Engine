namespace RabbetGameEngine
{
    /// <summary>
    /// Stores the 6 bit light level values tightly packed (every 4th stores the light value in the previous 3 bytes left over 2 bits.)
    /// Doing this saves 65.536 KB of ram per chunk.
    /// Thats 4.194 MB over 64 chunks saved!
    /// The maths used to store the data should be quite fast.
    /// </summary>
    public class LightMapHeavy
    {
        private byte[] data;

        public LightMapHeavy(int size)
        {
            data = new byte[size];
        }

        public void setLightLevelAt(int x, int y, int z, int l)
        {
            int index = x << Chunk.X_SHIFT | z << Chunk.Z_SHIFT | y;
            data[index] = (byte)l;
        }

        public int getLightLevelAt(int x, int y, int z)
        {
            int index = x << Chunk.X_SHIFT | z << Chunk.Z_SHIFT | y;
            return data[index];
        }
    }
}
