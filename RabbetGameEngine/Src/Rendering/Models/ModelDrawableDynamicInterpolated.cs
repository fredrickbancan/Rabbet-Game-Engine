
using OpenTK;
using OpenTK.Graphics.ES11;

namespace RabbetGameEngine.Models
{//TODO: impliment functionality of ModelDrawableDynamicInterpolated
    /*A ModelDrawableDynamicInterpolated has the functionality of a ModelDrawableDynamic (ability to combine frequently changing models into one draw call)
      While interpolating the transforms of each instance of models within it. Useful for dynamically drawing multiple moving objects with interpolation.
      This differs from ModelDrawableInstanced because it works with many different models instead of just one*/
    public class ModelDrawableDynamicInterpolated : ModelDrawableDynamic
    {
        private PrimitiveType drawType;
        private uint maxInstanceCount;
        private Matrix4[] prevTickModelMatrices;
        private Matrix4[] modelMatrices;
     //   private Vector3[] prevTickSinglePointPositions;//these are only used for rendering single points
     //   private Vector3[] singlePointPositions;
       // private int[] vertexCounts;//This array will hold the count of vertices for each seperate object combined into this model, so the shader can tell which transform to use with which set of verts

        /*indices can be null if they arent going to be used. e.g: point rendering
          parameter maxInstanceCount is the maximum number of models to be combined
          into this dynamic model. Determines the max number of model matrices to be
          sent to GPU and interpolated.*/
        public ModelDrawableDynamicInterpolated(uint maxInstanceCount, PrimitiveType drawType, string textureFile, uint[] indices, int maxVertexCount = 4000) : base(null, textureFile, indices, maxVertexCount)
        {
            this.maxInstanceCount = maxInstanceCount;
            this.drawType = drawType;
            prevTickModelMatrices = new Matrix4[maxInstanceCount];
            modelMatrices = new Matrix4[maxInstanceCount];
        }
    }
}
