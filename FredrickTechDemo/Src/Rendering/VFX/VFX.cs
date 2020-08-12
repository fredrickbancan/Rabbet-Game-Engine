using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo
{
    /*This class is a spawnable sort of entity which can be rendered as a certain provided effect.
      Can be a particle, sprite, ect. This class will just hold the position, velocity and tick update code.
      VFX objects are treated as disposable and should not last more than a few ticks.*/
    public class VFX : IDisposable
    {
        protected bool disposed = false;
        protected float scale = 1;//scale of the VFX
        protected double maxExistingTicks;
        protected int ticksExisted;
        protected Vector3D pos, prevTickPos, velocity;
        protected ModelDrawable vfxModel;
        protected Matrix4F prevTickModelMatrix;
        protected Matrix4F modelMatrix;
        protected Vector3F rotation = Vector3F.zero;
        private bool removalFlag = false;// true if this entity should be removed in the next tick

        public VFX(Vector3D pos, float initialScale, String shaderDir, String textureDir, String modelDir, double maxExistingSeconds = 1)
        {
            this.pos = pos;
            this.scale = initialScale;
            this.vfxModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelDir);
            maxExistingTicks = TicksAndFps.getNumOfTicksForSeconds(maxExistingSeconds);
            updateVFXModel();
            updateVFXModel();
        }
        public VFX(Vector3D pos, float initialScale, Vector3D velocity, String shaderDir, String textureDir, String modelDir, double maxExistingSeconds = 1)
        {
            this.pos = pos;
            this.scale = initialScale;
            this.velocity = velocity;
            this.vfxModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, modelDir);
            maxExistingTicks = TicksAndFps.getNumOfTicksForSeconds(maxExistingSeconds);
            updateVFXModel();
            updateVFXModel();
        }

        /*called every tick*/
        public virtual void onTick()
        {
            prevTickPos = pos;
            if (ticksExisted < 0) ticksExisted = 0;
            if (ticksExisted >= maxExistingTicks)
            {
                ceaseToExist();
            }
            updateVFXModel();

            ticksExisted++;
            pos += velocity;
        }

        protected virtual void updateVFXModel()
        {
            prevTickModelMatrix = modelMatrix;
            modelMatrix = Matrix4F.scale(new Vector3F(scale, scale, scale)) * Matrix4F.rotate(rotation) * Matrix4F.translate(Vector3F.convert(pos));
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

        public virtual Vector3D getLerpPos()
        {
            return prevTickPos - (pos - prevTickPos) * TicksAndFps.getPercentageToNextTick();
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
    }
}
