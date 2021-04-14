namespace RabbetGameEngine
{
    /// <summary>
    /// Same as the voxel struct but without the chunk index bits to save memory
    /// </summary>
    public struct CheapVoxel
    {
        public byte id;
        public byte lightLevel;

        public CheapVoxel(byte id, byte lightLevel)
        {
            this.id = id;
            this.lightLevel = lightLevel;
        }

    }
}
