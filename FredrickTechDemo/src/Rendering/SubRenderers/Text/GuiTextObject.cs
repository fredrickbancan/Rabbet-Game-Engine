using FredrickTechDemo.FredsMath;
using System;
/**
* Represents a piece of text in the game.
* 
* @author Karl
*
*/
namespace FredrickTechDemo.SubRendering
{
    public class GuiTextObject
	{

		private String textString;
		private float fontSize;

		private int textMeshVao;
		private int vertexCount;
		private ColourF colour;

		private Vector2F position;
		private float lineMaxSize;
		private int numberOfLines;

		private bool centerText = false;

		//TODO: create font type class and add to parameters and members
		public GuiTextObject(String text, float fontSize, Vector2F position, float maxLineLength, bool centered)
		{
			this.textString = text;
			this.fontSize = fontSize;
			this.position = position;
			this.lineMaxSize = maxLineLength;
			this.centerText = centered;
		}

		/**
		 * Remove the text from the screen.
		 */
		public void remove()
		{
		}


		public void setColour(ColourF color)
		{
			this.colour = color;
		}

		/**
		 * @return the colour of the text.
		 */
		public ColourF getColour()
		{
			return colour;
		}

		/**
		 * @return The number of lines of text. This is determined when the text is
		 *         loaded, based on the length of the text and the max line length
		 *         that is set.
		 */
		public int getNumberOfLines()
		{
			return numberOfLines;
		}

		/**
		 * @return The position of the top-left corner of the text in screen-space.
		 *         (0, 0) is the top left corner of the screen, (1, 1) is the bottom
		 *         right.
		 */
		public Vector2F getPosition()
		{
			return position;
		}

		/**
		 * @return the ID of the text's VAO, which contains all the vertex data for
		 *         the quads on which the text will be rendered.
		 */
		public int getMesh()
		{
			return textMeshVao;
		}

		/**
		 * Set the VAO and vertex count for this text.
		 * 
		 * @param vao
		 *            - the VAO containing all the vertex data for the quads on
		 *            which the text will be rendered.
		 * @param verticesCount
		 *            - the total number of vertices in all of the quads.
		 */
		public void setMeshInfo(int vao, int verticesCount)
		{
			this.textMeshVao = vao;
			this.vertexCount = verticesCount;
		}

		/**
		 * @return The total number of vertices of all the text's quads.
		 */
		public int getVertexCount()
		{
			return this.vertexCount;
		}

		/**
		 * @return the font size of the text (a font size of 1 is normal).
		 */
		protected float getFontSize()
		{
			return fontSize;
		}

		/**
		 * Sets the number of lines that this text covers (method used only in
		 * loading).
		 * 
		 * @param number
		 */
		protected void setNumberOfLines(int number)
		{
			this.numberOfLines = number;
		}

		/**
		 * @return {@code true} if the text should be centered.
		 */
		protected bool isCentered()
		{
			return centerText;
		}

		/**
		 * @return The maximum length of a line of this text.
		 */
		protected float getMaxLineSize()
		{
			return lineMaxSize;
		}

		/**
		 * @return The string of text.
		 */
		protected String getTextString()
		{
			return textString;
		}

	}
}
