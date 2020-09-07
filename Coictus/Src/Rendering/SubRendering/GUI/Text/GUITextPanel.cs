using Coictus.Models;
using Coictus.SubRendering.GUI.Text;
using OpenTK;
using System;
using System.Drawing;

namespace Coictus.GUI.Text
{
    public class TextFormat// a simle class for submitting format options for a text panel
    {
        public Vector2 panelPos = Vector2.Zero; // position of the top left corner of this panel
        public Vector2 panelPixelPos = Vector2.Zero; // position of the top left corner of this panel
        public Vector4 panelColour = MathUtil.colorToNormalVec4(Color.White);
        public TextAlign alignment = TextAlign.LEFT;
        public int screenEdgePadding = TextUtil.defaultScreenEdgePadding;
        public String[] lines = new String[] { "Sample Text" };
        public float fontSize = TextUtil.defaultFontSize;
        public FontFace font = null;

        public TextFormat()
        {

        }

        public TextFormat(float xPosFromLeft, float yPosFromTop)
        {
            panelPos = new Vector2(xPosFromLeft, yPosFromTop);
        }

        #region builderMethods
        public TextFormat setLines(String[] lines)
        {
            this.lines = lines;
            return this;
        }
        public TextFormat setAlign(TextAlign alignment)
        {
            this.alignment = alignment;
            return this;
        }
        public TextFormat setLine(String line)
        {
            lines = new string[] { line };
            return this;
        }
        public TextFormat setPanelColor(Color color)
        {
            panelColour = MathUtil.colorToNormalVec4(color);
            return this;
        }
        public TextFormat setFontSize(float size)
        {
            fontSize = size;
            return this;
        }
        #endregion builderMethods

    }
    public class GUITextPanel
    {
        public Model[] models;

        public TextFormat format;

        public bool hidden = false;

        public GUITextPanel()//new gui text panel with default format
        {
            format = new TextFormat();
        }

        public GUITextPanel(TextFormat format)//new gui text panel with provided format
        {
            this.format = format;
        }

        public void buildOrRebuild()
        {
            format.panelPixelPos.X = format.panelPos.X * GameInstance.gameWindowWidth;
            format.panelPixelPos.Y = format.panelPos.Y * GameInstance.gameWindowHeight;
            this.models = TextModelBuilder2D.convertStringArrayToModelArray(format.lines, format.font, format.panelColour, format.panelPixelPos, format.fontSize * GameInstance.dpiScale, format.screenEdgePadding, format.alignment);
        }

        public void hide()
        {
            hidden = true;
        }
        public void unHide()
        {
            hidden = false;
        }
        public void setFont(Font font)
        {
            format.font = font;
        }

        public String[] getLines()
        {
            return format.lines;
        }

        public void updateLines(String[] newLines)
        {
            format.lines = newLines;
        }

        #region builderMethods
        public GUITextPanel setAlign(TextAlign alignment)
        {
            this.format.alignment = alignment;
            return this;
        }
        public GUITextPanel setLines(String[] lines)
        {
            format.lines = lines;
            return this;
        }
        public GUITextPanel setLine(String line)
        {
            format.lines = new string[] { line };
            return this;
        }
        public GUITextPanel setPanelColor(Color color)
        {
            format.panelColour = color.normalVector4();
            return this;
        }
        public GUITextPanel setFontSize(float size)
        {
            format.fontSize = size;
            return this;
        }
        #endregion builderMethods
    }
}
