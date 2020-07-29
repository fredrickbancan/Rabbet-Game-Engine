using System;
using System.Collections.Generic;
using System.IO;

namespace FredrickTechDemo.SubRendering
{
    class FontFile
    {
        private String debugDir;
        private static readonly byte padTop = 0;
        private static readonly byte padLeft = 1;
        private static readonly byte padBottom = 2;
        private static readonly byte padRight = 3;
        private static readonly byte desiredPadding = 3;
        private static readonly char splitter = ' ';
        private static readonly char numberSeperator = ',';
        private static readonly int spaceAscii = 32;

        private byte[] padding;
        private int paddingWidth;
        private int paddingHeight;
        private int lineHeightPixels;
        private int imageSquareSize;
        private int spaceWidth;

        private Dictionary<int, Character> finalFontData = new Dictionary<int, Character>();
        private Dictionary<String, String> lineData = new Dictionary<String, String>();
        private StreamReader reader;

        public FontFile(String fontName)
        {
            debugDir = ResourceHelper.getFontTextureFileDir(fontName + ".fnt");

            try
            {
                reader = new StreamReader(ResourceHelper.getFontTextureFileDir(fontName + ".fnt"));
            }
            catch(Exception e)
            {
                Application.error("FontFile could not open the provided directory file!\nException message: " + e.Message + "\nDirectory: " + debugDir);
            }

            loadPaddingData();
            loadLineSize();
            imageSquareSize = getValueFromLineData("scaleW");
            reader.Close();
        }

        private void loadPaddingData()
        {
            readNextLine();
            padding = getValuesFromLineData("padding");
            paddingWidth = padding[padLeft] + padding[padRight];
            paddingHeight = padding[padTop] + padding[padBottom];
        }
        
        private void loadLineSize()
        {
            readNextLine();
            lineHeightPixels = getValueFromLineData("lineHeight") - paddingHeight;
        }

        private void loadCharacterData()
        {
            readNextLine();
            readNextLine();

            while(readNextLine())
            {
                Character character = loadCharacter();
                if(character != null)
                {
                    finalFontData.Add(character.getId(), character);
                }
            }
        }

        private Character loadCharacter()
        {
            int id = getValueFromLineData("id");
            if(id == spaceAscii)
            {
                spaceWidth = getValueFromLineData("xadvance") - paddingWidth;
                return null;
            }
            float u = (getValueFromLineData("x") + (padding[padLeft] - desiredPadding)) / imageSquareSize;
            float v = (getValueFromLineData("y") + (padding[padTop] - desiredPadding)) / imageSquareSize;
            float widthPixels = getValueFromLineData("width") - (paddingWidth - (2 * desiredPadding));
            float heightPixels = getValueFromLineData("height") - (paddingHeight - (2 * desiredPadding));
            float uMax = widthPixels / imageSquareSize;
            float vMax = heightPixels / imageSquareSize;
            float xOffsetPixels = (getValueFromLineData("xoffset") + (padding[padLeft] - desiredPadding));
            float yOffsetPixels = (getValueFromLineData("yoffset") + (padding[padTop] - desiredPadding));
            float xAdvancePixels = (getValueFromLineData("xadvance") - paddingWidth);

            return new Character(id, u, v, uMax, vMax, xOffsetPixels, yOffsetPixels, widthPixels, heightPixels, xAdvancePixels);
        }
        /*resets line data and reads the next line of file, adds any relevant info to the lineData dictionary for processing.*/
        private bool readNextLine()
        {
            lineData.Clear();
            String line = null;

            try
            {
                line = reader.ReadLine();
            }
            catch(Exception e)
            {
                Application.error("FontFile error reading line in readNextLine()!\nException message: " + e.Message);
            }

            if(line == null)
            {
                return false;
            }

            String[] pairs;
            foreach(String pair in line.Split(splitter))
            {
                pairs = pair.Split('=');
                if(pairs.Length == 2)
                {
                    lineData.Add(pairs[0], pairs[1]);
                }
            }
            return true;
        }

        private int getValueFromLineData(String key)
        {
            String stringResult;
            if(!lineData.TryGetValue(key, out stringResult))
            {
                Application.warn("FontFile.getValueFromLineData() could not find value for key: " + key);
                return 0;
            }

            int result;
            if(!int.TryParse(stringResult, out result))
            {
                Application.warn("FontFile.getValueFromLineData() could not parse value to byte from string: " + stringResult);
                return 0;
            }
            return result;
        }

        private byte[] getValuesFromLineData(String key)
        {
            String[] stringBytes;

            String resultString;
            if(!lineData.TryGetValue(key, out resultString))
            {
                Application.warn("FontFile.getValuesFromLineData() could not find value for key: " + key);
                return null;
            }

            stringBytes = resultString.Split(numberSeperator);

            byte[] bytes = new byte[stringBytes.Length];
            for(int i = 0; i < bytes.Length; i++)
            {
                byte result;
                if(!Byte.TryParse(stringBytes[i], out result))
                {
                    Application.warn("FontFile.getValueFromLineData() could not parse value to byte from string: " + stringBytes[i]);
                    result = 0;
                }

                bytes[i] = result;
            }

            return bytes;
        }

        public float getSpaceWidthPixels()
        {
            return spaceWidth;
        }

        public Character getCharacter(int asciiId)
        {
            Character character;
            if(!finalFontData.TryGetValue(asciiId, out character))
            {
                Application.error("FontFile.getCharacter() could not find character in Dictionary!\nascii ID: " + asciiId);
            }
            return character;
        }
    }
}
