using System;
using System.Collections.Generic;
using System.IO;

namespace FredrickTechDemo.SubRendering.Text
{
    /*This class is responsable for reading the .fnt files for each font, and generating a dictionary of characters for the text renderers to access.*/
    public class FontBuilder
    {
        private String debugDir;
        private static readonly char splitter = ' ';
        private static readonly char numberSeperator = ',';
        private static readonly byte spaceAscii = 32;
        private byte paddingTop;
        private byte paddingLeft;
        private byte paddingBottom;
        private byte paddingRight;
        private int scaleW;
        private int scaleH;
        private int lineHeightPixels;
        private int spaceWidth;

        private Dictionary<byte, Character> finalFontData = new Dictionary<byte, Character>();
        private Dictionary<String, String> lineData = new Dictionary<String, String>();
        private StreamReader reader;

        public FontBuilder(String fontName)
        {
            debugDir = ResourceHelper.getFontFileDir(fontName + ".fnt");

            try
            {
                reader = new StreamReader(ResourceHelper.getFontFileDir(fontName + ".fnt"));
            }
            catch(Exception e)
            {
                Application.error("FontBuilder could not open the provided directory file!\nException message: " + e.Message + "\nDirectory: " + debugDir);
            }

            loadPaddingData();
            loadLineAndImageSize();
            loadCharacterData();
            reader.Close();
        }

        private void loadPaddingData()
        {
            readNextLine();
            byte[] paddingData = getValuesFromLineData("padding");
            paddingTop = paddingData[0];
            paddingLeft = paddingData[1];
            paddingBottom = paddingData[2];
            paddingRight = paddingData[3];
        }
        
        private void loadLineAndImageSize()
        {
            readNextLine();
            lineHeightPixels = getValueFromLineData("lineHeight");
            scaleW = getValueFromLineData("scaleW");
            scaleH = getValueFromLineData("scaleH");
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
            byte id = (byte)getValueFromLineData("id");
            if(id == spaceAscii)
            {
                spaceWidth = getValueFromLineData("xadvance");
                return null;
            }
            float u = ((float)getValueFromLineData("x")) / scaleW;
            float v = ((float)getValueFromLineData("y")) / scaleH;
            float uMax = u + ((float)getValueFromLineData("width") - paddingRight) / scaleW;
            float vMax = v + ((float)getValueFromLineData("height") - paddingBottom) / scaleH;
            float pixelsWidth = getValueFromLineData("width");
            float pixelsHeight = getValueFromLineData("height");
            float xOffsetPixels = getValueFromLineData("xoffset");
            float yOffsetPixels = getValueFromLineData("yoffset");
            float xAdvancePixels = getValueFromLineData("xadvance") + paddingRight;

            //flipping v
            v = 1 - v;
            vMax = 1 - vMax;
            return new Character(id, u, v, uMax, vMax, pixelsWidth, pixelsHeight, xOffsetPixels, yOffsetPixels, xAdvancePixels);
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

            if(line == null || line.Contains("kernings"))
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

        public Character getCharacter(byte asciiId)
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
