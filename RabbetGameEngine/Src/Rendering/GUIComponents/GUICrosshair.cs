using OpenTK;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering.GUI;
namespace RabbetGameEngine.GUI
{
    public enum CrosshairType//enum for all different types of crosshairs
    {
        normal
    };

    /*A simple crosshair component class. can specify an enum for what type of crosshair and the corrosponding 
      crosshair texture will be gotten.*/
    public class GUICrosshair : GUIScreenComponent
    {
        private Texture crosshairTexture;
        private float texutrePixelWidth = 0F;
        private float texutrePixelHeight = 0F;
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
                    TextureUtil.tryGetTexture("CrosshairNormal", out crosshairTexture);
                    texutrePixelWidth = crosshairTexture.getWidth() * crosshairSize;
                    texutrePixelHeight = crosshairTexture.getHeight() * crosshairSize;
                    break;

                default:
                    TextureUtil.tryGetTexture("debug", out crosshairTexture);
                    texutrePixelWidth = crosshairTexture.getWidth() * crosshairSize;
                    texutrePixelHeight = crosshairTexture.getHeight() * crosshairSize;
                    break;
            }

            setSizePixels(texutrePixelWidth, texutrePixelHeight);
        }
    }
}
