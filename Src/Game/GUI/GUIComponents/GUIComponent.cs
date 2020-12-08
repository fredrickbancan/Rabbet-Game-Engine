using OpenTK.Mathematics;
using RabbetGameEngine.Models;
namespace RabbetGameEngine
{
    public class GUIComponent
    {
        protected Vector2 screenPos;
        protected Vector2 screenPixelPos;
        protected Vector2 size;
        protected Vector2 screenPixelSize;
        protected bool hidden = false;

        /// <summary>
        /// If this bool is true, size of this component will in pixels will be a percentage of the window HEIGHT,
        /// Otherwise, it will be a percentage of window HEIGHT AND WIDTH.
        /// </summary>
        protected bool dpiRelativeSize = true;

        protected string name = "";
        protected Texture componentTexture = null;
        protected Model componentQuadModel = null;
        protected Matrix4 translationAndScale = Matrix4.Identity;

        public GUIComponent(Vector2 screenPos)
        {
            screenPos.Y = 1 - screenPos.Y;
            this.screenPos = screenPos - new Vector2(0.5F, 0.5F);
        }

        /// <summary>
        /// Sets the size of this component in precentage of window height
        /// </summary>
        protected virtual void setSize(float width, float height, bool dpiRelative = true)
        {
            size = new Vector2(width, height);
            dpiRelativeSize = dpiRelative;
        }

        public virtual void onUpdate()
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
            scaleAndTranslate();
        }

        protected virtual void scaleAndTranslate()
        {
            screenPixelPos = new Vector2(screenPos.X * GameInstance.gameWindowWidth, screenPos.Y * GameInstance.gameWindowHeight);
            screenPixelSize = new Vector2(size.X * (dpiRelativeSize ? GameInstance.gameWindowHeight : GameInstance.gameWindowWidth), size.Y * GameInstance.gameWindowHeight);
            translationAndScale = Matrix4.CreateScale(screenPixelSize.X, screenPixelSize.Y, 1) *  Matrix4.CreateTranslation(screenPixelPos.X, screenPixelPos.Y, -0.2F);
        }

        public virtual void setHide(bool flag)
        {
            hidden = flag;
        }

        public virtual Vector2 getScreenPos()
        {
            return screenPos;
        }

        public virtual void requestRender()
        {
           
        }
    }
}
