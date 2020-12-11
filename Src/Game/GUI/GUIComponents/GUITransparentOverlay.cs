using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class GUITransparentOverlay : GUIComponent
    {
        public GUITransparentOverlay(Color color, float alpha, int renderLayer = 0) : base(new Vector2(0, 0.5F), renderLayer)
        {
            alignment = ComponentAlignment.CENTER;
            setModel(QuadPrefab.copyModel().setColor(color.toNormalVec4() * new Vector4(1,1,1,alpha)));
            setSize(1.0F, 1.0F, false);
            componentTexture = TextureUtil.getTexture("white");
            updateRenderData();
        }

        public override void requestRender()
        {
            base.requestRender();
            if (!hidden)
            {
                Renderer.requestRender(RenderType.guiTransparent, componentTexture, componentQuadModel.copyModel().transformVertices(translationAndScale), renderLayer);
            }
        }
    }
}
