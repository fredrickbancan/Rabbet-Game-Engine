using FredrickTechDemo.SubRendering.Text;
using System;
using System.Collections.Generic;

namespace FredrickTechDemo.SubRendering
{
    public static class TextUtil
    {
        public static readonly String textShaderDir = ResourceHelper.getShaderFileDir(@"GUI\GuiTextShader.shader");
        public static readonly float defaultFontSize = 0.02F;

        private static Dictionary<String, Font> fonts = new Dictionary<String, Font>();

        public static void tryAddNewFontToGlobalList(String fontName)
        {
            Font addingFont = new Font(fontName);
            if(addingFont.successfullyInitialized())
            {
                fonts.Add(fontName, addingFont);
            }
            else
            {
                Application.warn("TextUtil Could not process font: " + fontName);
            }
        }

        /*Returns true if the requested font was found in the global list*/
        public static bool tryGetFont(String name, out Font font)
        {
            bool success = fonts.TryGetValue(name, out font);
            if(!success)
            {
                Application.error("TextUtil could not find font named: " + name + " in global list, returning null.");
            }
            return success;
        }

        public static String getTextureDirForFont(Font font)
        {
            return ResourceHelper.getFontFileDir(font.getFontName() + ".png");
        }
    }
}
