namespace RabbetGameEngine.SubRendering
{
    /// <summary>
    /// Class for holding info about this machines rendering capabilities.
    /// </summary>
    public static class RenderAbility
    {
        private static int maxUniComponents;
        private static int maxUniVector3;
        private static int maxUniVector4;
        private static int maxUniMat4;
        public static void setMaxUniformComponents(int i)
        {
            Application.debugPrint("Max vertex uniform components: " + (maxUniComponents = i));
            Application.debugPrint("Max vertex uniform vector3: " + (maxUniVector3 = i / 3));
            Application.debugPrint("Max vertex uniform vector4: " + (maxUniVector4 = i / 4));
            Application.debugPrint("Max vertex uniform Mat4: " + (maxUniMat4 = i / 16));
        }

        /// <summary>
        /// The maximum number of 4 byte int/float etc components that can fit in a single uniform on this system.
        /// </summary>
        public static int maxUniformComponents { get => maxUniComponents; }

        /// <summary>
        /// The maximum number of Vector3's that can fit in a single uniform on this system.
        /// </summary>
        public static int maxUniformVector3 { get => maxUniVector3; }

        /// <summary>
        /// The maximum number of Vector4's that can fit in a single uniform on this system.
        /// </summary>
        public static int maxUniformVector4 { get => maxUniVector4; }

        /// <summary>
        /// The maximum number of Matrix4's that can fit in a single uniform on this system.
        /// </summary>
        public static int maxUniformMat4 { get => maxUniMat4; }
    }
}
