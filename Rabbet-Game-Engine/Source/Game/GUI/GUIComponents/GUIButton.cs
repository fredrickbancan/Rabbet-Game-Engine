using OpenTK.Mathematics;

using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class GUIButton : GUIComponent
    {
        protected Color titleColor = Color.white;
        protected Color currentTitleColor;
        protected Color titleHoverColor = Color.darkYellow;
        protected Color color;
        protected Color hoverColor;
        public bool isHovered = false;
        protected bool isPressed = false;
        /// <summary>
        /// True if the cursor was already down when it entered this button.
        /// </summary>
        protected bool clickDragged = false;
        protected List<System.Action<GUIButton>> clickListeners = new List<System.Action<GUIButton>>();
        protected List<System.Action> hoverEnterListeners = new List<System.Action>();
        protected List<System.Action> hoverExitListeners = new List<System.Action>();
        protected string title;
        protected Model titleTextModel = null;
        protected FontFace titleFont;
        protected float fontSize = 0.2F;
        protected bool disabled = false;

        /// <summary>
        /// Min and Max bounds of this button in window coordinates (minX, maxX, minY, maxY)
        /// </summary>
        protected Vector4 pixelBounds;
        public GUIButton(float posX, float posY, float sizeX, float sizeY, Color color, string title, FontFace font, ComponentAnchor align = ComponentAnchor.CENTER_LEFT, int renderLayer = 0, string textureName = "white") : base(posX, posY, renderLayer)
        {
            setHoverColor(Color.black.setAlphaF(0.8F));
            this.title = title;
            this.color = color;
            componentTexture = TextureUtil.getTexture(textureName);
            anchor = align;
            setSize(sizeX, sizeY, true);
            setModel(QuadPrefab.copyModel());
            titleFont = font;
            currentTitleColor = titleColor;
            if (title != null && titleFont != null)
                titleTextModel = TextModelBuilder2D.convertStringToModel(title, titleFont, currentTitleColor.toNormalVec4(), new Vector3(screenPixelPos.X, screenPixelPos.Y, -0.2F), fontSize, ComponentAnchor.CENTER);

            addClickListener(GUIUtil.defaultOnButtonClick);
            addHoverEnterListener(GUIUtil.defaultOnButtonHoverEnter);
        }

        public override void updateRenderData()
        {
            base.updateRenderData();
            float halfWindowWidth = GameInstance.gameWindowWidth * 0.5F;
            float halfWindowHeight = GameInstance.gameWindowHeight * 0.5F;
            pixelBounds = new Vector4(screenPixelPos.X + halfWindowWidth - screenPixelSize.X * 0.5F, screenPixelPos.X + halfWindowWidth + screenPixelSize.X * 0.5F, halfWindowHeight - screenPixelPos.Y - screenPixelSize.Y * 0.5F, halfWindowHeight - screenPixelPos.Y + screenPixelSize.Y * 0.5F);
            
            if(title != null && titleFont != null)
            titleTextModel = TextModelBuilder2D.convertStringToModel(title, titleFont, currentTitleColor.toNormalVec4(), new Vector3(screenPixelPos.X, screenPixelPos.Y, -0.2F), fontSize, ComponentAnchor.CENTER);
        }

        public GUIButton setFontSize(float s)
        {
            this.fontSize = s;
            return this;
        }

        public override void onUpdate()
        {
            base.onUpdate();

            if (hidden) return;
            if (disabled) return;
            bool wasPreviouslyHovered = isHovered;
            Vector2 mPos = GameInstance.get.MousePosition;
            isHovered = mPos.X >= pixelBounds.X && mPos.X <= pixelBounds.Y && mPos.Y >= pixelBounds.Z && mPos.Y <= pixelBounds.W;

            if(wasPreviouslyHovered != isHovered)
            {
                if(isHovered)
                {
                    onHoverEnter();
                    if (Input.mouseleftButtonDown()) clickDragged = true;
                    else clickDragged = false;
                }
                else
                {
                    onHoverExit();
                    isPressed = false;
                    clickDragged = false;
                }
            }

            if(isHovered)
            {
                bool wasPreviouslyPressed = isPressed;
                isPressed = Input.mouseleftButtonDown();
                if(wasPreviouslyPressed != isPressed)
                {
                    if(!isPressed)
                    {
                        if(clickDragged)
                        {
                            clickDragged = false;
                        }
                        else
                        onClick();
                    }
                }
            }
            else
            {
                isPressed = false;
            }
        }

        private void onHoverEnter()
        {
            foreach(System.Action a in hoverEnterListeners)
            {
                a();
            }
        }

        private void onHoverExit()
        {
            foreach (System.Action a in hoverExitListeners)
            {
                a();
            }
        }

        private void onClick()
        {
            foreach (System.Action<GUIButton> a in clickListeners)
            {
                a(this);
            }
        }

        public GUIButton addClickListener(System.Action<GUIButton> a)
        {
            clickListeners.Add(a);
            return this;
        }

        public GUIButton addHoverEnterListener(System.Action a)
        {
            hoverEnterListeners.Add(a);
            return this;
        }

        public GUIButton addHoverExitListener(System.Action a)
        {
            hoverExitListeners.Add(a);
            return this;
        }

        public GUIButton clearClickListeners()
        {
            clickListeners.Clear();
            return this;
        }
        public GUIButton clearHoverEnterListeners()
        {
            hoverEnterListeners.Clear();
            return this;
        }
        public GUIButton clearHoverExitListeners()
        {
            hoverExitListeners.Clear();
            return this;
        }

        public GUIButton setHoverColor(Color color)
        {
            hoverColor = color;
            return this;
        }
        public GUIButton setColor(Color color)
        {
            this.color = color;
            return this;
        }

        public GUIButton disable()
        {
            disabled = true;
            currentTitleColor = Color.darkGrey;
                titleTextModel.setColor(currentTitleColor.toNormalVec4());
            return this;
        }
        public GUIButton enable()
        {
            disabled = false;
            currentTitleColor = titleColor;
            titleTextModel.setColor(currentTitleColor.toNormalVec4());
            return this;
        }

        public override void requestRender()
        {
            if(!hidden)
            {
                Renderer.requestRender(RenderType.guiTransparent, componentTexture, componentQuadModel.copyModel().setColor(isHovered || disabled ? hoverColor.toNormalVec4() : color.toNormalVec4()).transformVertices(translationAndScale), renderLayer-1);
               
                if (title != null && titleFont != null)
                    Renderer.requestRender(RenderType.guiText, titleFont.texture, titleTextModel, renderLayer);
            }
        }
    }
}
