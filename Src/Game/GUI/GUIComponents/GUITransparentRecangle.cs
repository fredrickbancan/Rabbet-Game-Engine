using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class GUITransparentRecangle : GUIComponent
    {
        public GUITransparentRecangle(Vector2 pos, Vector2 size, Color color, ComponentAlignment alignment,int renderLayer = 0, bool dpiRelative = true, string texture = "white") : base(pos)
        {
            setModel(QuadPrefab.copyModel().setColor(color.toNormalVec4()));
            setSize(size.X, size.Y, dpiRelative);
            componentTexture = TextureUtil.getTexture(texture);
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
        public GUITransparentRecangle setPos(Vector2 p)
        {
            screenPixelPos= p;
            return this;
        }
    }
}
