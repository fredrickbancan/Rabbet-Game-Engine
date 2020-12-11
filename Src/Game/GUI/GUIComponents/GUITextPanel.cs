using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUITextPanel
    {
        public Vector2 panelPos; 
        Vector2 panelPixelPos;
        public Vector4 panelColour = Color.white.toNormalVec4();
        public ComponentAlignment alignment = ComponentAlignment.LEFT;
        public int screenEdgePadding = TextUtil.defaultScreenEdgePadding;
        public float fontSize = 0.2F;
        public FontFace font = null;
        public List<string> lines;

        public Model[] models;

        public bool hidden = false;

        /// <summary>
        /// Text with smaller renderlayer will be rendered first.
        /// </summary>
        public int renderLayer;

        private Vector3 pixelTranslation;

        public GUITextPanel(Vector2 panelPos, ComponentAlignment alignment, int renderLayer = 0)//new gui text panel with default format
        {
            lines = new List<string>();
            this.alignment = alignment;
            panelPos.Y = 1 - panelPos.Y;
            this.panelPos = panelPos - new Vector2(0.5F, 0.5F);
            this.renderLayer = renderLayer;
        }

        public void build()
        {
            correctPosition();
            this.models = TextModelBuilder2D.convertstringArrayToModelArray(lines.ToArray(), font, panelColour, pixelTranslation, fontSize, alignment);
        }

        private void correctPosition()
        {
            float halfWindowWidth = GameInstance.gameWindowWidth * 0.5F;
            panelPixelPos = new Vector2(panelPos.X * GameInstance.gameWindowWidth, panelPos.Y * GameInstance.gameWindowHeight);
            switch (alignment)
            {
                case ComponentAlignment.CENTER:
                    panelPixelPos.X += halfWindowWidth;
                    break;
                case ComponentAlignment.LEFT:
                    panelPixelPos.X += screenEdgePadding;
                    break;
                case ComponentAlignment.RIGHT:
                    panelPixelPos.X = halfWindowWidth - (panelPixelPos.X + halfWindowWidth + screenEdgePadding);
                    break;
            }
            pixelTranslation = new Vector3(panelPixelPos.X, panelPixelPos.Y, -0.2F);
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
        public GUITextPanel setAlign(ComponentAlignment alignment)
        {
            this.alignment = alignment;
            return this;
        }

        public void requestRender()
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
