using OpenTK.Mathematics;
using RabbetGameEngine.Models;
namespace RabbetGameEngine.SubRendering.GUI
{
    public class GUIScreenComponent
    {
        private float widthPixels, heightPixels;
        protected Vector2 screenPosAbsolute;
        protected bool hidden = false;
        protected string name = "";
        protected Texture componentTexture = null;
        protected Model componentQuadModel = null;
        protected Matrix4 translationAndScale = Matrix4.Identity;

        public GUIScreenComponent(Vector2 screenPos/*position where 0 is top left and 1 is bottom right*/)
        {
            screenPos.Y = 1F - screenPos.Y;//flips y to make it start from top left.
            this.screenPosAbsolute = screenPos;
        }

        /*Sets the size of this component in pixels*/
        protected virtual void setSizePixels(float width, float height)
        {
            widthPixels = width;
            heightPixels = height;
        }

        public virtual void onTick()
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
            translationAndScale = Matrix4.CreateScale((GameInstance.dpiScale * widthPixels) * 0.07F, (GameInstance.dpiScale * heightPixels) * 0.07F, 1) *  Matrix4.CreateTranslation(screenPosAbsolute.X - 0.01F,  screenPosAbsolute.Y, -0.2F);
        }

        public virtual void setHide(bool flag)
        {
            hidden = flag;
        }

        public virtual Vector2 getScreenPos()
        {
            return screenPosAbsolute;
        }

        public virtual void requestRender()
        {
           
        }
    }
}
