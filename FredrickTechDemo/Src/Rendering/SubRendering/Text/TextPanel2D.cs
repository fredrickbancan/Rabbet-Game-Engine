using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    class TextPanel2D
    {
        public Model[] models;

        private Vector2F panelPos; // position of the top left corner of this panel
        private Vector2F panelPixelPos; // position of the top left corner of this panel
        private Vector4F panelColour;
        private byte screenEdgePadding;
        private String[] lines;
        private float fontSize;
        private FontBuilder font;

        public TextPanel2D(String[] lines, Vector2F pos, ColourF panelColor, float fontSize, byte screenEdgePadding, FontBuilder font)
        {
            this.lines = lines;
            this.panelPos = pos;
            this.screenEdgePadding = screenEdgePadding;
            this.panelColour = panelColor.normalVector4F();
            this.fontSize = fontSize;
            this.font = font;
            this.panelPixelPos.x = panelPos.x * GameInstance.gameWindowWidth;
            this.panelPixelPos.y = panelPos.y * GameInstance.gameWindowHeight;
            this.build();
        }
        
        public void build()
        {
            this.panelPixelPos.x = panelPos.x * GameInstance.gameWindowWidth;
            this.panelPixelPos.y = panelPos.y * GameInstance.gameWindowHeight;
            this.models = TextModelBuilder.convertStringArrayToModelArray(lines, font, panelColour, panelPixelPos, fontSize, screenEdgePadding);
        }
    }
}
