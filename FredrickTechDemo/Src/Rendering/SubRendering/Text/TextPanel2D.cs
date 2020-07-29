using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.SubRendering.Text
{
    class TextPanel2D
    {
        private TextPanelTextLine2D[] textLinesInPanel;

        private Vector2F panelPos;
        private ColourF panelColour;
        private int linesAdded = 0;
        private float consolasLineHeight = 0.0023F;//TODO change to actual correct line height of text being used

        public TextPanel2D(String[] lines, Vector2F screenPos, ColourF panelColor, FontFile font)
        {
            this.panelPos = screenPos;
            this.panelColour = panelColor;

            textLinesInPanel = new TextPanelTextLine2D[lines.Length];

            for(int i = 0; i < lines.Length; i++)
            {
                textLinesInPanel[i] = makeLineWithCorrectPos(lines[i], font);
                linesAdded++;
            }
        }

        private TextPanelTextLine2D makeLineWithCorrectPos(String line, FontFile font)
        {
            Vector2F newPos;
            newPos.x = panelPos.x;
            newPos.y = panelPos.y + linesAdded * consolasLineHeight;
            return new TextPanelTextLine2D(line, newPos, panelColour, font);
        }

        public TextPanelTextLine2D[] getTextPanelTextLines()
        {
            return textLinesInPanel;
        }
    }
}
