using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    public class GUITextPanel
    {
        public Model[] models;

        private Vector2F panelPos; // position of the top left corner of this panel
        private Vector2F panelPixelPos; // position of the top left corner of this panel
        private Vector4F panelColour;
        private TextAlign alignment;
        private int screenEdgePadding;
        private String[] lines;
        private float fontSize;
        private Font font;

        public GUITextPanel(Vector2F pos, Font font, TextAlign alignment)
        {
            this.font = font;
            this.panelPos = pos;
            this.panelPixelPos.x = panelPos.x * GameInstance.gameWindowWidth;
            this.panelPixelPos.y = panelPos.y * GameInstance.gameWindowHeight;
            this.alignment = alignment;
        }
        
        public void buildOrRebuild()
        {
            this.panelPixelPos.x = panelPos.x * GameInstance.gameWindowWidth;
            this.panelPixelPos.y = panelPos.y * GameInstance.gameWindowHeight;
            this.models = TextModelBuilder2D.convertStringArrayToModelArray(lines, font, panelColour, panelPixelPos, fontSize * GameInstance.dpiScale, screenEdgePadding, alignment);
        }
    }
}
