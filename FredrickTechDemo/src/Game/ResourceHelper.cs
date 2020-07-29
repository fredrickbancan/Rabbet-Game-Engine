using System;

namespace FredrickTechDemo
{
    public static class ResourceHelper
    {
        public static String getTextureFileDir(String fileName)
        {
            return @"..\..\Res\Texture\" + fileName;
        }

        public static String getFontTextureFileDir(String fileName)
        {
            return @"..\..\Res\Font\" + fileName;
        }

        public static String getShaderFileDir(String shaderName)
        {
            return @"..\..\Res\Shaders\" + shaderName;
        }
    }
}
