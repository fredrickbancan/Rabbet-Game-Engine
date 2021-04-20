
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUITextPanel : GUIComponent
    {
        public Color defaultLineColor = Color.white;
        public int screenEdgePadding = TextUtil.defaultScreenEdgePadding;
        public float fontSize = 0.2F;
        public FontFace font = null;
        public List<string> lines;
        public List<Color> lineColors;
        public Model[] models = new Model[0];

        public GUITextPanel(float posX, float posY, FontFace font, ComponentAnchor anchor, int renderLayer = 0, bool dpiRelative = true) : base(posX, posY, renderLayer)//new gui text panel with default format
        {
            this.font = font;
            lines = new List<string>();
            lineColors = new List<Color>();
            this.anchor = anchor;
            this.renderLayer = renderLayer;
            setSize(0, 0, dpiRelative);
        }

        /// <summary>
        /// repositions this panel on the screen if needed and re-builds text meshes for rendering.
        /// </summary>
        public override void updateRenderData()
        {
            base.updateRenderData();

            build();
        }

        public void build()
        {
            this.models = TextModelBuilder2D.convertStringsToModelsWithColor(lines, lineColors, font, translationAndScale.ExtractTranslation(), fontSize, anchor);
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
            this.font = font;
            return this;
        }

        /// <summary>
        /// clears this panels lines of text and their associated colors
        /// </summary>
        public GUITextPanel clear()
        {
            lines.Clear();
            lineColors.Clear();
            return this;
        }

        public GUITextPanel addLine(string line)
        {
            lines.Add(line);
            lineColors.Add(defaultLineColor);
            return this;
        }
        public GUITextPanel addLine(string line, Color c)
        {
            lines.Add(line);
            lineColors.Add(c);
            return this;
        }
        public GUITextPanel setDefaultLineColor(Color color)
        {
            defaultLineColor = color;
            return this;
        }

        /// <summary>
        /// Sets the size of the font in this panel, each pixel in the font will be scaled up by size supplied
        /// </summary>
        public GUITextPanel setFontSize(float size)
        {
            fontSize = size;
            return this;
        }
        public GUITextPanel setAlign(ComponentAnchor alignment)
        {
            this.anchor = alignment;
            return this;
        }

        public override void requestRender()
        {
            if (!hidden)
            {
                for (int i = 0; i < models.Length; i++)
                {
                    Renderer.requestRender(RenderType.guiText, font.texture, models[i], renderLayer);
                }
            }
        }
    }
}
