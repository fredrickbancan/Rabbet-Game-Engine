using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
namespace RabbetGameEngine
{
    /// <summary>
    /// A class for creating mesh data / render data for rendering a chunk.
    /// Builds buffers of voxel data based on voxel states and visibility for performance.
    /// </summary>
    public class ChunkRenderer
    {
        public static readonly int MAX_CHUNK_FACE_COUNT = 786432;//maciumum voxel faces that can be visible in a chunk
        public static readonly int CHUNK_VERTEX_INDICES_COUNT = MAX_CHUNK_FACE_COUNT * 4;
        public static uint[] VOXEL_INDICES_BUFFER;//0000 1111 2222 3333 4444 5555 6666
        public static readonly Vector3i[] faceDirections = new Vector3i[]
        {
            new Vector3i(1,0,0),
            new Vector3i(0,1,0),
            new Vector3i(0,0,1),
            new Vector3i(-1,0,0),
            new Vector3i(0,-1,0),
            new Vector3i(0,0,-1)
        };
        private static IndexBufferObject VOXEL_VERTEX_IBO;

        public static void init()
        {
            VOXEL_INDICES_BUFFER = new uint[CHUNK_VERTEX_INDICES_COUNT];
            for (uint i = 0; i < CHUNK_VERTEX_INDICES_COUNT; i += 4U)
            {
                for (int j = 0; j < 4; j++) VOXEL_INDICES_BUFFER[i + j] = i / 4U;
            }
            VOXEL_VERTEX_IBO = new IndexBufferObject();
            VOXEL_VERTEX_IBO.initStatic(VOXEL_INDICES_BUFFER);
        }

        public static void onClosing()
        {
            VOXEL_VERTEX_IBO.delete();
        }

        private Chunk parentChunk = null;
        private VertexArrayObject voxelsVAO = null;
        private VoxelFace[] voxelFaceBuffer = null;
        private int addedVoxelFaceCount = 0;

        public ChunkRenderer(Chunk parentChunk)
        {
            this.parentChunk = parentChunk;
            voxelFaceBuffer = new VoxelFace[MAX_CHUNK_FACE_COUNT];
            voxelsVAO = new VertexArrayObject();
            voxelsVAO.beginBuilding();
            voxelsVAO.addBufferDynamic(MAX_CHUNK_FACE_COUNT * VoxelFace.SIZE_IN_BYTES, new VertexBufferLayout());
            GL.EnableVertexAttribArray(0);
            GL.BindVertexBuffer(0, voxelsVAO.getVBOIDAt(0), System.IntPtr.Zero, 4);
        }

        /// <summary>
        /// Updates the voxel buffer based on visible voxels for optimisation.
        /// Should be called whenever the parent chunk's voxels change such as voxels added or removed.
        /// </summary>
        public void updateVoxelMesh()
        {
            addedVoxelFaceCount = 0;
            int i;
            Vector3i pos;
            Vector3i offset;
            VoxelType vt;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                pos.X = x;
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    pos.Z = z;
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        vt = VoxelType.getVoxelById(parentChunk.getVoxelAt(x, y, z));
                        if (vt == null) continue;
                        pos.Y = y;
                        for (i = 0; i < 6; i++)
                        {
                            offset = pos + faceDirections[i];
                            if (!VoxelType.isVoxelOpaque(parentChunk.getVoxelAtSafe(offset)))
                            {
                                voxelFaceBuffer[addedVoxelFaceCount++] = new VoxelFace((byte)x, (byte)y, (byte)z, parentChunk.getLightLevelAtSafe(offset), (byte)i, vt.id);
                            }
                        }
                    }
                }
            }
            voxelsVAO.updateBuffer(0, voxelFaceBuffer, addedVoxelFaceCount * VoxelFace.SIZE_IN_BYTES);
        }

        public void bindVAO()
        {
            voxelsVAO.bind();
            VOXEL_VERTEX_IBO.bind();
        }


        public void deleteVAO()
        {
            voxelsVAO.delete();
        }

        public int visibleVoxelFaceCount
        { get => addedVoxelFaceCount; }
    }
}
