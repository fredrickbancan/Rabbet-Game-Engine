using OpenTK.Mathematics;


namespace RabbetGameEngine
{
    public class VFXStaticText3D : VFX
    {
        private FontFace font = null;
        private Model textModel = null;
        private string content = null;
        private Vector4 fontColor;

        /// <summary>
        /// A vfx which is text in 3d space. Can move however does not have interpolation.
        /// </summary>
        public VFXStaticText3D(World w, string name, string font, string content, Vector3 pos, float textSize, Color color) : base(w, pos, 0, RenderType.text3D)
        {
            TextUtil.tryGetFont(font, out this.font);
            this.content = content;
            this.setVFXName(name);
            fontColor = color.toNormalVec4();
            textModel = TextModelBuilder3D.convertStringToModel(content, this.font, fontColor);
            textModel.worldPos = this.pos;
        }

        public override void onTick(float timeStep)
        {
            textModel.worldPos = this.pos;
        }

        public override void sendRenderRequest()
        {
            // Renderer.requestRender(renderType, vfxTexture, textModel.copyModel().scaleVertices(new Vector3(scale, scale, 1.0F)));
        }
    }
}
