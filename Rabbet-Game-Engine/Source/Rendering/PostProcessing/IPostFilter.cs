namespace RabbetGameEngine
{
    /// <summary>
    /// Interface for a post processing filter class
    /// </summary>
    public interface IPostFilter
    {
        /// <summary>
        /// Initalizes this filters resources
        /// Resolution needed for setting up result framebuffer size so filters process at the provided resolution
        /// </summary>
        public void init(int initalResWidth, int initalResHeight);

        /// <summary>
        /// Processes the provided texture and returns the ID of the result texture.
        /// Result texture will be resolution of window
        /// </summary>
        /// <param name="textureID">The source texture</param>
        /// <param name="width">The source texture Width</param>
        /// <param name="height">The source texture Height</param>
        /// <returns>The texture id of the result texture</returns>
        public int processImage(int textureID, int width, int height);

        /// <summary>
        /// To be called when the window and/or offscreen framebuffer is resized
        /// </summary>
        /// <param name="newResWidth">new width for filter</param>
        /// <param name="newResHeight">new height for filter</param>
        public void onResize(int newResWidth, int newResHeight);

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
