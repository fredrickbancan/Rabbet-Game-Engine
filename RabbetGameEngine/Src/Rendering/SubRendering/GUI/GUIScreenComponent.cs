using OpenTK;
using RabbetGameEngine.Models;

namespace RabbetGameEngine.SubRendering.GUI
{
    public class GUIScreenComponent
    {
        private float widthPixels, heightPixels;
        protected Vector2 screenPosAbsolute;
        protected bool hidden = false;
        private Model componentQuadModel = null;
        private Matrix4 translationAndScale = Matrix4.Identity;
        private Matrix4 orthographicMatrix = Matrix4.Identity;
        public GUIScreenComponent(Vector2 screenPos/*position where 0 is top left and 1 is bottom right*/)
        {
            screenPos.X -= 0.5F;
            screenPos.Y += 0.5F;//compensating for the strange OpenTK ortho projection 
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
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0, 1);
            translationAndScale = Matrix4.CreateScale(widthPixels, heightPixels, 1) *  Matrix4.CreateTranslation(GameInstance.gameWindowWidth * screenPosAbsolute.X, GameInstance.gameWindowHeight * screenPosAbsolute.Y, -0.01F);
        }

        public virtual void setHide(bool flag)
        {
            hidden = flag;
        }

        public virtual Vector2 getScreenPos()
        {
            return screenPosAbsolute;
        }
    }
}
