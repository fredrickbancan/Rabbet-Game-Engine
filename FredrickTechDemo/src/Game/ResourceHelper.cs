using System;

namespace FredrickTechDemo
{
    public static class ResourceHelper
    {
        public static String getTextureFileDir(String fileName)
        {
            return @"..\..\res\texture\" + fileName;
        }

        public static String getFontTextureFileDir(String fileName)
        {
            return @"..\..\res\font\" + fileName;
        }

        public static String getShaderFileDir(String shaderName)
        {
            return @"..\..\res\shaders\" + shaderName;
        }
    }
}
