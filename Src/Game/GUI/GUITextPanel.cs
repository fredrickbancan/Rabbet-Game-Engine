using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class TextFormat// a simle class for submitting format options for a text panel
    {
        public Vector2 panelPos = Vector2.Zero; // position of the top left corner of this panel
        public Vector2 panelPixelPos = Vector2.Zero; // position of the top left corner of this panel
        public Vector4 panelColour = CustomColor.white.toNormalVec4();
        public TextAlign alignment = TextAlign.LEFT;
        public int screenEdgePadding = TextUtil.defaultScreenEdgePadding;
        public string[] lines = new string[] { "Sample Text" };
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
        public TextFormat setLines(string[] lines)
        {
            this.lines = lines;
            return this;
        }
        public TextFormat setAlign(TextAlign alignment)
        {
            this.alignment = alignment;
            return this;
        }
        public TextFormat setLine(string line)
        {
            lines = new string[] { line };
            return this;
        }
        public TextFormat setPanelColor(CustomColor color)
        {
            panelColour = color.toNormalVec4();
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
        private List<string> lines;

        public Model[] models;

        public TextFormat format;

        public bool hidden = false;

        public GUITextPanel()//new gui text panel with default format
        {
            format = new TextFormat();
            lines = new List<string>();
        }

        public GUITextPanel(TextFormat format)//new gui text panel with provided format
        {
            this.format = format;
            lines = new List<string>();
        }

        public void build()
        {
            format.panelPixelPos.X = format.panelPos.X * GameInstance.gameWindowWidth;
            format.panelPixelPos.Y = format.panelPos.Y * GameInstance.gameWindowHeight;
            this.models = TextModelBuilder2D.convertstringArrayToModelArray(format.lines, format.font, format.panelColour, format.panelPixelPos, format.fontSize * GameInstance.dpiScale, format.screenEdgePadding, format.alignment);
        }

        public GUITextPanel hide()
        {
            hidden = true;
            return this;
        }
        public GUITextPanel unHide()
        {
            hidden = false;
            return this;
        }
        public GUITextPanel setFont(FontFace font)
        {
            format.font = font;
            return this;
        }

        public GUITextPanel clear()
        {
            lines.Clear();
            return this;
        }

        public GUITextPanel addLine(string line)
        {
            lines.Add(line);
            return this;
        }

        public GUITextPanel pushLines()
        {
            format.lines = lines.ToArray();
            return this;
        }
        public GUITextPanel setLines(string[] lines)
        {
            format.lines = lines;
            return this;
        }

        public GUITextPanel setLine(string line)
        {
            format.lines = new string[] { line };
            return this;
        }

        public GUITextPanel setPanelColor(CustomColor color)
        {
            format.panelColour = color.toNormalVec4();
            return this;
        }

        public GUITextPanel setFontSize(float size)
        {
            format.fontSize = size;
            return this;
        }
    }
}
