using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    class TextPanel2D
    {
        private TextPanelTextLine2D[] textLinesInPanel;

        private Vector2F panelPos; // position of the top left corner of this panel
        private ColourF panelColour;
        private int linesAdded = 0;

        public TextPanel2D(String[] lines, Vector2F screenPos, ColourF panelColor, float fontSize, FontReader font)
        {
            this.panelPos = screenPos;
            this.panelColour = panelColor;

            textLinesInPanel = new TextPanelTextLine2D[lines.Length];

            for(int i = 0; i < lines.Length; i++)
            {
                textLinesInPanel[i] = new TextPanelTextLine2D(lines[i], screenPos, panelColor, fontSize, i, font);
                linesAdded++;
            }
        }

        public TextPanelTextLine2D[] getTextPanelTextLines()
        {
            return textLinesInPanel;
        }
    }
}
