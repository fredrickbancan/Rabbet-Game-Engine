using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo.SubRendering
{
    /*this class will be responsable for batch rendering text quads on the 2d screen space*/
    class TextRenderer2D
    {
        private TextMeshData textMesh;
        private Shader textShader;
        private ColourF textColour;

        public TextRenderer2D(ColourF textColour)
        {
            this.textColour = textColour;
        }
        
        public void setColour(ColourF newColour)
        {
            this.textColour = newColour;
        }

        public void init()
        {
            this.textShader = new Shader(@"..\..\res\Shaders\TextShader2D.shader");
        }
    }
}
