using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo.Models
{
    public struct EntityModel
    {
        private Entity parent;
        private ModelDrawable theModel;
        private Matrix4F prevTickModelMatrix;
        private Matrix4F modelMatrix;

        public EntityModel(Entity parent)
        {
            this.parent = parent;
            theModel = null;
            modelMatrix = new Matrix4F(1.0F);
            prevTickModelMatrix = new Matrix4F(1.0F);
            updateModel();//updating model twice to set first frame render position to the entity position.
            updateModel();
        }

        /*Will be called on entity TICK update to update the model matrix. Only if the parent entity has a model to be rendered.*/
        public void updateModel()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getPitch(), (float)-parent.getYaw(), (float)parent.getRoll())) *  Matrix4F.translate(Vector3F.convert(parent.getPosition())) ;
        }

        public EntityModel initModel(ModelDrawable newModel)
        {
            theModel = newModel;
            return this;
        }

        /*draws this model*/
        public void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            if (theModel != null)
            {
                //interpolating model matrix between ticks for smooth transitions
                theModel.draw(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            }
            else
            {
                Application.error("An attempt was made to render a null entity model!");
            }
        }

        public bool exists()
        {
            return theModel != null;
        }
    }
}
