using System.Collections.Generic;
using System.IO;

namespace RabbetGameEngine
{
    /*Class for pre-loading all Shaders found on file at launch so they can be used
      throughout the game without having to be re-loaded and re-allocated.*/
    public static class ShaderUtil
    {
        public static readonly string filterGBlurName = "Filter_GBlur";
        public static readonly string frameBufferFinalName = "FrameBuffer_Final";
        public static readonly string frameBufferPassThroughName = "FrameBuffer_PassThrough";
        public static readonly string frameBufferCombineAverageName = "FrameBuffer_CombineAverage";
        public static readonly string frameBufferMainName = "FrameBuffer_Main";
        public static readonly string sunName = "Sun";
        public static readonly string skyboxName = "Skybox";
        public static readonly string skyboxShroudName = "SkyboxShroud";
        public static readonly string starsName = "Stars";
        public static readonly string moonsName = "Moons";
        public static readonly string guiCutoutName = "GuiCutout";
        public static readonly string guiLinesName = "GuiLines";
        public static readonly string guiTransparentName = "GuiTransparent";
        public static readonly string text2DName = "GuiText";
        public static readonly string text3DName = "StaticText3D_F";
        public static readonly string lerpText3DName = "LerpText3D_F";
        public static readonly string trianglesName = "Static_F";
        public static readonly string quadsName = "Static_F";
        public static readonly string linesName = "StaticLines";
        public static readonly string iSpheresName = "StaticISpheres_F";
        public static readonly string iSpheresTransparentName = "StaticISpheres_FT";
        public static readonly string lerpISpheresName = "LerpISpheres_F";
        public static readonly string lerpTrianglesName = "Lerp_F";
        public static readonly string lerpQuadsName = "Lerp_F";
        public static readonly string lerpLinesName = "Lerp_F";
        public static readonly string lerpISpheresTransparentName = "LerpISpheres_FT";
        public static readonly string lerpTrianglesTransparentName = "Lerp_FT";
        public static readonly string lerpQuadsTransparentName = "Lerp_FT";
        public static readonly string trianglesTransparentName = "Static_FT";
        public static readonly string quadsTransparentName = "Static_FT";
        public static readonly string spriteCylinderName = "StaticSpriteCylinder_F";

        public static readonly string fileExtension = ".shader";
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
                if (file.Contains(fileExtension))
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
            string shaderName = Path.GetFileName(shaderDir).Replace(fileExtension, "");//removes directory
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
            Application.infoPrint("ShaderUtil deleting " + shaders.Count + " loaded shaders...");
            foreach (Shader shader in shaders.Values)
            {
                shader.Dispose();
            }
            Application.infoPrint("Done");
        }
    }
}
