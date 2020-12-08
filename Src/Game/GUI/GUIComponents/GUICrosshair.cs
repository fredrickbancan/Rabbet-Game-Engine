using OpenTK.Mathematics;
using RabbetGameEngine.Models;
namespace RabbetGameEngine
{
    public enum CrosshairType
    {
        normal
    };
    public class GUICrosshair : GUIComponent
    {
        private string crosshairTextureName;
        private CustomColor crosshairColor = CustomColor.black;

        public GUICrosshair(CustomColor color, float crosshairSize = 0.05F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            crosshairColor = color;
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(QuadPrefab.copyModel().setColor(crosshairColor));
        }

        public GUICrosshair(float crosshairSize = 0.05F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(QuadPrefab.copyModel().setColor(crosshairColor));
        }

        protected virtual void setCrosshairTextureAndSize(CrosshairType type, float crosshairSize)
        {
            switch(type)
            {
               case CrosshairType.normal:
                    crosshairTextureName = "CrosshairNormal";
                    TextureUtil.tryGetTexture(crosshairTextureName, out this.componentTexture);
                    break;

                default:
                    crosshairTextureName = "debug";
                    break;
            }

            setSize(crosshairSize, crosshairSize);
            scaleAndTranslate();
        }

        public override void requestRender()
        {
            if (!hidden)
            {
                Renderer.requestRender(RenderType.guiCutout, this.componentTexture, this.componentQuadModel.copyModel().transformVertices(this.translationAndScale));
            }
        }

    }
}
