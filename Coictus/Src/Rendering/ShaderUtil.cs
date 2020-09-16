using System.Collections.Generic;
using System.IO;

namespace Coictus
{
    /*Class for pre-loading all Shaders found on file at launch so they can be used
      throughout the game without having to be re-loaded and re-allocated.*/
    public static class ShaderUtil
    {
        private static Dictionary<string, Shader> shaders = null;
        public static void loadAllFoundShaderFiles()
        {
            shaders = new Dictionary<string, Shader>();
            loadAllShadersRecursive(ResourceUtil.getShaderFileDir());
        }

        private static void loadAllShadersRecursive(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);
            string[] allDirectories = Directory.GetDirectories(directory);
            foreach (string file in allFiles)
            {
                if (file.Contains(".shader"))
                {
                    tryAddNewShader(file);
                }
            }

            foreach (string dir in allDirectories)
            {
                loadAllShadersRecursive(dir);
            }
        }

        private static void tryAddNewShader(string shaderDir)
        {
            string shaderName = shaderDir.Replace(ResourceUtil.getShaderFileDir(), "");//removes directory
            Shader addingShader = new Shader(shaderDir);
            shaders.Add(shaderName, addingShader);
        }

        /*Returns true if the requested shader was found in the global list*/
        public static bool tryGetShader(string name, out Shader shader)
        {
            bool success = shaders.TryGetValue(name, out shader);
            if (!success)
            {
                Application.error("ShaderUtil could not find shader named: " + name + " in global list, assigning debug shader.");
            }
            return success;
        }

        public static int getShaderCount()
        {
            return shaders.Count;
        }

        public static void deleteAll()
        {
            foreach(Shader shader in shaders.Values)
            {
                shader.Dispose();
            }
        }
    }
}
