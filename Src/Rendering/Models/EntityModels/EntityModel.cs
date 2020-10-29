using OpenTK.Mathematics;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine.Models
{
    /// <summary>
    /// This class represents the display model of any entity, The entity base class
    /// Will call the functions of this model to update and manipulate it. A new model
    /// Can be created which overrides these functions to do special things, i.e, a tank
    /// model for an EntityTank with multiple moving parts and multiple draw calls
    /// </summary>
    public class EntityModel
    {
        protected Entity parent;
        protected Model theModel;
        protected Texture tex;
        protected BatchType batchType;

        public EntityModel(Entity parent, string textureName, Model baseModel, BatchType type = BatchType.lerpTriangles)
        {
            this.batchType = type;
            this.parent = parent;
            this.theModel = baseModel;
            this.tex = TextureUtil.getTexture(textureName);
            if (baseModel != null)
            {
                this.theModel.modelMatrix = Matrix4.Identity;
                this.theModel.prevModelMatrix = Matrix4.Identity;
                onTick();//updating model twice to set first frame render position to the entity position.
                onTick();
            }
        }

        /// <summary>
        /// Will be called on entity TICK update to update the model matrix. Only if the parent entity has a model to be rendered.
        /// By default this method will match the models rotation and position with the parent entities.
        /// </summary>
        public virtual void onTick()
        {
            this.theModel.prevModelMatrix = this.theModel.modelMatrix;
            this.theModel.modelMatrix = MathUtil.createRotation(new Vector3((float)parent.getPitch(), -(float)parent.getYaw() - 90, (float)parent.getRoll())) *  Matrix4.CreateTranslation(parent.getPosition()) ;
        }
        

        /// <summary>
        /// Should be called in each tick to send the updated mesh data to the renderer and batcher
        /// </summary>
        public virtual void sendRenderRequest()
        {
            if(!parent.getIsMarkedForRemoval())
            Renderer.requestRender(batchType, tex, theModel);
        }
    }
}
