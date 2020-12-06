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
        private float texturePixelWidth = 0F;
        private float texturePixelHeight = 0F;
        private CustomColor crosshairColor = CustomColor.black;

        public GUICrosshair(CustomColor color, float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            crosshairColor = color;
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(QuadPrefab.copyModel().setColor(crosshairColor));
        }

        public GUICrosshair(float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
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
                    texturePixelWidth = componentTexture.getWidth() * crosshairSize;
                    texturePixelHeight = componentTexture.getHeight() * crosshairSize;
                    break;

                default:
                    texturePixelWidth = 100;
                    texturePixelHeight = 100;
                    crosshairTextureName = "debug";
                    break;
            }

            setSizePixels(texturePixelWidth, texturePixelHeight);
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
