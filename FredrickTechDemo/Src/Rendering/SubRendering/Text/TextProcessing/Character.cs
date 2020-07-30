namespace FredrickTechDemo.SubRendering
{
    public class Character
	{
		private byte id;
		private float xTextureCoord;
		private float yTextureCoord;
		private float xMaxTextureCoord;
		private float yMaxTextureCoord;
		private float xOffsetPixels;
		private float yOffsetPixels;
		private float sizeXPixels;
		private float sizeYPixels;
		private float xAdvancePixels;
		public Character(byte id, float xTextureCoord, float yTextureCoord, float xTexSize, float yTexSize, float xOffset, float yOffset, float sizeXPixels, float sizeYPixels, float xAdvance)
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

		public byte getId()
		{
			return id;
		}

		public float getU()
		{
			return xTextureCoord;
		}

		public float getV()
		{
			return yTextureCoord;
		}

		public float getUMax()
		{
			return xMaxTextureCoord;
		}

		public float getVMax()
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
