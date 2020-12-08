using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public enum ButtonAlignment
    {
        LEFT,
        CENTER,
        RIGHT
    }

    public class GUIButton : GUIComponent
    {
        protected CustomColor color;
        protected CustomColor hoverColor;
        protected ButtonAlignment alignment;
        protected bool isHovered = false;
        protected bool isPressed = false;
        /// <summary>
        /// True if the cursor was already down when it entered this button.
        /// </summary>
        protected bool clickDragged = false;
        protected List<System.Action> clickListeners = new List<System.Action>();
        protected List<System.Action> hoverEnterListeners = new List<System.Action>();
        protected List<System.Action> hoverExitListeners = new List<System.Action>();

        /// <summary>
        /// Min and Max bounds of this button in screen coordinates (minX, maxX, minY, maxY)
        /// </summary>
        protected Vector4 pixelBounds;
        public GUIButton(Vector2 pos, Vector2 size, CustomColor color, string textureName = "white", ButtonAlignment align = ButtonAlignment.LEFT) : base(pos)
        {
            this.color = color;
            componentTexture = TextureUtil.getTexture(textureName);
            alignment = align;
            setSize(size.X, size.Y);
            setModel(QuadPrefab.copyModel());
            scaleAndTranslate();
        }

        protected override void scaleAndTranslate()
        {
            float halfWindowWidth = GameInstance.gameWindowWidth * 0.5F;
            float halfWindowHeight = GameInstance.gameWindowHeight * 0.5F;
            screenPixelPos = new Vector2(screenPos.X * GameInstance.gameWindowWidth, screenPos.Y * GameInstance.gameWindowHeight);
            screenPixelSize = new Vector2(size.X *  GameInstance.gameWindowHeight, size.Y * GameInstance.gameWindowHeight);
            switch(alignment)
            {
                case ButtonAlignment.CENTER:
                    screenPixelPos.X += halfWindowWidth;
                    break;
                case ButtonAlignment.LEFT:
                    screenPixelPos.X += screenPixelSize.X * 0.5F;
                    break;
                case ButtonAlignment.RIGHT:
                    screenPixelPos.X = halfWindowWidth - (screenPixelPos.X + halfWindowWidth + screenPixelSize.X * 0.5F);
                    break;
            }
            translationAndScale = Matrix4.CreateScale(screenPixelSize.X, screenPixelSize.Y, 1) * Matrix4.CreateTranslation(screenPixelPos.X, screenPixelPos.Y, -0.2F);
            pixelBounds = new Vector4(screenPixelPos.X + halfWindowWidth - screenPixelSize.X * 0.5F, screenPixelPos.X + halfWindowWidth + screenPixelSize.X * 0.5F, halfWindowHeight - screenPixelPos.Y - screenPixelSize.Y * 0.5F, halfWindowHeight - screenPixelPos.Y + screenPixelSize.Y * 0.5F);
        }


        public void addOnClickListener(System.Action a)
        {

        }

        public override void onUpdate()
        {
            base.onUpdate();

            if (hidden) return;
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
            foreach (System.Action a in clickListeners)
            {
                a();
            }
        }

        public void addClickListener(System.Action a)
        {
            clickListeners.Add(a);
        }

        public void addHoverEnterListener(System.Action a)
        {
            hoverEnterListeners.Add(a);
        }

        public void addHoverExitListener(System.Action a)
        {
            hoverExitListeners.Add(a);
        }

        public void setHoverColor(CustomColor color)
        {
            hoverColor = color;
        }

        public override void requestRender()
        {
            if(!hidden)
            Renderer.requestRender(RenderType.guiTransparent, componentTexture, componentQuadModel.copyModel().setColor(isHovered ? hoverColor.toNormalVec4() : color.toNormalVec4()).transformVertices(translationAndScale));
        }
    }
}
