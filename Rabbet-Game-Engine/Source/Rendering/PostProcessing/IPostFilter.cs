namespace RabbetGameEngine
{
    /// <summary>
    /// Interface for a post processing filter class
    /// </summary>
    public interface IPostFilter
    {
        /// <summary>
        /// Initalizes this filters resources
        /// </summary>
        public void init();

        /// <summary>
        /// Processes the provided texture and returns the ID of the result texture.
        /// </summary>
        /// <param name="textureID">The source texture</param>
        /// <returns>The texture id of the result texture</returns>
        public int processImage(int textureID);

        /// <summary>
        /// cleans up all opengl resources
        /// </summary>
        public void delete();
    }
}
