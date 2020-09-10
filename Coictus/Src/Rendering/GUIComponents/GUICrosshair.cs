using Coictus.Models;
using Coictus.SubRendering.GUI;
using OpenTK;
using System;
namespace Coictus.GUI
{
    public enum CrosshairType//enum for all different types of crosshairs
    {
        normal
    };

    /*A simple crosshair component class. can specify an enum for what type of crosshair and the corrosponding 
      crosshair texture will be gotten.*/
    public class GUICrosshair : GUIScreenComponent
    {
        private static String shaderDir = ResourceUtil.getShaderFileDir(@"GUI\GuiStencilShader.shader");
        private Texture crosshairTexture;
        private float texutrePixelWidth = 0F;
        private float texutrePixelHeight = 0F;
        private CustomColor crosshairColor = CustomColor.black;

        public GUICrosshair(CustomColor color,float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))
        {
            crosshairColor = color;
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(shaderDir, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        public GUICrosshair(float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2(0.5F, 0.5F))//TODO: figure out wtf happened here
        {
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(shaderDir, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        protected virtual void setCrosshairTextureAndSize(CrosshairType type, float crosshairSize)
        {
            switch(type)
            {
                case CrosshairType.normal:
                    crosshairTexture = new Texture(ResourceUtil.getTextureFileDir(@"GUI\Crosshairs\CrosshairNormal.png"), false);
                    texutrePixelWidth = crosshairTexture.getWidth() * crosshairSize;
                    texutrePixelHeight = crosshairTexture.getHeight() * crosshairSize;
                    break;

                default:
                    crosshairTexture = new Texture();
                    texutrePixelWidth = crosshairTexture.getWidth() * crosshairSize;
                    texutrePixelHeight = crosshairTexture.getHeight() * crosshairSize;
                    break;
            }

            setSizePixels(texutrePixelWidth, texutrePixelHeight);
        }
    }
}
