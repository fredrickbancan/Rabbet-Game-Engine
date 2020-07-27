using System;
using System.Collections.Generic;
using System.IO;
/**
* Provides functionality for getting the values from a font file.
* 
* @author Karl
*
*/
namespace FredrickTechDemo.Text
{
    public class FontFile
	{
		private const double LINE_HEIGHT = 0.03f;
		private const int SPACE_ASCII = 32;
		private const int PAD_TOP = 0;
		private const int PAD_LEFT = 1;
		private const int PAD_BOTTOM = 2;
		private const int PAD_RIGHT = 3;

		private const int DESIRED_PADDING = 3;

		private const char SPLITTER = ' ';
		private const char NUMBER_SEPARATOR = ',';

		private double aspectRatio;

		private double verticalPerPixelSize;
		private double horizontalPerPixelSize;
		private double spaceWidth;
		private int[] padding;
		private int paddingWidth;
		private int paddingHeight;

		private SortedDictionary<int, FontFileCharacter> metaData = new SortedDictionary<int, FontFileCharacter>();

		private StreamReader reader;
		private SortedDictionary<String, String> values = new SortedDictionary<String, String>();

		/**
		 * Opens a font file in preparation for reading.
		 * 
		 * @param file
		 *            - the font file.
		 */
		public FontFile(String file, double aspectRatio)
		{
			this.aspectRatio = aspectRatio;
			openFile(file);
			loadPaddingData();
			loadLineSizes();
			int imageWidth = getValueOfVariable("scaleW");
			loadCharacterData(imageWidth);
			close();
		}

		public double getSpaceWidth()
		{
			return spaceWidth;
		}

		public FontFileCharacter getCharacter(int ascii)
		{
			FontFileCharacter result;
			metaData.TryGetValue(ascii, out result);
			return result;
		}

		/**
		 * Read in the next line and store the variable values.
		 * 
		 * @return {@code true} if the end of the file hasn't been reached.
		 */
		private bool processNextLine()
		{
			values.Clear();
			String line = null;
			try
			{
				line = reader.ReadLine();
			}
			catch (IOException e1)
			{
				Application.error(e1.Message);
			}
			if (line == null || line.Contains("kernings"))
			{
				return false;
			}
			foreach (String part in line.Split(SPLITTER))
			{
				String[] valuePairs = part.Split('=');
				if (valuePairs.Length == 2)
				{
					values.Add(valuePairs[0], valuePairs[1]);
				}
			}
			return true;
		}

		/**
		 * Gets the {@code int} value of the variable with a certain name on the
		 * current line.
		 * 
		 * @param variable
		 *            - the name of the variable.
		 * @return The value of the variable.
		 */
		private int getValueOfVariable(String variable)
		{
			String result;
			values.TryGetValue(variable, out result);
			return int.Parse(result);
		}

		/**
		 * Gets the array of ints associated with a variable on the current line.
		 * 
		 * @param variable
		 *            - the name of the variable.
		 * @return The int array of values associated with the variable.
		 */
		private int[] getValuesOfVariable(String variable)
		{
			String value;
			values.TryGetValue(variable, out value);
			String[] numbers = value.Split(NUMBER_SEPARATOR);
			int[] actualValues = new int[numbers.Length];
			for (int i = 0; i < actualValues.Length; i++)
			{
				actualValues[i] = int.Parse(numbers[i]);
			}
			return actualValues;
		}

		/**
		 * Closes the font file after finishing reading.
		 */
		private void close()
		{
			try
			{
				reader.Close();
			}
			catch (IOException e)
			{
				Application.error(e.Message);
			}
		}

		/**
		 * Opens the font file, ready for reading.
		 * 
		 * @param file
		 *            - the font file.
		 */
		private void openFile(String file)
		{
			try
			{
				reader = new StreamReader(file);
			}
			catch (Exception e)
			{
				Application.error(e.Message);
				Application.error("Couldn't read font meta file!");
			}
		}

		/**
		 * Loads the data about how much padding is used around each character in
		 * the texture atlas.
		 */
		private void loadPaddingData()
		{
			processNextLine();
			this.padding = getValuesOfVariable("padding");
			this.paddingWidth = padding[PAD_LEFT] + padding[PAD_RIGHT];
			this.paddingHeight = padding[PAD_TOP] + padding[PAD_BOTTOM];
		}

		/**
		 * Loads information about the line height for this font in pixels, and uses
		 * this as a way to find the conversion rate between pixels in the texture
		 * atlas and screen-space.
		 */
		private void loadLineSizes()
		{
			processNextLine();
			int lineHeightPixels = getValueOfVariable("lineHeight") - paddingHeight;
			verticalPerPixelSize = LINE_HEIGHT / (double)lineHeightPixels;
			horizontalPerPixelSize = verticalPerPixelSize / aspectRatio;
		}

		/**
		 * Loads in data about each character and stores the data in the
		 * {@link Character} class.
		 * 
		 * @param imageWidth
		 *            - the width of the texture atlas in pixels.
		 */
		private void loadCharacterData(int imageWidth)
		{
			processNextLine();
			processNextLine();
			while (processNextLine())
			{
				FontFileCharacter c = loadCharacter(imageWidth);
				if (c != null)
				{
					metaData.Add(c.getId(), c);
				}
			}
		}

		/**
		 * Loads all the data about one character in the texture atlas and converts
		 * it all from 'pixels' to 'screen-space' before storing. The effects of
		 * padding are also removed from the data.
		 * 
		 * @param imageSize
		 *            - the size of the texture atlas in pixels.
		 * @return The data about the character.
		 */
		private FontFileCharacter loadCharacter(int imageSize)
		{
			int id = getValueOfVariable("id");
			if (id == SPACE_ASCII)
			{
				this.spaceWidth = (getValueOfVariable("xadvance") - paddingWidth) * horizontalPerPixelSize;
				return null;
			}
			double xTex = ((double)getValueOfVariable("x") + (padding[PAD_LEFT] - DESIRED_PADDING)) / imageSize;
			double yTex = ((double)getValueOfVariable("y") + (padding[PAD_TOP] - DESIRED_PADDING)) / imageSize;
			int width = getValueOfVariable("width") - (paddingWidth - (2 * DESIRED_PADDING));
			int height = getValueOfVariable("height") - ((paddingHeight) - (2 * DESIRED_PADDING));
			double quadWidth = width * horizontalPerPixelSize;
			double quadHeight = height * verticalPerPixelSize;
			double xTexSize = (double)width / imageSize;
			double yTexSize = (double)height / imageSize;
			double xOff = (getValueOfVariable("xoffset") + padding[PAD_LEFT] - DESIRED_PADDING) * horizontalPerPixelSize;
			double yOff = (getValueOfVariable("yoffset") + (padding[PAD_TOP] - DESIRED_PADDING)) * verticalPerPixelSize;
			double xAdvance = (getValueOfVariable("xadvance") - paddingWidth) * horizontalPerPixelSize;
			return new FontFileCharacter(id, xTex, yTex, xTexSize, yTexSize, xOff, yOff, quadWidth, quadHeight, xAdvance);
		}
	}
}