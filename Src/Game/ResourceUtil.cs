using System;
using System.IO;

namespace RabbetGameEngine
{
    /*A class containing functions to gelp find resources. This class will make it much easier to change resource directories.*/
    public static class ResourceUtil
    {
        public static readonly string workingDir = Directory.GetCurrentDirectory();
        public static readonly string libsDir = workingDir + @"\Libs\";
        public static readonly string resDir = workingDir + @"\Res\";

        public static void init()
        {
            try
            {
                Directory.CreateDirectory(getIconFileDir());
                Directory.CreateDirectory(getTextureFileDir());
                Directory.CreateDirectory(getFontFileDir());
                Directory.CreateDirectory(getShaderFileDir());
                Directory.CreateDirectory(getOBJFileDir());
                Directory.CreateDirectory(getSoundFileDir());
                Directory.CreateDirectory(getScreenShotFileDir());
            }
            catch(Exception e)
            {
                Application.warn("Error initializing resource utilities! Error message: " + e.Message);
            }
        }

        public static string getIconFileDir(string fileName = "")
        {
            return resDir + @"Icon\" + fileName;
        }
        public static string getTextureFileDir(string fileName = "")
        {
            return resDir + @"Texture\" + fileName;
        }

        public static string getFontFileDir(string fileName = "")
        {
            return resDir + @"Font\" + fileName;
        }

        public static string getShaderFileDir(string shaderName = "")
        {
            return resDir + @"Shaders\" + shaderName;
        }
        public static string getOBJFileDir(string modelName = "")
        {
            return resDir + @"OBJ\" + modelName;
        }

        public static string getSoundFileDir(string soundName = "")
        {
            return resDir + @"Sounds\" + soundName;
        }

        public static string getScreenShotFileDir(string imageName = "")
        {
            return resDir + @"ScreenShots\" + imageName;
        }
    }
}
