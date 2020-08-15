using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;
namespace FredrickTechDemo
{
    public enum VFXRenderType//used to determine how to batch vfx objects
    {
        tirangles,
        points,
        lines
    }
}
namespace FredrickTechDemo.VFX
{
    /*This class is a spawnable sort of entity which can be rendered as a certain provided effect.
      Can be a particle, sprite, ect. This class will just hold the position, velocity and tick update code.
      VFX objects are treated as disposable and should not last more than a few ticks.*/
    public class VFXBase : PositionalObject, IDisposable
    {
        protected bool disposed = false;
        protected float scale = 1;//scale of the VFX
        protected double maxExistingTicks;
        protected int ticksExisted;
        protected ModelDrawable vfxModel;
        protected Matrix4F prevTickModelMatrix;
        protected Matrix4F modelMatrix;
        private bool removalFlag = false;// true if this entity should be removed in the next tick

        protected VFXRenderType renderType;

        public VFXBase(Vector3D pos, float initialScale, String shaderDir, String textureDir, String modelDir, double maxExistingSeconds = 1, VFXRenderType renderType = VFXRenderType.tirangles) : base(pos)
        {
            this.scale = initialScale;
            this.vfxModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelDir);
            maxExistingTicks = TicksAndFps.getNumOfTicksForSeconds(maxExistingSeconds);
            updateVFXModel();
            updateVFXModel();

            this.renderType = renderType;
        }
        public VFXBase(Vector3D pos, Vector3D velocity, float initialScale, String shaderDir, String textureDir, String modelDir, double maxExistingSeconds = 1, VFXRenderType renderType = VFXRenderType.tirangles) : base (pos)
        {
            this.scale = initialScale;
            setVelocity(velocity);
            this.vfxModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelDir);
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

            base.preTickMovement();
            //do movement
            base.postTickMovement();
        }

        protected virtual void updateVFXModel()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = Matrix4F.scale(new Vector3F(scale, scale, scale)) * Matrix4F.rotate(new Vector3F((float)pitch, (float)yaw, (float)roll)) * Matrix4F.translate(Vector3F.convert(pos));
        }

        /*draws this vfx, can be overridden*/
        public virtual void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            if (vfxModel != null && !removalFlag)
            {
                vfxModel.draw(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            }
            else
            {
                Application.warn("An attempt was made to render a null or disposed VFX.");
            }
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
