using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public static class ChunkRenderer
    {
        private static Shader voxelShader = null;
        private static int chunkDrawCalls = 0;
        static ChunkRenderer()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
        }

        public static void renderAllChunksInWorld(World w)
        {
            Profiler.startSection("renderChunks");
            chunkDrawCalls = 0;
            Dictionary<Vector3i, Chunk> chunks = w.terrain.chunks;
            foreach (Chunk c in chunks.Values)
                renderChunk(c);

            Profiler.endCurrentSection();
        }

        public static void renderChunk(Chunk c)
        {
            VoxelBatcher vb = c.voxelMesh;
            vb.bindVAO();
            voxelShader.use();
            voxelShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            voxelShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            voxelShader.setUniformMat4F("modelMatrix", Matrix4.CreateTranslation((Vector3)c.chunkCoord * Chunk.CHUNK_PHYSICAL_SIZE));
            GL.DrawElements(PrimitiveType.Points, vb.visibleVoxelFaceCount * 6, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
        }

        public static int chunkDraws
        { get => chunkDrawCalls; set => chunkDrawCalls = value; }

    }
}
