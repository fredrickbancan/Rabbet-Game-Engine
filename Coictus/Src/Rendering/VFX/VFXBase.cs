using Coictus.FredsMath;
using Coictus.Models;
using System;
namespace Coictus
{
    public enum VFXRenderType//used to determine how to batch vfx objects
    {
        tirangles,
        points,
        lines
    }
}
namespace Coictus.VFX
{
    /*This class is a spawnable sort of entity which can be rendered as a certain provided effect.
      Can be a particle, sprite, ect. This class will just hold the position, velocity and tick update code.
      VFX objects are treated as disposable and should not last more than a few ticks.*/
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
        protected double maxExistingTicks;
        protected int ticksExisted;
        protected ModelDrawable vfxModel;
        protected Matrix4F prevTickModelMatrix;
        protected Matrix4F modelMatrix;
        protected bool removalFlag = false;// true if this entity should be removed in the next tick

        protected VFXRenderType renderType;
        public VFXBase(Vector3D pos, float initialScale, String shaderDir, String textureDir, String modelDir, float maxExistingSeconds = 1, VFXRenderType renderType = VFXRenderType.tirangles) : base(pos)
        {
            this.scale = initialScale;
            this.vfxModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelDir, renderType == VFXRenderType.points ? false : true);//TODO:Inneficcient. This will mean we have to load the data every time a VFX is spawned. maybe a list of pre loaded models is needed?
            maxExistingTicks = TicksAndFps.getNumOfTicksForSeconds(maxExistingSeconds);
            updateVFXModel();
            updateVFXModel();

            this.renderType = renderType;
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
            modelMatrix = Matrix4F.scale(new Vector3F(scale * scaleXModifyer, scale * scaleYModifyer, scale * scaleZModifyer)) * Matrix4F.rotate(new Vector3F((float)pitch, -(float)yaw, (float)roll)) * Matrix4F.translate(Vector3F.convert(pos));
        }

        /*draws this vfx, can be overridden*/
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor, int pass = 1)
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
                if (vfxModel != null)
                {
                    vfxModel.delete();
                }

                removalFlag = true;
            }
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

        public VFXRenderType getRenderType()
        {
            return renderType;
        }
    }
}
