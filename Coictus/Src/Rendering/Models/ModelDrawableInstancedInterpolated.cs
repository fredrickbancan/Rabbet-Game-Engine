namespace Coictus.Models
{
    /*This class is for drawing multiple instances of the same model, the same way as ModelDrawableInstanced, but with interpolation.
      Uses two matrices for each instance, and a interpolation factor passed to shader each draw call.*/
    public class ModelDrawableInstancedInterpolated : ModelDrawableInstanced
    {
        public ModelDrawableInstancedInterpolated(ModelDrawable instance, uint[] indices, ModelDrawType drawType) : base(instance, indices, drawType)
        {
        }

        public ModelDrawableInstancedInterpolated(ModelDrawable instance,  ModelDrawType drawType) : base(instance, drawType)
        {
        }
    }
}
