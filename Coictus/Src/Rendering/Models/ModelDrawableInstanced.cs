using OpenTK;
using System.Collections.Generic;

namespace Coictus.Models
{
    /*This class is for rendering the same model data many times with different transforms, colors and textures.
      It is more efficient to use this instead of ModelDrawableDynamic if its just multiple instances of one mesh.*/
    public class ModelDrawableInstanced
    {
        /*this array of shader directories can be indexed with the ModelDrawType enum.
          each shader in this array must be a shader specifically made for interpolating a dynamic
          model. */
        /*  private static string[] shaders = new string[] {
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.triangles 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.points 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.singlePoint 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.lines 
              ResourceUtil.getShaderFileDir(""),//ModelDrawType.billboardSpherical 
              ResourceUtil.getShaderFileDir("")//ModelDrawType.billboardCylindrical 
          };*/

        protected Model instance;
        protected uint[] indices;
        protected ModelDrawType drawType;
        protected List<Matrix4> transforms = null;
        protected List<Vector3> vecTransforms = null;//for when this model is for instancing points/sprites

        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(Model instance, uint[] indices, ModelDrawType drawType)
        {
            this.instance = instance;
            this.indices = indices;
            this.drawType = drawType;
            if(drawType == ModelDrawType.singlePoint)
            {
                vecTransforms = new List<Vector3>();
            }
            else
            {
                transforms = new List<Matrix4>();
            }
        }

        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(ModelDrawable instance, ModelDrawType drawType)
        {
            this.instance = instance;
            this.indices = instance.indices;
            this.drawType = drawType;
            if (drawType == ModelDrawType.singlePoint)
            {
                vecTransforms = new List<Vector3>();
            }
            else
            {
                transforms = new List<Matrix4>();
            }
        }

        /*when called adds a transform to the list and when this model is drawn, an
          instance will be drawn with the provided transform.*/
        public void addRenderAt(Matrix4 transform)
        {
            if (drawType == ModelDrawType.singlePoint)
            {
                Application.error("ModelDrawableInstanced was requested to add a matrix4 as a transform forsingle point drawing!");
            }
            else
            {
                transforms.Add(transform);
            }
        }

        public void addRenderAt(Vector3 transform)//for use with single points
        {
            if (drawType != ModelDrawType.singlePoint)
            {
                Application.error("ModelDrawableInstanced was requested to add a vector3 as a transform for non single point drawing!");
            }
            else
            {
                vecTransforms.Add(transform);
            }
        }
        //TODO: impliment (ModelDrawableInstanced)
    }
}
