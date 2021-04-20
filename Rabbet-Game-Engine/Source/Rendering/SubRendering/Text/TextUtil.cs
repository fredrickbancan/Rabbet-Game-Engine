using System.Collections.Generic;
using System.IO;

namespace RabbetGameEngine
{
    public static class TextUtil
    {
        public static readonly string textShaderName = "GuiText";
        public static readonly int defaultScreenEdgePadding = 5; //5 pixels

        private static Dictionary<string, FontFace> fonts = new Dictionary<string, FontFace>();

        /*Returns true if the requested font was found in the global list*/
        public static bool tryGetFont(string name, out FontFace font)
        {
            name = name.ToLower();
            bool success = fonts.TryGetValue(name, out font);
            if (!success)
            {
                Application.error("TextUtil could not find font named: " + name + " in global list, returning null.");
            }
            return success;
        }

        /*Goes through the font file directory and adds all fonts*/
        public static void loadAllFoundTextFiles()
        {
            try
            {
                string[] allFileDirectoriesAndName = Directory.GetFiles(ResourceUtil.getFontFileDir());
                foreach (string dir in allFileDirectoriesAndName)
                {
                    if (dir.Contains(".fnt"))
                    {
                        string fontName = dir.Replace(ResourceUtil.getFontFileDir(), "").Replace(".fnt", "");//removes directory and file extension
                        tryAddNewFontToGlobalList(fontName.ToLower());
                    }
                }
            }
            catch (IOException e)
            {
                Application.error(e.Message);
            }
        }

        private static void tryAddNewFontToGlobalList(string fontName)
        {
            FontFace addingFont = new FontFace(fontName);
            if (addingFont.successfullyInitialized())
            {
                fonts.Add(fontName, addingFont);
            }
            else
            {
                Application.warn("TextUtil Could not process font: " + fontName);
            }
        }

        public static string getTextureNameForScreenFont(FontFace font)
        {
            return font.getFontName() + "";
        }
    }
}
