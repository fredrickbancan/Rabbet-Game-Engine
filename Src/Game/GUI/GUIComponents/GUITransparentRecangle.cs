using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class GUITransparentRecangle : GUIComponent
    {
        public GUITransparentRecangle(Vector2 pos, Vector2 size, Color color, ComponentAlignment alignment, int renderLayer = 0, string texture = "white") : base(pos)
        {
            setModel(QuadPrefab.copyModel().setColor(color.toNormalVec4()));
            setSize(size.X, size.Y, true);
            componentTexture = TextureUtil.getTexture(texture);
            scaleAndTranslate();
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
