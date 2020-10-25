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
        public static readonly int maxIndiciesCount = maxBufferSizeBytes / sizeof(uint);
        public static readonly int maxVertexCount = maxBufferSizeBytes / Vertex.vertexByteSize;
        public int maxPointCount = maxBufferSizeBytes / PointParticle.pParticleByteSize;
        public int maxMatrixCount = maxBufferSizeBytes / (sizeof(float) * 16);
        public int maxDrawCommandCount = maxBufferSizeBytes / DrawCommand.sizeInBytes;
        private VertexArrayObject VAO;
        private Model batchedModel = null;

        /*points are stored in a single VBO interlaced. all the batchedPoints come first, and then the prevTickBatchedPoints are packed after them.*/
        private PointParticle[] batchedPoints = null;
        private PointParticle[] prevTickBatchedPoints = null;

        /*Matrices are stored in a single VBO interlaced. all the modelMatrices come first, and then the prevTickModelMatrices are packed after them.*/
        private Matrix4[] prevTickModelMatrices = null;
        private Matrix4[] modelMatrices = null;

        private DrawCommand[] drawCommands = null;

        private bool usesLerping = false;
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
                case BatchType.guiCutout:
                    ShaderUtil.tryGetShader(ShaderUtil.guiCutoutName, out batchShader);
                    break;

                case BatchType.text2D:
                    ShaderUtil.tryGetShader(ShaderUtil.text2DName, out batchShader);
                    break;

                case BatchType.text3D:
                    ShaderUtil.tryGetShader(ShaderUtil.text3DName, out batchShader);
                    break;

                case BatchType.triangles:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out batchShader);
                    break;

                case BatchType.trianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesTransparentName, out batchShader);
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
                    usesLerping = true;
                    break;

                case BatchType.lerpTrianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesTransparentName, out batchShader);
                    maxMatrixCount /= 2;
                    prevTickModelMatrices = new Matrix4[maxMatrixCount];
                    modelMatrices = new Matrix4[maxMatrixCount];
                    drawCommands = new DrawCommand[maxDrawCommandCount];
                    usesLerping = true;
                    break;

                case BatchType.lerpLines:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out batchShader);
                    maxMatrixCount /= 2;
                    prevTickModelMatrices = new Matrix4[maxMatrixCount];
                    modelMatrices = new Matrix4[maxMatrixCount];
                    drawCommands = new DrawCommand[maxDrawCommandCount];
                    usesLerping = true;
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
           
            if (usesLerping)
            { 
                //if this batch cant fit the matrices or draw commands then return false
                if (matricesItterator + 1 >= maxMatrixCount) return false;
                if (requestedObjectItterator + 1 >= maxDrawCommandCount) return false;
                modelMatrices[matricesItterator] = theModel.modelMatrix;
                prevTickModelMatrices[matricesItterator] = theModel.prevModelMatrix;
                configureDrawCommandsForCurrentObject(theModel.vertices.Length);
                ++matricesItterator;
            }

            theModel.setObjectID(requestedObjectItterator);
            
            Array.Copy(theModel.vertices, 0, batchedModel.vertices, requestedVerticesCount, theModel.vertices.Length);

            for (int i = 0; i < theModel.indices.Length; ++i)
            {
                batchedModel.indices[requestedIndicesCount + i] = (uint)(theModel.indices[i] + requestedVerticesCount);
            }

            requestedVerticesCount += theModel.vertices.Length;
            requestedIndicesCount += theModel.indices.Length;
            requestedObjectItterator++;
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

        /// <summary>
        /// Sets up and configures vertexattribptrs and vertexattribdivisors for the current requested object.
        /// This is required for LERP type batches for selecting the correct matrices.
        /// </summary>
        private void configureDrawCommandsForCurrentObject(int objVertCount)
        {
            drawCommands[requestedObjectItterator] = new DrawCommand((uint)objVertCount, 1, 0, (uint)requestedVerticesCount, (uint)requestedObjectItterator);
        }

        public void onTickStart()
        {
            requestedVerticesCount = 0;
            requestedIndicesCount = 0;
            requestedObjectItterator = 0;
            matricesItterator = 0;
            hasBeenUpdated = false;
        }

        /// <summary>
        /// After each tick, all the batches must submit their updated render data to the GPU to prepare for all draw calls.
        /// </summary>
        public void onTickEnd()
        {
            //decrement these so they are the values of the actual current amounts
            matricesItterator--;
            requestedObjectItterator--;
            if (usesLerping)
            {
                VAO.bindMatricesVBO();
                //interlacing both arrays into one to submit to gpu
                //using matricesItterator to only interlace and submit the necessary amount of data.
                Matrix4[] b;
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (matricesItterator * 2) * (sizeof(float) * 16), b = MatrixCombiner.interlaceMatrixArraysByCount(modelMatrices, prevTickModelMatrices, matricesItterator));
                //TODO: THIS IS FUCKED SOMEHOWW
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
            else
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (requestedVerticesCount * 2) *  PointParticle.pParticleByteSize, PointCombiner.interlacePointArraysByCount(batchedPoints, prevTickBatchedPoints, requestedVerticesCount));
            }
        }
        
        public void draw(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 fogColor)
        {
            VAO.bindVaoVboIbo();
            batchShader.use();
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

            if (batchTex != null && !batchTex.getIsNone())
            {
                batchTex.use();
            }

            if (usesLerping)
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
            return hasBeenUpdated;
        }
        public void delete()
        {
            VAO.delete();
        }
    }
}
