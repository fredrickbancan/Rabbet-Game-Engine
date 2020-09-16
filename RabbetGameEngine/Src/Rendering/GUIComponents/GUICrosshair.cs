using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering.GUI;
using OpenTK;
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
        private Shader crosshairShader;
        private Texture crosshairTexture;
        private float texutrePixelWidth = 0F;
        private float texutrePixelHeight = 0F;
        private CustomColor crosshairColor = CustomColor.black;

        public GUICrosshair(CustomColor color,float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            ShaderUtil.tryGetShader("GUI\\GuiStencilShader.shader", out crosshairShader);
            crosshairColor = color;
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(crosshairShader, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        public GUICrosshair(float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            ShaderUtil.tryGetShader("GUI\\GuiStencilShader.shader", out crosshairShader);
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(crosshairShader, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        protected virtual void setCrosshairTextureAndSize(CrosshairType type, float crosshairSize)
        {
            switch(type)
            {
                case CrosshairType.normal:
                    TextureUtil.tryGetTexture("GUI\\Crosshairs\\CrosshairNormal.png", out crosshairTexture);
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
