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
        public static int maxBufferSizeBytes = 16777216;
        public static int maxIndiciesCount = maxBufferSizeBytes / sizeof(uint);
        public static int maxVertexCount = maxBufferSizeBytes / Vertex.vertexByteSize;
        public int maxPointCount = maxBufferSizeBytes / PointParticle.pParticleByteSize;
        public int maxMatrixCount = maxBufferSizeBytes / (sizeof(float) * 16);
        public int maxDrawCommandCount = maxBufferSizeBytes / DrawCommand.sizeInBytes;
        private bool usingLerpPoints = false;

        /// <summary>
        /// true if this batch requires transparency sorting
        /// </summary>
        public bool requiresSorting = false;

        /// <summary>
        /// true if this batch should be rendered last.
        /// </summary>
        public bool transparentGUI = false;
        private VertexArrayObject VAO;
        private Model batchedModel = null;

        /*points are stored in a single VBO half packed, meaning the prevTickBatchedPoints start in the second half of the array. all the batchedPoints come first, and then the prevTickBatchedPoints are packed after them.*/
        private PointParticle[] batchedPoints = null;
        private PointParticle[] prevTickBatchedPoints = null;

        /*Matrices are stored in a single VBO interlaced. all the modelMatrices come first, and then the prevTickModelMatrices are packed after them.*/
        private Matrix4[] prevTickModelMatrices = null;
        private Matrix4[] modelMatrices = null;

        private DrawCommand[] drawCommands = null;

        private bool usesMultiDrawLerp = false;
        private bool pointBased = false;

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

        public Batch(BatchType type, Texture tex)
        {
            if(type == BatchType.lerpISpheres || type == BatchType.lerpISpheresTransparent)
            {
                Application.error("Wrong batch constructor being used for batch of type points!!!");
                return;
            }
            batchType = type;
            batchTex = tex;
            VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
            batchedModel = new Model(new Vertex[maxVertexCount], new uint[maxIndiciesCount]);
            initializeBatchFormat();
        }

        public Batch(bool pointTransparency, bool lerp)
        {
            pointBased = true;
            if(pointTransparency)
            {
                if(!lerp)
                {
                    batchType = BatchType.iSpheresTransparent;
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresTransparentName, out batchShader);
                    requiresSorting = true;
                    VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
                    batchedPoints = new PointParticle[maxPointCount];
                    return;
                }
                else
                {
                    batchType = BatchType.lerpISpheresTransparent;
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresTransparentName, out batchShader);
                    VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
                    requiresSorting = true;
                    usingLerpPoints = true;
                    maxPointCount /= 2; //single points require a copy of each vertex, meaning we can only accept half the number of vertices.
                    prevTickBatchedPoints = new PointParticle[maxPointCount];
                    batchedPoints = new PointParticle[maxPointCount];
                    return;
                }
            }
            else
            {
                if (!lerp)
                {
                    batchType = BatchType.iSpheres;
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out batchShader);
                    VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
                    batchedPoints = new PointParticle[maxPointCount];
                    return;
                }
                else
                {
                    batchType = BatchType.lerpISpheres;
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresName, out batchShader);
                    usingLerpPoints = true;
                    VAO = VertexArrayObject.createDynamic(batchType, maxBufferSizeBytes);
                    maxPointCount /= 2; //single points require a copy of each vertex, meaning we can only accept half the number of vertices.
                    prevTickBatchedPoints = new PointParticle[maxPointCount];
                    batchedPoints = new PointParticle[maxPointCount];
                    return;
                }
            }
           
        }

        private void initializeBatchFormat()
        {
            switch (batchType)
            {
                case BatchType.none:
                    ShaderUtil.tryGetShader("debug", out batchShader);
                    break;

                case BatchType.guiCutout:
                    ShaderUtil.tryGetShader(ShaderUtil.guiCutoutName, out batchShader);
                    break;

                case BatchType.guiText:
                    ShaderUtil.tryGetShader(ShaderUtil.text2DName, out batchShader);
                    transparentGUI = true;
                    break;

                case BatchType.text3D:
                    ShaderUtil.tryGetShader(ShaderUtil.text3DName, out batchShader);
                    break;

                case BatchType.triangles:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out batchShader);
                    break;

                case BatchType.trianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesTransparentName, out batchShader);
                    requiresSorting = true;
                    break;

                case BatchType.lines:
                    ShaderUtil.tryGetShader(ShaderUtil.linesName, out batchShader);
                    break;

                case BatchType.lerpTriangles:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out batchShader);
                    maxMatrixCount /= 2;
                    prevTickModelMatrices = new Matrix4[maxMatrixCount];
                    modelMatrices = new Matrix4[maxMatrixCount];
                    drawCommands = new DrawCommand[maxDrawCommandCount];
                    usesMultiDrawLerp = true;
                    break;

                case BatchType.lerpTrianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesTransparentName, out batchShader);
                    maxMatrixCount /= 2;
                    prevTickModelMatrices = new Matrix4[maxMatrixCount];
                    modelMatrices = new Matrix4[maxMatrixCount];
                    drawCommands = new DrawCommand[maxDrawCommandCount];
                    usesMultiDrawLerp = true;
                    requiresSorting = true;
                    break;

                case BatchType.lerpLines:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out batchShader);
                    maxMatrixCount /= 2;
                    prevTickModelMatrices = new Matrix4[maxMatrixCount];
                    modelMatrices = new Matrix4[maxMatrixCount];
                    drawCommands = new DrawCommand[maxDrawCommandCount];
                    usesMultiDrawLerp = true;
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
            //if this batch cant fit the new models vertices or indices
            if (requestedVerticesCount + theModel.vertices.Length >= maxVertexCount || requestedIndicesCount + theModel.indices.Length >= maxIndiciesCount)
            {
                return false;
            }
           
            if (usesMultiDrawLerp)
            { 
                //if this batch cant fit the matrices or draw commands then return false
                if (requestedObjectItterator + 1 >= maxMatrixCount) return false;
                if (requestedObjectItterator + 1 >= maxDrawCommandCount) return false;
                modelMatrices[requestedObjectItterator] = theModel.modelMatrix;
                prevTickModelMatrices[requestedObjectItterator] = theModel.prevModelMatrix;
                configureDrawCommandsForCurrentObject(theModel.indices.Length, theModel.vertices.Length);
                Array.Copy(theModel.indices, 0, batchedModel.indices, requestedIndicesCount, theModel.indices.Length);
            }
            else
            {
                for (int i = 0; i < theModel.indices.Length; ++i)
                {
                    batchedModel.indices[requestedIndicesCount + i] = (uint)(theModel.indices[i] + requestedVerticesCount);
                }

            }

            //not being used currently
           // theModel.setObjectID(requestedObjectItterator);
            
            Array.Copy(theModel.vertices, 0, batchedModel.vertices, requestedVerticesCount, theModel.vertices.Length);

            requestedVerticesCount += theModel.vertices.Length;
            requestedIndicesCount += theModel.indices.Length;
            requestedObjectItterator++;
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
            return true;
        }

        public bool addToBatch(PointParticle singlePoint)
        {
            if (requestedVerticesCount + 1 >= maxPointCount)
            {
                return false;
            }
            batchedPoints[requestedVerticesCount] = singlePoint;
            ++requestedVerticesCount;
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
            if(usingLerpPoints)
            Array.Copy(theModel.prevPoints, 0, prevTickBatchedPoints, requestedVerticesCount, theModel.prevPoints.Length);
            requestedVerticesCount += theModel.points.Length;
            return true;
        }

        /// <summary>
        /// Sets up and configures vertexattribptrs and vertexattribdivisors for the current requested object.
        /// This is required for LERP type batches for selecting the correct matrices.
        /// </summary>
        private void configureDrawCommandsForCurrentObject(int objIndCount, int vertCount)
        {
            drawCommands[requestedObjectItterator] = new DrawCommand((uint)(objIndCount), (uint)(1), (uint)(requestedIndicesCount), (uint)(requestedVerticesCount), (uint)(requestedObjectItterator));
        }

        public void onTickStart()
        {
            requestedVerticesCount = 0;
            requestedIndicesCount = 0;
            requestedObjectItterator = 0;
        }

        /// <summary>
        /// After each tick, all the batches must submit their updated render data to the GPU to prepare for all draw calls.
        /// </summary>
        public void onTickEnd()
        {
            if (usesMultiDrawLerp)
            {
                VAO.bindMatricesVBO();
                //interlacing both arrays into one to submit to gpu
                //using matricesItterator to only interlace and submit the necessary amount of data.
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (requestedObjectItterator * 2) * (sizeof(float) * 16),  MatrixCombiner.interlaceMatrixArraysByCount(modelMatrices, prevTickModelMatrices, requestedObjectItterator));
                
                VAO.bindIndirectBufferObject();
                GL.BufferSubData(BufferTarget.DrawIndirectBuffer, IntPtr.Zero, requestedObjectItterator * DrawCommand.sizeInBytes, drawCommands);
            }

            VAO.bindVBO();
            VAO.bindIBO();

            if(!pointBased)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, requestedVerticesCount * Vertex.vertexByteSize, batchedModel.vertices);
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, requestedIndicesCount * sizeof(uint), batchedModel.indices);
            }
            else if(usingLerpPoints)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, requestedVerticesCount * 2 * PointParticle.pParticleByteSize, PointCombiner.interlacePointArraysByCount(batchedPoints, prevTickBatchedPoints, requestedVerticesCount));
            }
            else
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, requestedVerticesCount * PointParticle.pParticleByteSize, batchedPoints);
            }
        }
        
        public void draw( Matrix4 viewMatrix, Vector3 fogColor)
        {
            VAO.bindVaoVboIbo();
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchShader.setUniformMat4F("viewMatrix", viewMatrix);
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
            batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
            batchShader.setUniformVec3F("fogColor", fogColor);
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            batchShader.setUniform1I("frame", Renderer.frame);
            batchShader.setUniformVec2F("viewPortSize", Renderer.useOffScreenBuffer ? new Vector2(OffScreen.getWidth, OffScreen.getHeight) : new Vector2(GameInstance.get.Width, GameInstance.get.Height));

            if(pointBased)
            {
                VAO.bindInstVBO();
                GL.DrawArraysInstanced(VAO.getPrimType(), 0, 4, requestedVerticesCount);
                return;
            }

            if (batchTex != null && !batchTex.getIsNone())
            {
                batchTex.use();
            }

            if (usesMultiDrawLerp)
            {
                VAO.bindMatricesVBO();
                VAO.bindIndirectBufferObject();
                GL.MultiDrawElementsIndirect(VAO.getPrimType(), DrawElementsType.UnsignedInt, IntPtr.Zero, requestedObjectItterator, 0);
                return;
            }
            
            GL.DrawElements(VAO.getPrimType(), requestedIndicesCount, DrawElementsType.UnsignedInt, 0);
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
            return requestedVerticesCount > 0;
        }
        public void delete()
        {
            VAO.delete();
        }
    }
}
