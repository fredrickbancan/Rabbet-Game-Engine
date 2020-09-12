using OpenTK;
using System;

namespace Coictus.Models
{
    /*This class represents the display model of any entity, The entity base class
      Will call the functions of this model to update and manipulate it. A new model
      Can be created which overrides these functions to do special things, i.e, a tank
      model for an EntityTank with multiple moving parts and multiple draw calls*/
    public class EntityModel
    {
        protected Entity parent;
        protected ModelDrawable theModel;
        protected Matrix4 prevTickModelMatrix;
        protected Matrix4 modelMatrix;

        protected EntityModel()
        {
            theModel = null;
            modelMatrix = Matrix4.Identity;
            prevTickModelMatrix = Matrix4.Identity;
        }
        public EntityModel(Entity parent)
        {
            this.parent = parent;
            theModel = null;
            modelMatrix = Matrix4.Identity;
            prevTickModelMatrix = Matrix4.Identity;
            onTick();//updating model twice to set first frame render position to the entity position.
            onTick();
        }
        public EntityModel(Entity parent,String shaderDir, String textureDir, String modelPath)
        {
            this.parent = parent;
            theModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelPath);//TODO: Inefficient. This will mean we have to load model data each time a model for an entity etc is spawned!, maybe make a list of pre loaded models?
            modelMatrix = Matrix4.Identity;
            prevTickModelMatrix = Matrix4.Identity;
            onTick();//updating model twice to set first frame render position to the entity position.
            onTick();
        }

        /*Will be called on entity TICK update to update the model matrix. Only if the parent entity has a model to be rendered.*/
        /*By default this method will match the models rotation and position with the parent entities.*/
        public virtual void onTick()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = MathUtil.createRotation(new Vector3((float)parent.getPitch(), -(float)parent.getYaw() - 90, (float)parent.getRoll())) *  Matrix4.CreateTranslation(parent.getPosition()) ;
        }

        /*Replaces the current or non existing ModelDrawable with the one provided*/
        public virtual EntityModel setModel(ModelDrawable newModel)
        {
            theModel = newModel;
            return this;
        }

        /*called every frame before rendering*/
        public virtual void onFrame()
        {

        }

        /*draws this model, can be overwritten.*/
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
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
            if (theModel != null)
            {
                theModel.delete();
                theModel = null;
            }
        }

        public virtual bool exists()
        {
            return theModel != null;
        }
    }
}
