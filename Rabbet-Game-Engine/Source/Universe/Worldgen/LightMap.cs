namespace RabbetGameEngine
{
    public class LightMap
    {
        private byte[] data;

        public LightMap(int size)
        {
            data = new byte[(int)(System.MathF.Ceiling((float)size / 3.0F))];
        }

        public void setLightLevelAt(int x, int y, int z, byte l)
        {
            int index = x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y;
            if (index != 0 && index % 3 == 0) { storeLightLevelInPreviousLeftOverBits(index / 3, l); return; }
            data[index - (index / 3)] = (byte)(l << 2);
        }

        public byte getLightLevelAt(int x, int y, int z)
        {
            int index = x << Chunk.CHUNK_X_SHIFT | z << Chunk.CHUNK_Z_SHIFT | y;
            if (index != 0 && index % 3 == 0) return combinePreviousLeftOverBits(index - (index / 3));
            return (byte)(data[index - (index / 3)] >> 2);
        }

        private byte combinePreviousLeftOverBits(int i)
        {
            return (byte)(((data[i - 2] & 3) << 4) | ((data[i - 1] & 3) << 2) | data[i] & 3);
        }

        private void storeLightLevelInPreviousLeftOverBits(int i, byte l)
        {
            data[i - 2] |= (byte)(l >> 4);
            data[i - 1] |= (byte)((l & 12) >> 2);
            data[i] |= (byte)(l & 3);
        }
    }
}
