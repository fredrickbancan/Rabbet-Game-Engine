/**
 * Simple data structure class holding information about a certain glyph in the
 * font texture atlas. All sizes are for a font-size of 1.
 * 
 * @author Karl
 *
 */
namespace FredrickTechDemo.Text
{
    public class FontFileCharacter
	{

		private int id;
		private double xTextureCoord;
		private double yTextureCoord;
		private double xMaxTextureCoord;
		private double yMaxTextureCoord;
		private double xOffset;
		private double yOffset;
		private double sizeX;
		private double sizeY;
		private double xAdvance;

		/**
		 * @param id
		 *            - the ASCII value of the character.
		 * @param xTextureCoord
		 *            - the x texture coordinate for the top left corner of the
		 *            character in the texture atlas.
		 * @param yTextureCoord
		 *            - the y texture coordinate for the top left corner of the
		 *            character in the texture atlas.
		 * @param xTexSize
		 *            - the width of the character in the texture atlas.
		 * @param yTexSize
		 *            - the height of the character in the texture atlas.
		 * @param xOffset
		 *            - the x distance from the curser to the left edge of the
		 *            character's quad.
		 * @param yOffset
		 *            - the y distance from the curser to the top edge of the
		 *            character's quad.
		 * @param sizeX
		 *            - the width of the character's quad in screen space.
		 * @param sizeY
		 *            - the height of the character's quad in screen space.
		 * @param xAdvance
		 *            - how far in pixels the cursor should advance after adding
		 *            this character.
		 */
		public FontFileCharacter(int id, double xTextureCoord, double yTextureCoord, double xTexSize, double yTexSize,
				double xOffset, double yOffset, double sizeX, double sizeY, double xAdvance)
		{
			this.id = id;
			this.xTextureCoord = xTextureCoord;
			this.yTextureCoord = yTextureCoord;
			this.xOffset = xOffset;
			this.yOffset = yOffset;
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			this.xMaxTextureCoord = xTexSize + xTextureCoord;
			this.yMaxTextureCoord = yTexSize + yTextureCoord;
			this.xAdvance = xAdvance;
		}

		public int getId()
		{
			return id;
		}

		public double getxTextureCoord()
		{
			return xTextureCoord;
		}

		public double getyTextureCoord()
		{
			return yTextureCoord;
		}

		public double getXMaxTextureCoord()
		{
			return xMaxTextureCoord;
		}

		public double getYMaxTextureCoord()
		{
			return yMaxTextureCoord;
		}

		public double getxOffset()
		{
			return xOffset;
		}

		public double getyOffset()
		{
			return yOffset;
		}

		public double getSizeX()
		{
			return sizeX;
		}

		public double getSizeY()
		{
			return sizeY;
		}

		public double getxAdvance()
		{
			return xAdvance;
		}

	}
}