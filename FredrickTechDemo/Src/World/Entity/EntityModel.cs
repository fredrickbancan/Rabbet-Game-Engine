using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo.Entities
{
    public struct EntityModel
    {
        private Entity parent;
        private ModelDrawable entityModel;
        private Matrix4F prevTickModelMatrix;
        private Matrix4F modelMatrix;

        public EntityModel(Entity parent)
        {
            this.parent = parent;
            entityModel = null;
            modelMatrix = new Matrix4F(1.0F);
            prevTickModelMatrix = new Matrix4F(1.0F);
            updateModel();
        }

        /*Will be called on entity TICK update to update the model matrix. Only if the parent entity has a model to be rendered.*/
        public void updateModel()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = Matrix4F.translate(Vector3F.convert(parent.getPosition())) * Matrix4F.rotate(new Vector3F((float)parent.getPitch(), (float)parent.getYaw(), (float)parent.getRoll()));
        }

        public EntityModel initModel(ModelDrawable newModel)
        {
            entityModel = newModel;
            return this;
        }

        /*draws this model*/
        public void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            if (entityModel != null)
            {
                //interpolating model matrix between ticks for smooth transitions
                entityModel.draw(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            }
            else
            {
                Application.error("An attempt was made to render a null entity model!");
            }
        }
    }
}
