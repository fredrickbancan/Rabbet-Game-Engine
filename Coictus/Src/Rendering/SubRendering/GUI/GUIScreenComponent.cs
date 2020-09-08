
using Coictus.Models;
using OpenTK;
namespace Coictus.SubRendering.GUI
{
    public class GUIScreenComponent
    {
        private float widthPixels, heightPixels;
        protected Vector2 screenPosAbsolute;
        protected bool hidden = false;
        private bool hasModel = false;
        private Matrix4 translationAndScale = new Matrix4();
        private Matrix4 orthographicMatrix = new Matrix4();
        private ModelDrawable componentQuadModel = null;

        public GUIScreenComponent(Vector2 screenPos/*position where 0 is top left and 1 is bottom right*/)
        {
            this.screenPosAbsolute = new Vector2(screenPos.X, 1F-screenPos.Y);//flips y.
        }

        /*Sets the size of this component in pixels*/
        protected virtual void setSizePixels(float width, float height)
        {
            widthPixels = width;
            heightPixels = height;
        }

        /*Set the renderable model for this component. Should be a 1x1 quad. The modeldrawable will contain the shader and texture.*/
        public virtual void setModel(ModelDrawable model)
        {
            this.componentQuadModel = model;
            hasModel = componentQuadModel != null;
        }

        public virtual void onTick()
        {

        }

        public virtual void onWindowResize()
        {
            scaleAndTranslate();
        }

        protected virtual void scaleAndTranslate()
        {
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0, 1);

            translationAndScale = Matrix4.CreateScale(new Vector3(widthPixels, heightPixels, 1)) *  Matrix4.CreateTranslation(new Vector3(GameInstance.gameWindowWidth * screenPosAbsolute.X, GameInstance.gameWindowHeight * screenPosAbsolute.Y, -0.01F));
        }
        public virtual void draw()
        {
            if (!hidden)
            {
                if (hasModel && componentQuadModel != null)
                {
                    componentQuadModel.draw(orthographicMatrix, translationAndScale);
                }
                else
                {
                    Application.warn("An attempt was made to render a null gui screen component model");
                }
            }
        }

        public virtual void setHide(bool flag)
        {
            hidden = flag;
        }

        public virtual void deleteComponent()
        {
            componentQuadModel.delete();
            hasModel = false;
            componentQuadModel = null;
        }

        public virtual Vector2 getScreenPos()
        {
            return screenPosAbsolute;
        }
    }
}
