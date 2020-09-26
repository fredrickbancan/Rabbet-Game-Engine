using RabbetGameEngine.Models;
using OpenTK;
using System;
namespace RabbetGameEngine
{
    public enum VFXBatchType//used to determine how to batch vfx objects
    {
        tirangles,
        points,
        lines
    }
}
namespace RabbetGameEngine.VFX
{
    /*This class is a spawnable sort of entity which can be rendered as a certain provided effect.
      Can be a particle, sprite, ect. This class will just hold the position, velocity and tick update code.
      VFX objects are treated as disposable and should not last more than a few seconds.*/
    public class VFXBase : PositionalObject, IDisposable
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
        protected ModelDrawable vfxModel;
        protected Matrix4 prevTickModelMatrix;
        protected Matrix4 modelMatrix;
        protected bool removalFlag = false;// true if this entity should be removed in the next tick
        protected bool shouldDeleteModel = false;// true if this vfx is using a model loaded from file. If so, it should NOT be deleted!

        protected VFXBatchType BatchType;
        public VFXBase(Vector3 pos, float initialScale, string shaderDir, string textureDir, string modelDir, float maxExistingSeconds = 1, VFXBatchType BatchType = VFXBatchType.tirangles) : base(pos)
        {
            this.scale = initialScale;
            this.vfxModel = ModelUtil.createModelDrawable(shaderDir, textureDir, modelDir);
            maxExistingTicks = TicksAndFps.getNumOfTicksForSeconds(maxExistingSeconds);
            updateVFXModel();
            updateVFXModel();

            this.BatchType = BatchType;
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
            prevTickModelMatrix = modelMatrix;
            scaleVelocity += scaleAcceleration - scaleResistance * scaleVelocity; //decrease expansion rate
            scale += scaleVelocity;
            modelMatrix = Matrix4.CreateScale(new Vector3(scale * scaleXModifyer, scale * scaleYModifyer, scale * scaleZModifyer)) * MathUtil.createRotation(new Vector3((float)pitch, -(float)yaw, (float)roll)) * Matrix4.CreateTranslation(pos);
        }

        /*draws this vfx, can be overridden*/
        public virtual void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            if (vfxModel != null && !removalFlag)
            {
                vfxModel.draw(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            }
            else
            {
                Application.warn("An attempt was made to render a null or disposed Base VFX.");
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
        public virtual void ceaseToExist()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!removalFlag)
            {
                removalFlag = true;
                if(shouldDeleteModel)
                {
                    vfxModel.delete();
                }
            }
        }
        protected virtual void setShouldDeleteModelOnDeath(bool flag)
        {
            shouldDeleteModel = flag;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public virtual bool exists()
        {
            return !removalFlag;
        }

        public VFXBatchType getBatchType()
        {
            return BatchType;
        }
    }
}
