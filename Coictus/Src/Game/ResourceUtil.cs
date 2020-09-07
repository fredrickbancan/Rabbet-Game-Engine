using System;

namespace Coictus
{
    /*A class containing functions to gelp find resources. This class will make it much easier to change resource directories.*/
    public static class ResourceUtil
    {
        public static String getTextureFileDir(String fileName)
        {
            return @"..\..\Res\Texture\" + fileName;
        }

        public static String getFontFileDir(String fileName)
        {
            return @"..\..\Res\Font\" + fileName;
        }

        public static String getShaderFileDir(String shaderName)
        {
            return @"..\..\Res\Shaders\" + shaderName;
        }
        public static String getOBJFileDir(String modelName)
        {
            return @"..\..\Res\OBJ\" + modelName;
        }
    }
}
