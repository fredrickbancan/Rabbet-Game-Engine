namespace FredrickTechDemo.SubRendering
{
    public class Character
	{
		private int id;
		private float xTextureCoord;
		private float yTextureCoord;
		private float xMaxTextureCoord;
		private float yMaxTextureCoord;
		private float xOffsetPixels;
		private float yOffsetPixels;
		private float sizeXPixels;
		private float sizeYPixels;
		private float xAdvancePixels;
		public Character(int id, float xTextureCoord, float yTextureCoord, float xTexSize, float yTexSize, float xOffset, float yOffset, float sizeXPixels, float sizeYPixels, float xAdvance)
		{
			this.id = id;
			this.xTextureCoord = xTextureCoord;
			this.yTextureCoord = yTextureCoord;
			this.xOffsetPixels = xOffset;
			this.yOffsetPixels = yOffset;
			this.sizeXPixels = sizeXPixels;
			this.sizeYPixels = sizeYPixels;
			this.xMaxTextureCoord = xTexSize + xTextureCoord;
			this.yMaxTextureCoord = yTexSize + yTextureCoord;
			this.xAdvancePixels = xAdvance;
		}

		public int getId()
		{
			return id;
		}

		public float getxTextureCoord()
		{
			return xTextureCoord;
		}

		public float getyTextureCoord()
		{
			return yTextureCoord;
		}

		public float getXMaxTextureCoord()
		{
			return xMaxTextureCoord;
		}

		public float getYMaxTextureCoord()
		{
			return yMaxTextureCoord;
		}

		public float getxOffset()
		{
			return xOffsetPixels;
		}

		public float getyOffset()
		{
			return yOffsetPixels;
		}

		public float getSizeX()
		{
			return sizeXPixels;
		}

		public float getSizeY()
		{
			return sizeYPixels;
		}

		public float getxAdvance()
		{
			return xAdvancePixels;
		}
	}
}
