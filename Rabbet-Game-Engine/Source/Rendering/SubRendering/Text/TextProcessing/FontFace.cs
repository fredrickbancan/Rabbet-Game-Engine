using System;
using System.Collections.Generic;
using System.IO;

namespace RabbetGameEngine
{
    /*This class is responsable for reading the .fnt files for each font, and generating a dictionary of characters for the text renderers to access.*/
    public class FontFace
    {
        private bool successfull = true;
        private string debugDir;
        private string fontName;
        private static readonly char splitter = ' ';
        private static readonly char numberSeperator = ',';
        private static readonly byte spaceAscii = 32;
        private byte paddingTop;
        private byte paddingLeft;
        private byte paddingBottom;
        private byte paddingRight;
        private int size;
        private int scaleW;
        private int scaleH;
        private float lineHeightPixels;
        private float spaceWidth;

        private Dictionary<byte, Character> finalFontData = new Dictionary<byte, Character>();
        private Dictionary<string, string> lineData = new Dictionary<string, string>();
        private StreamReader reader;
        private Texture fontTexture;
        private float dpiCorrection = 0.0F;
        public FontFace(string fontName)
        {
            debugDir = ResourceUtil.getFontFileDir(fontName + ".fnt");
            this.fontName = fontName;
            try
            {
                reader = new StreamReader(ResourceUtil.getFontFileDir(fontName + ".fnt"));
            }
            catch (Exception e)
            {
                Application.error("Font could not open the provided directory file!\nException message: " + e.Message + "\nDirectory: " + debugDir);
                successfull = false;
            }

            if (successfull)
            {
                loadPaddingAndSize();
                loadLineAndImageSize();
                loadCharacterData();
            }
            reader.Close();
            if (!TextureUtil.tryGetTexture(fontName, out fontTexture))
            {
                Application.error("Could not find texture for font named " + fontName);
            }
        }

        private void loadPaddingAndSize()
        {
            readNextLine();
            byte[] paddingData = getValuesFromLineData("padding");
            paddingTop = paddingData[0];
            paddingLeft = paddingData[1];
            paddingBottom = paddingData[2];
            paddingRight = paddingData[3];
            size = getValueFromLineData("size");
            dpiCorrection = 100.0F / (float)size;
        }

        private void loadLineAndImageSize()
        {
            readNextLine();
            lineHeightPixels = getValueFromLineData("lineHeight") * dpiCorrection;
            scaleW = getValueFromLineData("scaleW");
            scaleH = getValueFromLineData("scaleH");
        }

        private void loadCharacterData()
        {
            readNextLine();
            readNextLine();

            while (readNextLine())
            {
                Character character = loadCharacter();
                if (character != null)
                {
                    finalFontData.Add(character.getId(), character);
                }
            }
        }

        private Character loadCharacter()
        {
            byte id = (byte)getValueFromLineData("id");
            if (id == spaceAscii)
            {
                spaceWidth = getValueFromLineData("xadvance") + paddingLeft + paddingRight;
                spaceWidth *= dpiCorrection;
                return null;
            }
            float u = ((float)getValueFromLineData("x")) / scaleW;
            float v = ((float)getValueFromLineData("y")) / scaleH;
            float uMax = u + ((float)getValueFromLineData("width")) / scaleW;
            float vMax = v + ((float)getValueFromLineData("height")) / scaleH;
            float pixelsWidth = getValueFromLineData("width") + paddingRight + paddingLeft;
            float pixelsHeight = getValueFromLineData("height") + paddingTop + paddingBottom;
            float xOffsetPixels = getValueFromLineData("xoffset");
            float yOffsetPixels = getValueFromLineData("yoffset");
            float xAdvancePixels = getValueFromLineData("xadvance");

            //flipping v
            v = 1 - v;
            vMax = 1 - vMax;
            float uvEpsilon = 0.0005F;
            return new Character(id, u + uvEpsilon, v, uMax - uvEpsilon, vMax, pixelsWidth * dpiCorrection, pixelsHeight * dpiCorrection, xOffsetPixels * dpiCorrection, yOffsetPixels * dpiCorrection, xAdvancePixels * dpiCorrection);
        }

        /*resets line data and reads the next line of file, adds any relevant info to the lineData dictionary for processing.*/
        private bool readNextLine()
        {
            lineData.Clear();
            string line = null;

            try
            {
                line = reader.ReadLine();
            }
            catch (Exception e)
            {
                Application.error("FontFile error reading line in readNextLine()!\nException message: " + e.Message);
                successfull = false;
            }

            if (line == null || line.Contains("kerning"))
            {
                return false;
            }

            string[] pairs;
            foreach (string pair in line.Split(splitter))
            {
                pairs = pair.Split('=');
                if (pairs.Length == 2)
                {
                    lineData.Add(pairs[0], pairs[1]);
                }
            }
            return true;
        }

        private int getValueFromLineData(string key)
        {
            string stringResult;
            if (!lineData.TryGetValue(key, out stringResult))
            {
                Application.error("FontFile.getValueFromLineData() could not find value for key: " + key);
                successfull = false;
                return 0;
            }

            int result;
            if (!int.TryParse(stringResult, out result))
            {
                Application.error("FontFile.getValueFromLineData() could not parse value to byte from string: " + stringResult);
                successfull = false;
                return 0;
            }
            return result;
        }

        private byte[] getValuesFromLineData(string key)
        {
            string[] stringBytes;

            string resultstring;
            if (!lineData.TryGetValue(key, out resultstring))
            {
                Application.error("FontFile.getValuesFromLineData() could not find value for key: " + key);
                successfull = false;
                return null;
            }

            stringBytes = resultstring.Split(numberSeperator);

            byte[] bytes = new byte[stringBytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte result;
                if (!Byte.TryParse(stringBytes[i], out result))
                {
                    Application.error("FontFile.getValueFromLineData() could not parse value to byte from string: " + stringBytes[i]);
                    successfull = false;
                    result = 0;
                }

                bytes[i] = result;
            }

            return bytes;
        }

        public int getImagePixelWidth()
        {
            return scaleW;
        }

        public int getImagePixelHeight()
        {
            return scaleH;
        }
        public float getLineHeightPixels()
        {
            return lineHeightPixels;
        }

        public float getSpaceWidthPixels()
        {
            return spaceWidth;
        }

        public float getTextSizePixels()
        {
            return size;
        }
        public Character getCharacter(byte asciiId)
        {
            Character character;
            if (!finalFontData.TryGetValue(asciiId, out character))
            {
                Application.error("FontFile.getCharacter() could not find character in Dictionary!\nascii ID: " + asciiId);
            }
            return character;
        }

        public bool successfullyInitialized()
        {
            return successfull;
        }

        public string getFontName()
        {
            return fontName;
        }

        public Texture texture { get => fontTexture; }

    }
}
