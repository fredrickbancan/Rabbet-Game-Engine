using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coictus
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
            loadAllTexturesRecursive(ResourceUtil.getTextureFileDir());
            loadAllTexturesRecursive(ResourceUtil.getFontFileDir());
        }

        private static void loadAllTexturesRecursive(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);
            string[] allDirectories = Directory.GetDirectories(directory);
            foreach (string file in allFiles)
            {
                if (file.Contains(".png"))
                {
                    tryAddNewTexture(file);
                }
            }

            foreach (string dir in allDirectories)
            {
                loadAllTexturesRecursive(dir);
            }
        }

        private static void tryAddNewTexture(string textureDir)
        {
            string shaderName = textureDir.Replace(ResourceUtil.getTextureFileDir(), "").Replace(ResourceUtil.getFontFileDir(), "");//removes directory
            Texture addingTexture = new Texture(textureDir, false);
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
                Application.error("TextureUtil could not find texture named: " + name + " in global list, returning null.");
            }
            return success;
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
