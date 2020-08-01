using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    class TextPanel2D
    {
        private TextPanelTextLine2D[] textLinesInPanel;

        private Vector2F panelPos; // position of the top left corner of this panel
        private Vector2F panelPixelPos; // position of the top left corner of this panel
        private Vector3F panelColour;
        private String[] lines;
        private float fontSize;
        private FontBuilder font;
        private bool hasBuilt = false;

        public TextPanel2D(String[] lines, Vector2F pos, ColourF panelColor, float fontSize, FontBuilder font)
        {
            this.lines = lines;
            this.panelPos = pos;
            
            this.panelColour = panelColor.normalize();
            this.fontSize = fontSize;
            this.font = font;
        }

        /*builds or re-builds the text in this panel.*/
        public void build()
        {
            if (!hasBuilt)
            {
                textLinesInPanel = new TextPanelTextLine2D[lines.Length];
                this.panelPixelPos.x = panelPos.x * GameInstance.gameWindowWidth;
                this.panelPixelPos.y = panelPos.y * GameInstance.gameWindowHeight;
                for (int i = 0; i < lines.Length; i++)
                {
                    textLinesInPanel[i] = new TextPanelTextLine2D(lines[i], panelPixelPos, panelColour, fontSize, i, font);
                }
                hasBuilt = true;
            }
        }

        public TextPanelTextLine2D[] getTextPanelTextLines()
        {
            return textLinesInPanel;
        }
    }
}
