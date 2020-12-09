using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine.SubRendering
{
    //TODO: replace look at camera function in iSphere shaders with new faster one. (found in mathutil and in moons.shader)
    public class Batch
    { 
        public static readonly int vectorSize = sizeof(float) * 3;
        public static readonly int matrixSize = sizeof(float) * 16;
        public static readonly int initialArraySize = 32;
        public static readonly int baseMaxBufferSizeBytes = 8388608;
        public int maxBufferSizeBytes = baseMaxBufferSizeBytes;
        public int maxIndiciesCount;
        public int maxVertexCount;
        public int maxPositionCount;
        public int maxPointCount;
        public int maxSprite3DCount;
        public int maxMatrixCount;
        public int maxDrawCommandCount;

        /// <summary>
        /// true if this batch requires transparency sorting
        /// </summary>
        public bool requiresSorting = false;

        public bool hasBeenUsed = false;

        public VertexArrayObject VAO;
        public Vertex[] vertices;
        public uint[] indices;

        public PointParticle[] batchedPoints = null;

        public Matrix4[] modelMatrices = null;

        public Vector3[] positions = null;

        public Vector3[] scales = null;

        public Sprite3D[] sprites3D = null;

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

        public int spriteItterator = 0;

        /// <summary>
        /// number of vertices requested to be added to this batch since the last update.
        /// </summary>
        public int requestedVerticesCount = 0;

        /// <summary>
        /// number of indices requested to be added to this batch since the last update
        /// </summary>
        public int requestedIndicesCount = 0;

        private RenderType batchType;
        public Texture batchTex;
        public Shader batchShader;

        public int renderLayer = 0;

        public Batch(RenderType type, Texture tex, int renderLayer = 0)
        {
            this.renderLayer = renderLayer;
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

        public void configureDrawCommandsForCurrentObject(int objIndCount, bool quads)
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

            if(quads)
            drawCommands[requestedObjectItterator] = new DrawCommand((uint)(objIndCount), (uint)(1), (uint)(0), (uint)(requestedVerticesCount), (uint)(requestedObjectItterator));
            else
            drawCommands[requestedObjectItterator] = new DrawCommand((uint)(objIndCount), (uint)(1), (uint)(requestedIndicesCount), (uint)(requestedVerticesCount), (uint)(requestedObjectItterator));
        }

        public void reset()
        {
            requestedVerticesCount = 0;
            requestedIndicesCount = 0;
            requestedObjectItterator = 0;
            matricesItterator = 0;
            pointsItterator = 0;
            positionItterator = 0;
            spriteItterator = 0;
        }

        public void postRenderUpdate()
        {
            BatchUtil.updateBuffers(this);
            hasBeenUsed = false;
        }
        
        public void draw( Matrix4 viewMatrix, Vector3 fogColor)
        {
            BatchUtil.drawBatch(this, viewMatrix, fogColor);
        }

        public void bindVAO()
        {
            VAO.bind();
        }

        public RenderType getRenderType()
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

        public bool hasBeenUsedSinceLastUpdate()
        {
            return hasBeenUsed;
        }

        public void deleteVAO()
        {
            VAO.delete();
        }
    }
}
