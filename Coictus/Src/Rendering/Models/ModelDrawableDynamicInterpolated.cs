
using OpenTK;

namespace Coictus.Models
{//TODO: impliment (ModelDrawableDynamicInterpolated)
    /*A ModelDrawableDynamicInterpolated has the functionality of a ModelDrawableDynamic (ability to combine frequently changing models into one draw call)
      While interpolating the transforms of each instance of models within it. Useful for dynamically drawing multiple moving objects with interpolation.
      This differs from ModelDrawableInstanced because it works with many different models instead of just one*/
    public class ModelDrawableDynamicInterpolated : ModelDrawableDynamic
    {
        /*this array of shader directories can be indexed with the ModelDrawType enum.
          each shader in this array must be a shader specifically made for interpolating a dynamic
          model. */
        private static string[] shaders = new string[] {
            ResourceUtil.getShaderFileDir(""),//ModelDrawType.triangles 
            ResourceUtil.getShaderFileDir(""),//ModelDrawType.points 
            ResourceUtil.getShaderFileDir(""),//ModelDrawType.singlePoint 
            ResourceUtil.getShaderFileDir("")//ModelDrawType.lines 
        };

        private ModelDrawType drawType;
        private uint maxInstanceCount;
        private Matrix4[] prevTickModelMatrices;
        private Matrix4[] modelMatrices;
        private Vector3[] prevTickSinglePointPositions;//these are only used for rendering single points
        private Vector3[] singlePointPositions;
        private int[] vertexCounts;//This array will hold the count of vertices for each seperate object combined into this model, so the shader can tell which transform to use with which set of verts

        /*indices can be null if they arent going to be used. e.g: point rendering
          parameter maxInstanceCount is the maximum number of models to be combined
          into this dynamic model. Determines the max number of model matrices to be
          sent to GPU and interpolated.*/
        public ModelDrawableDynamicInterpolated(uint maxInstanceCount, ModelDrawType drawType, string textureFile, uint[] indices, int maxVertexCount = 4000) : base(shaders[(int)drawType], textureFile, indices, maxVertexCount)
        {
            this.maxInstanceCount = maxInstanceCount;
            this.drawType = drawType;

            /*single points only use positions, not model matrices. This is more efficient.*/
            if(drawType ==  ModelDrawType.singlePoint)
            {
                prevTickSinglePointPositions = new Vector3[maxInstanceCount];
                singlePointPositions = new Vector3[maxInstanceCount];
            }
            else
            {
                prevTickModelMatrices = new Matrix4[maxInstanceCount];
                modelMatrices = new Matrix4[maxInstanceCount];
            }
        }
    }
}
