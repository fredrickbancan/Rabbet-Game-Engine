using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine.SubRendering
{
    //TODO: Change all text and gui quad rendering to use GL_QUADS so they do not require indices.
    //TODO: DrawCommands are made for glmultidrawelementsindirect, so they need to be set up differently for glmultidrawarraysindirect. namely basevertex. This can be done using an offset and using the basevertex value as baseinstance and skippuing over 4 bytes.
    //TODO: Implement new VAO system and adding to batches with resizing arrays AND vao buffers.
    //Too many if statements in adding to batches is slow.
    //complexity increases when adding new data types/arrays
    public class Batch
    { 
        public static readonly int vectorSize = sizeof(float) * 3;
        public static readonly int matrixSize = sizeof(float) * 16;
        public static readonly int initialArraySize = 32;
        public static readonly int baseMaxBufferSizeBytes = 8388608;
        public int maxBufferSizeBytes = baseMaxBufferSizeBytes;
        private int maxIndiciesCount;
        private int maxVertexCount;
        private int maxPositionCount;
        private int maxPointCount;
        private int maxSprite3DCount;
        private int maxMatrixCount;
        private int maxDrawCommandCount;

        /// <summary>
        /// true if this batch requires transparency sorting
        /// </summary>
        public bool requiresSorting = false;

        /// <summary>
        /// true if this batch should be rendered last.
        /// </summary>
        public bool transparentGUI = false;

        private VertexArrayObject VAO;
        public Model batchedModel = null;

        /*points are stored in a single VBO half packed, meaning the prevTickBatchedPoints start in the second half of the array. all the batchedPoints come first, and then the prevTickBatchedPoints are packed after them.*/
        public PointParticle[] batchedPoints = null;

        /*Matrices are stored in a single VBO interlaced. all the modelMatrices come first, and then the prevTickModelMatrices are packed after them.*/
        public Matrix4[] modelMatrices = null;

        /*If using prev positions for lerping, they will be stored in a single vbo interlaced. Otherwise if positions are being used, they are stored flat in a single vbo.*/
        public Vector3[] positions = null;

        public Vector3[] scales = null;

        public DrawCommand[] drawCommands = null;

        /// <summary>
        /// number of individual objects requested. This must be used as an identifier for each vertex of 
        /// the individual objects so the shader can determine which model matrices to use to transform it.
        /// </summary>
        public int requestedObjectItterator = 0;

        /// <summary>
        /// Used for properly interlacing and including new requests for lerp triangle types which require 2 matrices per object
        /// </summary>
        public int matricesItterator = 0;

        /// <summary>
        /// Used for properly interlacing and including new requests for lerp points which require 2 points per point.
        /// </summary>
        public int pointsItterator = 0;

        /// <summary>
        /// Used for properly interlacing and including new requests for lerp 3d text or any other type which uses 2 positions per object.
        /// </summary>
        public int positionItterator = 0;


        /// <summary>
        /// number of vertices requested to be added to this batch since the last update.
        /// </summary>
        public int requestedVerticesCount = 0;

        /// <summary>
        /// number of indices requested to be added to this batch since the last update
        /// </summary>
        public int requestedIndicesCount = 0;

        private BatchType batchType;
        public Texture batchTex;
        public Shader batchShader;

        public Batch(BatchType type, Texture tex)
        {
            batchType = type;
            batchTex = tex;
            BatchUtil.buildBatch(this);
            calculateBatchLimitations();
        }

        public void calculateBatchLimitations()
        {
            maxIndiciesCount = maxBufferSizeBytes / sizeof(uint);
            maxVertexCount = maxBufferSizeBytes / Vertex.vertexByteSize;
            maxDrawCommandCount = maxBufferSizeBytes / DrawCommand.sizeInBytes;
            maxMatrixCount = maxBufferSizeBytes / (sizeof(float) * 16);
            maxPositionCount = maxBufferSizeBytes / (sizeof(float) * 3);
            maxSprite3DCount = maxBufferSizeBytes / Sprite3D.sizeInBytes;
            maxPointCount = maxBufferSizeBytes / PointParticle.pParticleByteSize;
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
            return false;
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
            return false;
        }

        public bool addToBatch(PointParticle singlePoint)
        {
            return false;
        }

        public bool addToBatch(PointCloudModel theModel)
        {
            return false;
        }

        /// <summary>
        /// This is required for LERP type batches for selecting the correct matrices.
        /// </summary>
        private void configureDrawCommandsForCurrentObject(int objIndCount, int vertCount)
        {
            int n;
            if((n = requestedObjectItterator + 1) >= drawCommands.Length)//resizing drawcommands
            {
                if((n*=2) >= maxDrawCommandCount)
                {
                    Array.Resize<DrawCommand>(ref drawCommands, maxDrawCommandCount);
                    VAO.resizeIndirect(maxDrawCommandCount);
                }
                else
                {
                    Array.Resize<DrawCommand>(ref drawCommands, n);
                    VAO.resizeIndirect(n);
                }
            }

            drawCommands[requestedObjectItterator] = new DrawCommand((uint)(objIndCount), (uint)(1), (uint)(requestedIndicesCount), (uint)(requestedVerticesCount), (uint)(requestedObjectItterator));
        }

        public void beforeTick()
        {
            requestedVerticesCount = 0;
            requestedIndicesCount = 0;
            requestedObjectItterator = 0;
            matricesItterator = 0;
            pointsItterator = 0;
            positionItterator = 0;
        }

        /// <summary>
        /// After each tick, all the batches must submit their updated render data to the GPU to prepare for all draw calls.
        /// </summary>
        public void onTickEnd()
        {
           //TODO: vao buffer updates
        }
        
        public void draw( Matrix4 viewMatrix, Vector3 fogColor)
        {
            BatchUtil.drawBatch(this, viewMatrix, fogColor);
        }

        public void bindVAO()
        {
            VAO.bind();
        }

        public BatchType getBatchType()
        {
            return this.batchType;
        }

        public Texture getBatchtexture()
        {
            return batchTex;
        }

        public void setVAO(VertexArrayObject vao)
        {
            this.VAO = vao;
        }

        public bool hasBeenUsedInCurrentTick()
        {
            return requestedVerticesCount > 0 || pointsItterator > 0 || requestedObjectItterator > 0;
        }

        public void deleteVAO()
        {
            VAO.delete();
        }
    }
}
