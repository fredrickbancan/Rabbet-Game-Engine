namespace RabbetGameEngine
{
    public class Character
	{
		private byte id;
		private float u;
		private float v;
		private float uMax;
		private float vMax;
		private float pixelWidth;
		private float pixelHeight;
		private float xOffsetPixels;
		private float yOffsetPixels;
		private float xAdvancePixels;
		public Character(byte id, float u, float v, float uMax, float vMax, float pixelWidth, float pixelHeight, float xOffset, float yOffset, float xadvance)
		{
			this.id = id;
			this.u = u;
			this.v = v;
			this.xOffsetPixels = xOffset;
			this.yOffsetPixels = yOffset;
			this.uMax = uMax;
			this.vMax = vMax;
			this.pixelWidth = pixelWidth;
			this.pixelHeight = pixelHeight;
			this.xAdvancePixels = xadvance;
		}

		public byte getId()
		{
			return id;
		}

		public float getU()
		{
			return u;
		}

		public float getV()
		{
			return v;
		}

		public float getUMax()
		{
			return uMax;
		}

		public float getVMax()
		{
			return vMax;
		}
		
		public float getPixelWidth()
        {
			return pixelWidth;
        }

		public float getPixelHeight()
        {
			return pixelHeight;
        }
		public float getxOffsetPixels()
		{
			return xOffsetPixels;
		}

		public float getyOffsetPixels()
		{
			return yOffsetPixels;
		}

		public float getXAdvancePixels()
		{
			return xAdvancePixels;
		}
	}
}
