using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine.VFX
{
    /*This class is a spawnable sort of entity which can be rendered as a certain provided effect.
      Can be a particle, sprite, ect. This class will just hold the position, velocity and tick update code.
      VFX objects are treated as disposable and should not last more than a few seconds.*/
    public class VFXBase : PositionalObject
    {
        protected bool disposed = false;
        protected float scale = 1;//scale of the VFX
        protected float scaleVelocity; //how much to expand the VFX model every tick, should be converted from expansion every second
        protected float scaleAcceleration; //how much to expand the VFX model every tick, should be converted from expansion every second
        protected float scaleResistance = 0.03572F; //multiplyer decelerates the expansion of this VFX every tick
        protected float scaleXModifyer = 1;
        protected float scaleYModifyer = 1;
        protected float scaleZModifyer = 1;
        protected float maxExistingTicks;
        protected int ticksExisted;
        protected Model vfxModel;
        protected Texture vfxTexture;
        protected bool removalFlag = false;// true if this entity should be removed in the next tick
        protected bool shouldDeleteModel = false;// true if this vfx is using a model loaded from file. If so, it should NOT be deleted!
        protected BatchType batchType;
        public VFXBase(Vector3 pos, float initialScale,  string textureName, Model baseModel, float maxExistingSeconds = 1, BatchType type = BatchType.triangles) : base(pos)
        {
            this.batchType = type;
            this.scale = initialScale;
            TextureUtil.tryGetTexture(textureName, out vfxTexture);
            maxExistingTicks = TicksAndFrames.getNumOfTicksForSeconds(maxExistingSeconds);
            this.vfxModel = baseModel;
            if (vfxModel != null)
            {
                updateVFXModel();
                updateVFXModel();
            }
        }

        /*called every tick*/
        public virtual void onTick()
        {
            ticksExisted++;
            if (ticksExisted < 0) ticksExisted = 0;
            if (ticksExisted >= maxExistingTicks)
            {
                ceaseToExist();
            }
            updateVFXModel();
        }

        /*Called every tick can be overridden*/
        protected virtual void updateVFXModel()
        {
            if (vfxModel != null)
            {
                vfxModel.prevModelMatrix = vfxModel.modelMatrix;
                scaleVelocity += scaleAcceleration - scaleResistance * scaleVelocity; //decrease expansion rate
                scale += scaleVelocity;
                vfxModel.modelMatrix = Matrix4.CreateScale(new Vector3(scale * scaleXModifyer, scale * scaleYModifyer, scale * scaleZModifyer)) * MathUtil.createRotation(new Vector3(pitch, -yaw - 90, roll)) * Matrix4.CreateTranslation(pos);
            }
        }

        /*set deceleration muliplyer for expansion every tick*/
        public virtual void setExpansionResistance(float amount)
        {
            scaleResistance =  amount;
        }

        public virtual void setExpansionAccel(float expansionEverySecond)
        {
            this.scaleAcceleration = expansionEverySecond;
        }
        public virtual void setExpansionVelocity(float expansionvel)
        {
            this.scaleVelocity = expansionvel;
        }

        public virtual void setExpansionXModifyer(float modifyer)
        {
            scaleXModifyer = modifyer;
        }
        public virtual void setExpansionYModifyer(float modifyer)
        {
            scaleYModifyer = modifyer;
        }
        public virtual void setExpansionZModifyer(float modifyer)
        {
            scaleZModifyer = modifyer;
        }

        public virtual void sendRenderRequest()
        {
            if(vfxModel != null)
            Renderer.requestRender(this.batchType, vfxTexture, vfxModel);
        }

        public virtual void ceaseToExist()
        {
            removalFlag = true;
        }

        public virtual bool exists()
        {
            return !removalFlag;
        }
    }
}
