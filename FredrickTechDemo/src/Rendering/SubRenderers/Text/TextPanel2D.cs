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

        public TextPanel2D(String[] lines, Vector2F screenPos, ColourF panelColor)
        {
            this.panelPos = screenPos;
            this.panelColour = panelColor;

            textLinesInPanel = new TextPanelTextLine2D[lines.Length];

            for(int i = 0; i < lines.Length; i++)
            {
                textLinesInPanel[i] = makeLineWithCorrectPos(lines[i]);
                linesAdded++;
            }
        }

        private TextPanelTextLine2D makeLineWithCorrectPos(String line)
        {
            Vector2F newPos;
            newPos.x = panelPos.x;
            newPos.y = panelPos.y + linesAdded * consolasLineHeight;
            return new TextPanelTextLine2D(line, newPos, panelColour);
        }

        public TextPanelTextLine2D[] getTextPanelTextLines()
        {
            return textLinesInPanel;
        }
    }
}
