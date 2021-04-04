namespace RabbetGameEngine
{
    /// <summary>
    /// Interface for a post processing filter class
    /// </summary>
    public interface IPostFilter
    {
        /// <summary>
        /// Initalizes this filters resources
        /// Resolution needed for setting up result framebuffer size so filters return textures with full screen resolution.
        /// </summary>
        public void init();

        /// <summary>
        /// Processes the provided texture and returns the ID of the result texture.
        /// Result texture will be resolution of window
        /// </summary>
        /// <param name="textureID">The source texture</param>
        /// <returns>The texture id of the result texture</returns>
        public int processImage(int textureID);

        /// <summary>
        /// returns width pixels of result image texture
        /// </summary>
        public int getResultWidth();

        /// <summary>
        /// returns Height pixels of result image texture
        /// </summary>
        public int getResultHeight();

        /// <summary>
        /// cleans up all opengl resources
        /// </summary>
        public void delete();
    }
}
