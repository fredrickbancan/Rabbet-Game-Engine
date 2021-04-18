namespace RabbetGameEngine
{
    /// <summary>
    /// Stores the 6 bit light level values tightly packed (every 4th stores the light value in the previous 3 bytes left over 2 bits.)
    /// Doing this saves 65.536 KB of ram per chunk.
    /// Thats 4.194 MB over 64 chunks saved!
    /// The maths used to store the data should be quite fast.
    /// </summary>
    public class LightMap
    {
        private byte[] data;

        public LightMap(int size)
        {
            data = new byte[(int)(System.MathF.Ceiling((float)size - (float)size / 4))];
        }

        public void setLightLevelAt(int x, int y, int z, byte l)
        {
            int index = x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y;
            int storeIndex = index - index / 4;
            if (index != 0 && index % 3 == 0) { storeLightLevelInPreviousLeftOverBits(storeIndex-1, l); return; }
            data[storeIndex] |= (byte)(l & 63);
        }

        public byte getLightLevelAt(int x, int y, int z)
        {
            int index = x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y;
            int storeIndex = index - index / 4;
            if (index != 0 && index % 3 == 0) return combinePreviousLeftOverBits(storeIndex-1);
            return (byte)(data[storeIndex] & 63);
        }

        private byte combinePreviousLeftOverBits(int i)
        {
            return (byte)((data[i - 2] & 192) >> 2 | (data[i - 1] & 192) >> 4 | (data[i] & 192) >> 6);
        }

        private void storeLightLevelInPreviousLeftOverBits(int i, byte l)
        {
            data[i - 2] |= (byte)((l & 48) << 2);
            data[i - 1] |= (byte)((l & 12) << 4);
            data[i] |= (byte)((l & 3) << 6);
        }
    }
}
