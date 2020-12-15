using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUITextPanel : GUIComponent
    {
        public Vector4 panelColour = Color.white.toNormalVec4();
        public int screenEdgePadding = TextUtil.defaultScreenEdgePadding;
        public float fontSize = 0.2F;
        public FontFace font = null;
        public List<string> lines;
        public Model[] models = new Model[0];

        public GUITextPanel(float posX, float posY, FontFace font, ComponentAnchor anchor, int renderLayer = 0, bool dpiRelative = true) : base(posX, posY, renderLayer)//new gui text panel with default format
        {
            this.font = font;
            lines = new List<string>();
            this.anchor = anchor;
            this.renderLayer = renderLayer;
            setSize(0, 0, dpiRelative); 
        }

        public override void updateRenderData()
        {
            base.updateRenderData();

            build();
        }

        public void build()
        {
            this.models = TextModelBuilder2D.convertStringArrayToModelArray(lines.ToArray(), font, panelColour, translationAndScale.ExtractTranslation(), fontSize, anchor);
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

        public GUITextPanel setPanelColor(Color color)
        {
            panelColour = color.toNormalVec4();
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
            if(!hidden)
            {
                for(int i = 0; i < models.Length; i++)
                {
                    Renderer.requestRender(RenderType.guiText, font.texture, models[i], renderLayer);
                }
            }
        }
    }
}
