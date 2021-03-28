namespace RabbetGameEngine.Rendering
{
    public static class RenderConstants
    {
        public static readonly int VEC3_SIZE_BYTES = sizeof(float) * 3;
        public static readonly int MAT4_SIZE_BYTES = sizeof(float) * 16;
        public static readonly int INIT_BATCH_ARRAY_SIZE = 32;
        public static readonly int MAX_BATCH_BUFFER_SIZE_BYTES = 8388608;
        public static readonly int MAX_BATCH_TEXTURES = 8;
    }
}
