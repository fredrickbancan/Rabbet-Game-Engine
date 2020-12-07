using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class GUITransparentOverlay : GUIComponent
    {
        public GUITransparentOverlay(CustomColor color, float alpha) : base(new Vector2(0.5F, 0.5F))
        {
            setModel(QuadPrefab.copyModel().setColor(color.toNormalVec4() * new Vector4(1,1,1,alpha)));
            setSizePixels(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);
            componentTexture = TextureUtil.getTexture("none");
        }

        protected override void scaleAndTranslate()
        {
            translationAndScale = Matrix4.CreateScale(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 1) * Matrix4.CreateTranslation(screenPosAbsolute.X - 0.01F, screenPosAbsolute.Y, -0.2F);
        }

        public override void requestRender()
        {
            base.requestRender();
            if (!hidden)
            {
                scaleAndTranslate();
                Renderer.requestRender(RenderType.guiTransparent, componentTexture, componentQuadModel.copyModel().transformVertices(translationAndScale));
            }
        }
    }
}
