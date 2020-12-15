using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbetGameEngine
{
    /*Class for pre-loading all textures found on file at launch so they can be used
      throughout the game without having to be re-loaded and re-allocated.*/
    public static class TextureUtil
    {
        private static Dictionary<string, Texture> textures = null;
        public static void loadAllFoundTextureFiles()
        {
            textures = new Dictionary<string, Texture>();
            textures.Add("none", new Texture("none", false));//first texture will be the null texture
            textures.Add("debug", new Texture());//second texture will be the debug texture
            textures.Add("dither", new Texture("dither", false));//third texture will be the dithering texture
            textures.Add("white", new Texture("white", false));//fourth texture will be a flat white texture
            loadAllTexturesRecursive(ResourceUtil.getTextureFileDir(), false);
            loadAllTexturesRecursive(ResourceUtil.getFontFileDir(), true);
            loadAllTexturesRecursive(ResourceUtil.getIconFileDir(), false);
        }

        private static void loadAllTexturesRecursive(string directory, bool filtering)
        {
            try
            {
                string[] allFiles = Directory.GetFiles(directory);
                string[] allDirectories = Directory.GetDirectories(directory);
                foreach (string file in allFiles)
                {
                    if (file.Contains(".png"))
                    {
                        tryAddNewTexture(file, filtering);
                    }
                }

                foreach (string dir in allDirectories)
                {
                    loadAllTexturesRecursive(dir, filtering);
                }
            }
            catch (Exception e)
            {
                Application.error(e.Message);
            }
        }

        private static void tryAddNewTexture(string textureDir, bool filtering)
        {
            string shaderName = Path.GetFileName(textureDir).Replace(".png", "");//removes directory
            Texture addingTexture = new Texture(textureDir, filtering);
            textures.Add(shaderName, addingTexture);
        }

        /*Returns true if the requested texture was found in the global list*/
        public static bool tryGetTexture(string name, out Texture texture)
        {
            if(name == "none")
            {
                texture = textures.ElementAt(0).Value;
                return false;
            }
            if(name == "debug")
            {
                texture = textures.ElementAt(1).Value;
                return true;
            }

            bool success = textures.TryGetValue(name, out texture);
            if (!success)
            {
                Application.error("TextureUtil could not find texture named: " + name + " in global list, assigning debug texture.");
                texture = textures.ElementAt(1).Value;
            }
            return success;
        }

        public static Texture getTexture(string name)
        {
            Texture result = null;
            tryGetTexture(name, out result);
            return result;
        }
        public static int getTextureCount()
        {
            return textures.Count;
        }

        public static void deleteAll()
        {
            foreach (Texture tex in textures.Values)
            {
                tex.Dispose();
            }
        }
    }
}
