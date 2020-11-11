using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;

namespace RabbetGameEngine.VisualEffects
{
    public class VFXMovingText3D : VFX
    {
        private FontFace font = null;
        private Model textModel = null;
        private string content = null;
        public PositionalObject parent;
        private Vector4 fontColor;
        private Vector3 offset;
        public string name;

        /// <summary>
        /// A vfx which is text in 3d space. Can move however does not have interpolation.
        /// </summary>
        public VFXMovingText3D(PositionalObject parent, string name, string font, string content, Vector3 offset, float textSize, CustomColor color) : base(parent.getPosition(), textSize, font, null, 0, RenderType.lerpText3D)
        {
            TextUtil.tryGetFont(font, out this.font);
            this.setVFXName(name);
            this.offset = offset;
            this.parent = parent;
            this.content = content;
            fontColor = color.toNormalVec4();
            textModel = TextModelBuilder3D.convertStringToModel(content, this.font, fontColor);
            textModel.worldPos = this.parent.getPosition() + offset;
            textModel.prevWorldPos = this.parent.getPrevTickPosition() + offset;
        }

        public override void onTick()
        {
            pos = parent.getPosition();
            textModel.worldPos = this.parent.getPosition() + offset;
            textModel.prevWorldPos = this.parent.getPrevTickPosition() + offset;
        }

        public override void sendRenderRequest()
        {
            Renderer.requestRender(batchType, vfxTexture, textModel.copyModel().scaleVertices(new Vector3(scale, scale, 1.0F)));
        }
    }
}
