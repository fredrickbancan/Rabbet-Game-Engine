namespace FredrickTechDemo.SubRendering
{
    /**
 * Stores the vertex data for all the quads on which a text will be rendered.
 * @author Karl
 *
 */
    struct TextMeshData
	{

		private float[] vertexPositions;
		private float[] textureCoords;

		public TextMeshData(float[] vertexPositions, float[] textureCoords)
		{
			this.vertexPositions = vertexPositions;
			this.textureCoords = textureCoords;
		}

		public float[] getVertexPositions()
		{
			return vertexPositions;
		}

		public float[] getTextureCoords()
		{
			return textureCoords;
		}

		public int getVertexCount()
		{
			return vertexPositions.Length / 2;
		}

	}
}
