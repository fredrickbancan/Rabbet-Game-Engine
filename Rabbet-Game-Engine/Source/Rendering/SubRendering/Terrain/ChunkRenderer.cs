using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    //TODO: Implement setNewRendererPos() function for recycling renderers when player moves
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class ChunkRenderer
    {
        public static readonly int MAX_CHUNK_FACE_COUNT = 98304;//maximum voxel faces that can be visible in a chunk
        public static readonly int CHUNK_VERTEX_INDICES_COUNT = MAX_CHUNK_FACE_COUNT * 4;
        private static uint[] CHUNK_INDICES_BUFFER;//0000 1111 2222 3333 4444 5555 6666
        private static IndexBufferObject CHUNK_IBO;
        public static readonly Vector3i[] FACE_DIRS = new Vector3i[]
        {
            new Vector3i(1,0,0),
            new Vector3i(0,1,0),
            new Vector3i(0,0,1),
            new Vector3i(-1,0,0),
            new Vector3i(0,-1,0),
            new Vector3i(0,0,-1)
        };

        public static void init()
        {
            CHUNK_IBO = new IndexBufferObject();
            CHUNK_INDICES_BUFFER = new uint[CHUNK_VERTEX_INDICES_COUNT];
            for (uint i = 0; i < CHUNK_VERTEX_INDICES_COUNT; i += 4U)
                for (int j = 0; j < 4; j++) CHUNK_INDICES_BUFFER[i + j] = i / 4U;
            CHUNK_IBO.initStatic(CHUNK_INDICES_BUFFER);
        }

        public static void bindIbo()
        {
            CHUNK_IBO.bind();
        }

        public static void onClosing()
        {
            CHUNK_IBO.delete();
        }

        public Chunk parentChunk
        { get; private set; }

        private VertexArrayObject voxelsVAO = null;
        private VoxelFace[] voxelFaceBuffer = null;
        public AABB rendererBoundingBox { get; private set; }
        private Matrix4 modelMatrix;
        public int addedVoxelFaceCount { get; private set; }
        private bool hasCreatedVao = false;
        public bool isInFrustum = false;

        public ChunkRenderer(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            Vector3i voxelMinBounds = parentChunk.coord * Chunk.CHUNK_SIZE;
            Vector3i voxelMaxBounds = voxelMinBounds + new Vector3i(Chunk.CHUNK_SIZE);
            rendererBoundingBox = AABB.fromBounds((Vector3)voxelMinBounds, (Vector3)voxelMaxBounds);
            modelMatrix = Matrix4.CreateTranslation((Vector3)parentChunk.coord * Chunk.CHUNK_PHYSICAL_SIZE);
        }

        /// <summary>
        /// Updates the voxel buffer based on visible voxels for optimisation.
        /// Should be called whenever the parent chunk's voxels change such as voxels added or removed.
        /// </summary>
        public void doChunkRenderUpdate(NeighborChunkColumnGroup voxelAccess)
        {
            if (!hasCreatedVao)
            {
                if (parentChunk.isEmpty) return;
                createVao();
            }
            Profiler.startTickSection("chunkMeshCreation");
            addedVoxelFaceCount = 0;
            int i;
            Vector3i pos;
            Vector3i offset;
            int vID;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                pos.X = x;
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    pos.Z = z;
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        if ((vID = voxelAccess.getVoxelAtLocalVoxelCoords(x, y, z)) == 0) continue;
                        pos.Y = y;

                        for (i = 0; i < 6; i++)
                        {
                            offset = pos + FACE_DIRS[i];
                            if (!VoxelType.isVoxelOpaque(voxelAccess.getVoxelAtLocalVoxelCoords(offset.X, offset.Y, offset.Z)))
                            {
                                voxelFaceBuffer[addedVoxelFaceCount++] = new VoxelFace(x, y, z, voxelAccess.getLightLevelAtVoxelCoords(offset.X, offset.Y, offset.Z), i, vID);
                            }
                        }
                    }
                }
            }
            Profiler.endStartTickSection("bufferUpdate");
            voxelsVAO.updateBuffer(0, voxelFaceBuffer, addedVoxelFaceCount * VoxelFace.SIZE_IN_BYTES);
            parentChunk.isMarkedForRenderUpdate = false;
            Profiler.endCurrentTickSection();
        }

        private void createVao()
        {
            voxelFaceBuffer = new VoxelFace[MAX_CHUNK_FACE_COUNT];
            voxelsVAO = new VertexArrayObject();
            voxelsVAO.beginBuilding();
            voxelsVAO.addBufferDynamic(MAX_CHUNK_FACE_COUNT * VoxelFace.SIZE_IN_BYTES, new VertexBufferLayout());
            GL.EnableVertexAttribArray(0);
            GL.BindVertexBuffer(0, voxelsVAO.getVBOIDAt(0), IntPtr.Zero, 4);
            hasCreatedVao = true;
        }

        public void renderChunk(Shader VOXEL_SHADER, Matrix4 projView)
        {
            if (!hasCreatedVao) return;
            VOXEL_SHADER.setUniformMat4F("projViewModel", modelMatrix * projView);
            voxelsVAO.bind();
            CHUNK_IBO.bind();
            GL.DrawElements(PrimitiveType.Patches, addedVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            TerrainRenderer.NUM_DRAWCALLS++;
        }

        public void delete()
        {
            if(hasCreatedVao)
            voxelsVAO.delete();
        }
    }
}
