namespace RabbetGameEngine
{
    /*A class containing functions to gelp find resources. This class will make it much easier to change resource directories.*/
    public static class ResourceUtil
    {
        public static string getIconFileDir(string fileName = "")
        {
            return @"..\..\Res\Icon\" + fileName;
        }
        public static string getTextureFileDir(string fileName = "")
        {
            return @"..\..\Res\Texture\" + fileName;
        }

        public static string getFontFileDir(string fileName = "")
        {
            return @"..\..\Res\Font\" + fileName;
        }

        public static string getShaderFileDir(string shaderName = "")
        {
            return @"..\..\Res\Shaders\" + shaderName;
        }
        public static string getOBJFileDir(string modelName = "")
        {
            return @"..\..\Res\OBJ\" + modelName;
        }
    }
}
