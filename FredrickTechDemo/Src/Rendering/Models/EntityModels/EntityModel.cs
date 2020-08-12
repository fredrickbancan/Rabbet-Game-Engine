using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    /*This class represents the display model of any entity, The entity base class
      Will call the functions of this model to update and manipulate it. A new model
      Can be created which overrides these functions to do special things, i.e, a tank
      model for an EntityTank with multiple moving parts and multiple draw calls*/
    public class EntityModel
    {
        protected Entity parent;
        protected ModelDrawable theModel;
        protected Matrix4F prevTickModelMatrix;
        protected Matrix4F modelMatrix;

        protected EntityModel()
        {
            theModel = null;
            modelMatrix = new Matrix4F(1.0F);
            prevTickModelMatrix = new Matrix4F(1.0F);
        }
        public EntityModel(Entity parent)
        {
            this.parent = parent;
            theModel = null;
            modelMatrix = new Matrix4F(1.0F);
            prevTickModelMatrix = new Matrix4F(1.0F);
            updateModel();//updating model twice to set first frame render position to the entity position.
            updateModel();
        }
        public EntityModel(Entity parent,String shaderDir, String textureDir, String modelPath)
        {
            this.parent = parent;
            theModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelPath);
            modelMatrix = new Matrix4F(1.0F);
            prevTickModelMatrix = new Matrix4F(1.0F);
            updateModel();//updating model twice to set first frame render position to the entity position.
            updateModel();
        }

        /*Will be called on entity TICK update to update the model matrix. Only if the parent entity has a model to be rendered.*/
        /*By default this method will match the models rotation and position with the parent entities.*/
        public virtual void updateModel()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getPitch(), -(float)parent.getYaw() - 90, (float)parent.getRoll())) *  Matrix4F.translate(Vector3F.convert(parent.getPosition())) ;
        }

        /*Replaces the current or non existing ModelDrawable with the one provided*/
        public virtual EntityModel setModel(ModelDrawable newModel)
        {
            theModel = newModel;
            return this;
        }

        /*draws this model, can be overwritten.*/
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            if (theModel != null)
            {
                //interpolating model matrix between ticks for smooth transitions
                theModel.draw(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            }
            else
            {
                Application.warn("An attempt was made to render a null entity model!");
            }
        }

        public virtual void delete()
        {
            theModel.delete();
            theModel = null;
        }

        public virtual bool exists()
        {
            return theModel != null;
        }
    }
}
