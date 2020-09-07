using Coictus.SubRendering.GUI.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Coictus.GUI.Text
{
    //enum for text alignment choices
    public enum TextAlign { LEFT, CENTER, RIGHT };
    public static class TextUtil
    {
        public static readonly String textShaderDir = ResourceUtil.getShaderFileDir(@"GUI\GuiTextShader.shader");
        public static readonly float defaultFontSize = 0.02F;
        public static readonly int defaultScreenEdgePadding = 5; //5 pixels

        private static Dictionary<String, FontFace> fonts = new Dictionary<String, FontFace>();

        /*Returns true if the requested font was found in the global list*/
        public static bool tryGetFont(String name, out FontFace font)
        {
            bool success = fonts.TryGetValue(name, out font);
            if(!success)
            {
                Application.error("TextUtil could not find font named: " + name + " in global list, returning null.");
            }
            return success;
        }

        /*Goes through the font file directory and adds all fonts*/
        public static void loadAllFoundTextFiles()
        {
            String[] allFileDirectoriesAndName = Directory.GetFiles(ResourceUtil.getFontFileDir(""));
            foreach(String dir in allFileDirectoriesAndName)
            {
                if (dir.Contains(".fnt"))
                {
                    String fontName = dir.Replace(ResourceUtil.getFontFileDir(""), "").Replace(".fnt", "");//removes directory and file extension
                    tryAddNewFontToGlobalList(fontName);
                }
            }
        }

        private static void tryAddNewFontToGlobalList(String fontName)
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

        public static String getTextureDirForFont(FontFace font)
        {
            return ResourceUtil.getFontFileDir(font.getFontName() + ".png");
        }
    }
}
