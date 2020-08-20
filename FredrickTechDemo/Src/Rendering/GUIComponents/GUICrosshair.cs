using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering.GUI;
using System;

namespace FredrickTechDemo.GUI
{
    public enum CrosshairType//enum for all different types of crosshairs
    {
        normal
    };

    /*A simple crosshair component class. can specify an enum for what type of crosshair and the corrosponding 
      crosshair texture will be gotten.*/
    public class GUICrosshair : GUIScreenComponent
    {
        private static String shaderDir = ResourceHelper.getShaderFileDir(@"GUI\GuiStencilShader.shader");
        private Texture crosshairTexture;
        private float texutrePixelWidth = 0F;
        private float texutrePixelHeight = 0F;
        private ColourF crosshairColor = ColourF.black;

        public GUICrosshair(ColourF color,float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2F(0.5F, 0.5F))
        {
            crosshairColor = color;
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(shaderDir, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        public GUICrosshair(float crosshairSize = 2.0F, CrosshairType crosshairType = CrosshairType.normal) : base(new Vector2F(0.5F, 0.5F))
        {
            setCrosshairTextureAndSize(crosshairType, crosshairSize);
            setModel(new ModelDrawable(shaderDir, crosshairTexture, QuadPrefab.getNewModel().setColor(crosshairColor).vertices, QuadPrefab.quadIndices));
        }

        protected virtual void setCrosshairTextureAndSize(CrosshairType type, float crosshairSize)
        {
            switch(type)
            {
                case CrosshairType.normal:
                    crosshairTexture = new Texture(ResourceHelper.getTextureFileDir(@"GUI\Crosshairs\CrosshairNormal.png"), false);
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
