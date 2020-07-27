using System.Collections.Generic;
/**
* During the loading of a text this represents one word in the text.
* @author Karl
*
*/
namespace FredrickTechDemo.SubRendering
{
    public class FontFileWord
	{

		private List<FontFileCharacter> characters = new List<FontFileCharacter>();
		private double width = 0;
		private double fontSize;

		/**
		 * Create a new empty word.
		 * @param fontSize - the font size of the text which this word is in.
		 */
		public FontFileWord(double fontSize)
		{
			this.fontSize = fontSize;
		}

		/**
		 * Adds a character to the end of the current word and increases the screen-space width of the word.
		 * @param character - the character to be added.
		 */
		public void addCharacter(FontFileCharacter character)
		{
			characters.Add(character);
			width += character.getxAdvance() * fontSize;
		}

		/**
		 * @return The list of characters in the word.
		 */
		public List<FontFileCharacter> getCharacters()
		{
			return characters;
		}

		/**
		 * @return The width of the word in terms of screen size.
		 */
		public double getWordWidth()
		{
			return width;
		}

	}
}
