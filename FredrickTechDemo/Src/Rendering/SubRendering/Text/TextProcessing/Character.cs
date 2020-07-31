namespace FredrickTechDemo.SubRendering
{
    public class Character
	{
		private byte id;
		private float xPixelCoord;
		private float yPixelCoord;
		private float xMaxTextureCoord;
		private float yMaxTextureCoord;
		private float xOffsetPixels;
		private float yOffsetPixels;
		private float sizeXPixels;
		private float sizeYPixels;
		private float xAdvancePixels;
		public Character(byte id, float x, float y, float width, float height, float xOffset, float yOffset, float xadvance)
		{
			this.id = id;
			this.xPixelCoord = x;
			this.yPixelCoord = y;
			this.xOffsetPixels = xOffset;
			this.yOffsetPixels = yOffset;
			this.sizeXPixels = width;
			this.sizeYPixels = height;
			this.xMaxTextureCoord = width + x;
			this.yMaxTextureCoord = height + y;
			this.xAdvancePixels = xadvance;
		}

		public byte getId()
		{
			return id;
		}

		public float getUPixels()
		{
			return xPixelCoord;
		}

		public float getVPixels()
		{
			return yPixelCoord;
		}

		public float getUPixelMax()
		{
			return xMaxTextureCoord;
		}

		public float getVPixelMax()
		{
			return yMaxTextureCoord;
		}

		public float getxOffsetPixels()
		{
			return xOffsetPixels;
		}

		public float getyOffsetPixels()
		{
			return yOffsetPixels;
		}

		public float getXPixels()
		{
			return sizeXPixels;
		}

		public float getYPixels()
		{
			return sizeYPixels;
		}

		public float getXAdvancePixels()
		{
			return xAdvancePixels;
		}
	}
}
