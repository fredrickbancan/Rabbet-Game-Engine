using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine.Models
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

        protected ModelDrawable instance;
        protected uint[] indices;
        protected ModelDrawType drawType;

         //TODO: solve instance individualism, set shader based on draw type (ModelDrawableInstanced)
        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(ModelDrawable instance, uint[] indices, ModelDrawType drawType)
        {
            this.instance = instance;
            this.indices = indices;
            this.drawType = drawType;
        }

        /*the provided model instance will be re-drawn at each transform*/
        public ModelDrawableInstanced(ModelDrawable instance, ModelDrawType drawType)
        {
            this.instance = instance;
            this.indices = instance.indices;
            this.drawType = drawType;
            Shader shader;
            ShaderUtil.tryGetShader("ColorTextureFogInstanced3D.shader", out shader);
            instance.setShader(shader);
        }

        /*when called adds a transform to the list and when this model is drawn, an
          instance will be drawn with the provided transform.*/
        public void addRenderAt(Matrix4 transform)
        {
        }

        public void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            instance.bind();
            instance.getShader().setUniformMat4F("viewMatrix", viewMatrix); 
            instance.getShader().setUniformMat4F("projectionMatrix", projectionMatrix); 
            instance.getShader().setUniformVec3F("fogColor", fogColor); 
            switch (drawType)
            {
                case ModelDrawType.points:
                    GL.DrawArraysInstanced(PrimitiveType.Points, 0, instance.vertices.Length, 0);
                    break;

                case ModelDrawType.lines:
                    GL.DrawElementsInstanced(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, System.IntPtr.Zero, 0);
                    break;

                default:
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, System.IntPtr.Zero, 0);
                    break;
            }
        }
        
    }
}
