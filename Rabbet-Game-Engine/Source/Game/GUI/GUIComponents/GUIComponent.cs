using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class GUIComponent
    {
        protected ComponentAnchor anchor = ComponentAnchor.CENTER;
        protected Vector2 screenPos;
        protected Vector2 screenPixelPos;
        protected Vector2 size;
        protected Vector2 screenPixelSize;
        protected bool hidden = false;
        public bool paused = false;

        /// <summary>
        /// If this bool is true, size of this component will in pixels will be a percentage of the window HEIGHT,
        /// Otherwise, it will be a percentage of window HEIGHT AND WIDTH.
        /// </summary>
        protected bool dpiRelativeSize = true;

        protected string name = "";
        protected Texture componentTexture = null;
        protected Model componentQuadModel = null;
        protected Matrix4 translationAndScale = Matrix4.Identity;
        protected int renderLayer;
        public GUIComponent(float posX, float posY, int renderLayer = 0)
        {
            screenPos = new Vector2(posX, posY);
            this.renderLayer = renderLayer;
        }

        /// <summary>
        /// Sets the size of this component in precentage of window height
        /// </summary>
        public virtual void setSize(float width, float height, bool dpiRelative = true)
        {
            size = new Vector2(width, height);
            dpiRelativeSize = dpiRelative;
        }

        public virtual void setPos(Vector2 p)
        {
            screenPos = p;
        }

        public virtual void onFrame()
        {

        }

        public virtual void onUpdate()
        {

        }

        public virtual void onKeyDown(KeyboardKeyEventArgs e)
        {

        }

        public virtual void onMouseDown(MouseButtonEventArgs e)
        {

        }

        public virtual void setName(string name)
        {
            this.name = name;
        }

        public virtual void setModel(Model mod)
        {
            this.componentQuadModel = mod;
        }

        public virtual void onWindowResize()
        {
            updateRenderData();
        }

        public virtual void updateRenderData()
        {
            float posScale = MathUtil.normalize(0, GUIManager.guiMapRes.Y, GameInstance.gameWindowHeight);
            float halfWindowWidth = GameInstance.gameWindowWidth * 0.5F;
            float halfWindowHeight = GameInstance.gameWindowHeight * 0.5F;
            screenPixelSize = new Vector2(size.X * (dpiRelativeSize ? GameInstance.gameWindowHeight : GameInstance.gameWindowWidth), size.Y * GameInstance.gameWindowHeight);
           
            switch (anchor)
            {
                case ComponentAnchor.CENTER:
                    screenPixelPos.X = screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    screenPixelPos.Y = screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    break;
                case ComponentAnchor.CENTER_LEFT:
                    screenPixelPos.Y = screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    screenPixelPos.X = (screenPixelSize.X * 0.5F - halfWindowWidth) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    break;
                case ComponentAnchor.CENTER_RIGHT:
                    screenPixelPos.Y = screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    screenPixelPos.X = (halfWindowWidth - screenPixelSize.X * 0.5F) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    break;
                case ComponentAnchor.CENTER_BOTTOM:
                    screenPixelPos.X = screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    screenPixelPos.Y = (screenPixelSize.Y * 0.5F - halfWindowHeight) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    break;
                case ComponentAnchor.CENTER_TOP:
                    screenPixelPos.X = screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    screenPixelPos.Y = (halfWindowHeight - screenPixelSize.Y * 0.5F) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    break;
                case ComponentAnchor.TOP_LEFT:
                    screenPixelPos.Y = (halfWindowHeight - screenPixelSize.Y * 0.5F) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    screenPixelPos.X = (screenPixelSize.X * 0.5F - halfWindowWidth) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    break;
                case ComponentAnchor.TOP_RIGHT:
                    screenPixelPos.X = (halfWindowWidth - screenPixelSize.X * 0.5F) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    screenPixelPos.Y = (halfWindowHeight - screenPixelSize.Y * 0.5F) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    break;
                case ComponentAnchor.BOTTOM_LEFT:
                    screenPixelPos.Y = (screenPixelSize.Y * 0.5F - halfWindowHeight) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    screenPixelPos.X = (screenPixelSize.X * 0.5F - halfWindowWidth) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    break;
                case ComponentAnchor.BOTTOM_RIGHT:
                    screenPixelPos.X = (halfWindowWidth - screenPixelSize.X * 0.5F) + screenPos.X * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowWidth);
                    screenPixelPos.Y = (screenPixelSize.Y * 0.5F - halfWindowHeight) + screenPos.Y * (dpiRelativeSize ? GUIManager.guiMapRes.Y * posScale : GameInstance.gameWindowHeight);
                    break;
            }

            translationAndScale = Matrix4.CreateScale(screenPixelSize.X, screenPixelSize.Y, 1) *  Matrix4.CreateTranslation(screenPixelPos.X, screenPixelPos.Y, -0.2F);
        }

        public virtual void onMouseWheel(float scrolldelta)
        {
            
        }

        public virtual void setHide(bool flag)
        {
            hidden = flag;
        }

        public virtual Vector2 getScreenPos()
        {
            return screenPos;
        }

        public Vector2 getPixelSize()
        {
            return screenPixelSize;
        }

        public virtual void requestRender()
        {
           
        }

        public void pause()
        {
            paused = true;
        }

        public void unPause()
        {
            paused = false;
        }
    }
}
