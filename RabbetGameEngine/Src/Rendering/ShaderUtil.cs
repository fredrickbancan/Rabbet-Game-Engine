using System.Collections.Generic;
using System.IO;

namespace RabbetGameEngine
{
    /*Class for pre-loading all Shaders found on file at launch so they can be used
      throughout the game without having to be re-loaded and re-allocated.*/
    public static class ShaderUtil
    {
        public static readonly string guiCutoutName = "GuiCutout";
        public static readonly string text2DName = "GuiText";
        public static readonly string text3DName = "";
        public static readonly string trianglesName = "Static_F";
        public static readonly string linesName = "StaticLines";
        public static readonly string pointsName = "StaticPParticles_F";
        public static readonly string pointsTransparentName = "StaticPParticles_FT";
        public static readonly string lerpPointsName = "LerpPParticles_F";
        public static readonly string lerpTrianglesName = "Lerp_F";
        public static readonly string lerpLinesName = "Lerp_F";
        public static readonly string lerpPointsTransparentName = "LerpPParticles_FT";
        public static readonly string lerpTrianglesTransparentName = "Lerp_FT";
        public static readonly string trianglesTransparentName = "Static_FT";

        private static Dictionary<string, Shader> shaders = null;
        public static void loadAllFoundShaderFiles()
        {
            shaders = new Dictionary<string, Shader>();
            Shader debugShader = new Shader("debug");
            shaders.Add("debug", debugShader);
            loadAllShadersRecursive(ResourceUtil.getShaderFileDir());
        }

        private static void loadAllShadersRecursive(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);
            string[] allDirectories = Directory.GetDirectories(directory);
            foreach (string file in allFiles)
            {
                if (file.Contains(""))
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
            string shaderName = Path.GetFileName(shaderDir).Replace(".shader", "");//removes directory
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

        public static Shader getShader(string name)
        {
            Shader result = null;
            tryGetShader(name, out result);
            return result;
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
