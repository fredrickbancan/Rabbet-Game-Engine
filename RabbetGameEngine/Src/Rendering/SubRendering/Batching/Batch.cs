using OpenTK;
using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine.SubRendering
{
    /*Class for containing information of a render batch. New batches will need to be created for 
      different types of rendering and/or if a previous batch is too full.*/
    public class Batch
    {
        public static readonly int maxBufferSizeBytes = 2500000;
        public int maxVertexCount = maxBufferSizeBytes / Vertex.vertexByteSize;
        public int maxIndiciesCount = maxBufferSizeBytes / sizeof(uint);
        public int maxPointCount = maxBufferSizeBytes / PointParticle.pParticleByteSize;
        private VertexArrayObject VAO;
        private Model batchedModel = null;
        private PointParticle[] batchedPoints = null;
        private PointParticle[] prevTickBatchedPoints = null;
        private Matrix4[] prevTickModelMatrices = null;
        private Matrix4[] modelMatrices = null;
        private bool usingMatricesUniforms = false;
        private bool pointBased = false;
        private int matricesItterator = 0;

        /// <summary>
        /// number of individual objects requested. This must be used as an identifier for each vertex of 
        /// the individual objects so the shader can determine which model matrices to use to transform it.
        /// </summary>
        private int requestedObjectItterator = 0;

        /// <summary>
        /// number of vertices requested to be added to this batch since the last update.
        /// </summary>
        private int requestedVerticesCount = 0;

        /// <summary>
        /// number of indices requested to be added to this batch since the last update
        /// </summary>
        private int requestedIndicesCount = 0;

        private BatchType batchType;
        private Texture batchTex;
        private Shader batchShader;
        private bool hasBeenUpdated = false;

        public Batch(BatchType type, Texture tex)
        {
            if(type == BatchType.lerpPoints || type == BatchType.lerpPointsTransparent)
            {
                Application.error("Wrong batch constructor being used for batch type of points!!!");
                return;
            }
            batchType = type;
            batchTex = tex;
            VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
            batchedModel = new Model(new Vertex[maxVertexCount], new uint[maxIndiciesCount]);
            initializeBatchFormat();
        }

        public Batch(bool pointTransparency)
        {
            batchType = pointTransparency ? BatchType.lerpPointsTransparent : BatchType.lerpPoints;
            VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
            Application.warn("Unimplimented shader for batch type lerpPoints!, using debug shader instead.");
            ShaderUtil.tryGetShader("debug", out batchShader);
            maxPointCount /= 2; //single points require a copy of each vertex, meaning we can only accept half the number of vertices.
            prevTickBatchedPoints = new PointParticle[maxPointCount];
            batchedPoints = new PointParticle[maxPointCount];
            pointBased = true;
        }

        private void initializeBatchFormat()
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
                    ShaderUtil.tryGetShader("EntityWorld_FNEW", out batchShader);
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

                case BatchType.lerpLines:
                    Application.warn("Unimplimented shader for batch type lerpLines!, using debug shader instead.");
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    prevTickModelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    modelMatrices = new Matrix4[RenderAbility.maxUniformMat4];
                    break;
            }
        }

        //For dynamic vertex objects, when submitting data, the residual un-updated data at the end of the buffer does not need to be cleared.
        //use the submittedVerticesCount or something similar with drawElements(count) to only draw the submitted vertices and ignore the residual ones.
        //This is faster than clearing the whole buffer each update.
        /// <summary>
        /// attempts to add the provided model data to the batch. Returns true if successful, and returns false if not enough room.
        /// </summary>
        /// <param name="theModel">The model to be added</param>
        /// <returns>true if the provided model can be added</returns>
        public bool addToBatch(Model theModel)
        {
            //if this batch cant fit the new models vertices
            if (requestedVerticesCount + theModel.vertices.Length >= maxVertexCount || requestedIndicesCount + theModel.indices.Length >= maxIndiciesCount)
            {
                return false;
            }
           
            if (usingMatricesUniforms)
            { 
                //if this batch uses matrices unforms for linear interpolation and cant fit the new matrix
                if (matricesItterator + 1 >= RenderAbility.maxUniformMat4) return false;
                modelMatrices[matricesItterator] = theModel.modelMatrix;
                prevTickModelMatrices[matricesItterator] = theModel.prevModelMatrix;
                ++matricesItterator;
            }

            theModel.setObjectID(requestedObjectItterator++);

            Array.Copy(theModel.vertices, 0, batchedModel.vertices, requestedVerticesCount, theModel.vertices.Length);
            
            for(int i = 0; i < theModel.indices.Length; ++i)
            {
                batchedModel.indices[requestedIndicesCount + i] = (uint)(theModel.indices[i] + requestedVerticesCount);
            }

            requestedVerticesCount += theModel.vertices.Length;
            requestedIndicesCount += theModel.indices.Length;
            hasBeenUpdated = true;
            return true;
        }

        /// <summary>
        /// attempts to add the provided single point to the batch. This method only works for 
        /// batches with have a batch type of single points!
        /// </summary>
        /// <param name="singlePoint">The single point</param>
        /// <param name="prevTickSinglePoint">The single point's previous tick data.</param>
        /// <returns>true if this batch is able to accept the point.</returns>
        public bool addToBatch(PointParticle singlePoint, PointParticle prevTickSinglePoint)
        {
            if(requestedVerticesCount + 1 >= maxPointCount)
            {
                return false;
            }
            batchedPoints[requestedVerticesCount] = singlePoint;
            prevTickBatchedPoints[requestedVerticesCount] = prevTickSinglePoint;
            ++requestedVerticesCount;
            hasBeenUpdated = true;
            return true;
        }

        /// <summary>
        /// attempts to add the provided single point to the batch. This method only works for 
        /// batches with have a batch type of single points!
        /// </summary>
        /// <param name="singlePoint">The single point</param>
        /// <param name="prevTickSinglePoint">The single point's previous tick data.</param>
        /// <returns>true if this batch is able to accept the point.</returns>
        public bool addToBatch(PointCloudModel theModel)
        {
            if (requestedVerticesCount + theModel.points.Length >= maxPointCount)
            {
                return false;
            }
            Array.Copy(theModel.points, 0, batchedPoints, requestedVerticesCount, theModel.points.Length);
            Array.Copy(theModel.prevPoints, 0, prevTickBatchedPoints, requestedVerticesCount, theModel.prevPoints.Length);
            requestedVerticesCount += theModel.points.Length;
            hasBeenUpdated = true;
            return true;
        }

        public void onTickStart()
        {
            requestedVerticesCount = 0;
            matricesItterator = 0;
            hasBeenUpdated = false;
        }

        public void onTickEnd()
        {
            VAO.bindVBO();
            //do buffer submissions here.
            if(pointBased)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (requestedVerticesCount * 2) *  PointParticle.pParticleByteSize, PointCombiner.addPointsToArray(batchedPoints, prevTickBatchedPoints));
            }
            else
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, maxBufferSizeBytes, batchedModel.vertices);

                VAO.bindIBO();
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, maxBufferSizeBytes, batchedModel.indices);
            }
        }


        
        public void draw(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 fogColor)
        {
            VAO.bind();
            batchShader.use();
            if (batchTex != null && !batchTex.getIsNone())
            {
                batchTex.use();
            }
            batchShader.setUniformMat4F("projectionMatrix", projectionMatrix);
            batchShader.setUniformMat4F("viewMatrix", viewMatrix);
            batchShader.setUniformVec3F("fogColor", fogColor);
            batchShader.setUniform1F("percentageToNextTick", TicksAndFps.getPercentageToNextTick());
            batchShader.setUniform1I("frame", Renderer.frame);
            batchShader.setUniformVec2F("viewPortSize", Renderer.useOffScreenBuffer ? new Vector2(OffScreen.getWidth, OffScreen.getHeight) : new Vector2(GameInstance.get.Width, GameInstance.get.Height));

            if(pointBased)
            {
                GL.DrawArrays(PrimitiveType.Points, 0, requestedVerticesCount);
                return;
            }

            if (usingMatricesUniforms)
            {
                batchShader.setUniformMat4FArray("modelMatrices", modelMatrices);
                batchShader.setUniformMat4FArray("prevTickModelMatrices", prevTickModelMatrices);
            }

            GL.DrawElements(PrimitiveType.Triangles, batchedModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            return;
        }

        public BatchType getBatchType()
        {
            return this.batchType;
        }

        public Texture getBatchtexture()
        {
            return batchTex;
        }


        public bool hasBeenUsedInCurrentTick()
        {
            return hasBeenUpdated;
        }
        public void delete()
        {
            VAO.delete();
        }
    }
}
