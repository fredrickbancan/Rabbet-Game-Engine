namespace RabbetGameEngine
{
    /*A class containing functions to gelp find resources. This class will make it much easier to change resource directories.*/
    public static class ResourceUtil
    {
        public static string getResourceFileDir()
        {
            return @"Res\";
        }
        public static string getIconFileDir(string fileName = "")
        {
            return getResourceFileDir() + @"Icon\" + fileName;
        }
        public static string getTextureFileDir(string fileName = "")
        {
            return getResourceFileDir() + @"Texture\" + fileName;
        }

        public static string getFontFileDir(string fileName = "")
        {
            return getResourceFileDir() + @"Font\" + fileName;
        }

        public static string getShaderFileDir(string shaderName = "")
        {
            return getResourceFileDir() + @"Shaders\" + shaderName;
        }
        public static string getOBJFileDir(string modelName = "")
        {
            return getResourceFileDir() + @"OBJ\" + modelName;
        }

        public static string getSoundFileDir(string soundName = "")
        {
            return getResourceFileDir() + @"Sounds" + soundName;
        }
    }
}
