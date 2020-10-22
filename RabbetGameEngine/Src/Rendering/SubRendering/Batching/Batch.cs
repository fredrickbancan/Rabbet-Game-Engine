using OpenTK;
using RabbetGameEngine.Models;

namespace RabbetGameEngine.SubRendering
{
    /*Class for containing information of a render batch. New batches will need to be created for 
      different types of rendering and/or if a previous batch is too full.*/
    public class Batch//TODO: impliment batching and render requests, All methods here must be completed.
    {
        public static readonly int maxVertexCount = 2500000 / Vertex.vertexByteSize;
        private VertexArrayObject VAO;//TODO Impliment functionality of VertexArrayObject and use here.
        private Vertex[] batchedVertices = null;
        private Vertex[] batchedPoints = null;
        private Vertex[] prevTickBatchedPoints = null;
        private Matrix4[] prevTickModelMatrices = null;
        private Matrix4[] modelMatrices = null;
        private int matricesItterator = 0;
        private int submittedVerticesCount = 0;
        private uint[] indices;
        private BatchType batchType;
        private Texture batchTex;
        private Shader batchShader;
        private bool hasBeenUpdated = false;

        public Batch(BatchType type, Texture tex)
        {
            batchType = type;
            batchTex = tex;
            VAO = VertexArrayObject.createDynamic(type, maxVertexCount);
            selectShaderAndInitArrays();
        }

        private void selectShaderAndInitArrays()
        {
            switch (batchType)
            {
                case BatchType.none:
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    break;

                case BatchType.text2D:
                    ShaderUtil.tryGetShader("GuiText", out batchShader);
                    break;

                case BatchType.text3D:
                    Application.warn("Unimplimented shader for batch type text3D!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    break;

                case BatchType.triangles:
                    ShaderUtil.tryGetShader("EntityWorld_F", out batchShader);
                    break;

                case BatchType.trianglesTransparent:
                    ShaderUtil.tryGetShader("EntityWorld_FT", out batchShader);
                    break;

                case BatchType.lines:
                    Application.warn("Unimplimented shader for batch type lines!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    break;

                case BatchType.lerpTriangles:
                    Application.warn("Unimplimented shader for batch type lerpTriangles!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;

                case BatchType.lerpTrianglesTransparent:
                    Application.warn("Unimplimented shader for batch type lerpTrianglesTransparent!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;

                case BatchType.lerpPointCloud:
                    ShaderUtil.tryGetShader("PParticleCloud_F", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;

                case BatchType.lerpPointCloudTransparent:
                    ShaderUtil.tryGetShader("PParticleCloud_FT", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;

                case BatchType.lerpSinglePoint:
                    Application.warn("Unimplimented shader for batch type lerpSinglePoint!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickBatchedPoints = new Vertex[maxVertexCount];
                    batchedPoints = new Vertex[maxVertexCount];
                    break;

                case BatchType.lerpSinglePointTransparent:
                    Application.warn("Unimplimented shader for batch type lerpSinglePointTransparent!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickBatchedPoints = new Vertex[maxVertexCount];
                    batchedPoints = new Vertex[maxVertexCount];
                    break;

                case BatchType.lerpLines:
                    Application.warn("Unimplimented shader for batch type lerpLines!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;

                default:
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    break;
            }
        }

        //For dynamic vertex objects, when submitting data, the residual un-updated data at the end of the buffer does not need to be cleared.
        //use the submittedVerticesCount or something similar with drawElements(count) to only draw the submitted vertices and ignore the residual ones.
        //This is faster than clearing the whole buffer each update.
        /// <summary>
        /// attempts to add the provided data to the batch. Returns true if successful, and returns false if not enough room.
        /// </summary>
        /// <param name="theModel">The model to be added</param>
        /// <returns>true if the provided model can be added</returns>
        public bool addToBatch(Model theModel)
        {
            if (submittedVerticesCount + theModel.vertices.Length >= maxVertexCount)
            {
                return false;
            }
            
            hasBeenUpdated = true;
            return true;
        }

        public void onUpdate()
        {
            submittedVerticesCount = 0;
            hasBeenUpdated = false;
        }
        private void prepareForDraw()
        {
            VAO.bind();
            batchShader.use();
            batchTex.use();

            //do buffer submissions here
        }
        public void draw()
        {
            prepareForDraw();

            switch (batchType)
            {
                case BatchType.none:
                    break;

                case BatchType.text2D:
                    break;

                case BatchType.text3D:
                    break;

                case BatchType.triangles:
                    break;

                case BatchType.trianglesTransparent:
                    break;

                case BatchType.lines:
                    break;

                case BatchType.lerpTriangles:
                    break;

                case BatchType.lerpTrianglesTransparent:
                    break;

                case BatchType.lerpPointCloud:
                    break;

                case BatchType.lerpPointCloudTransparent:
                    break;

                case BatchType.lerpSinglePoint:
                    break;

                case BatchType.lerpSinglePointTransparent:
                    break;

                case BatchType.lerpLines:
                    break;

                default:
                    break;
            }
        }

        public BatchType getBatchType()
        {
            return this.batchType;
        }

        public Texture getBatchtexture()
        {
            return batchTex;
        }


        public bool hasBeenUsedSinceLastUpdate()
        {
            return hasBeenUpdated;
        }
        public void delete()
        {
            VAO.delete();
        }
    }
}
