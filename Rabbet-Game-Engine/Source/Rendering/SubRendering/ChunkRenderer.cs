using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public static class ChunkRenderer
    {
        private static Shader voxelShader = null;

        static ChunkRenderer()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
        }

        public static void renderAllChunksInWorld(World w)
        {
            Dictionary<Vector3i, Chunk> chunks = w.terrain.chunks;
            foreach (Chunk c in chunks.Values)
                renderChunk(c);
        }

        public static void renderChunk(Chunk c)
        {
            VoxelBatcher vb = c.voxelMesh;
            vb.bindVAO();
            voxelShader.use();
            voxelShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            voxelShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            voxelShader.setUniformMat4F("modelMatrix", Matrix4.CreateTranslation(c.chunkCoord));
            GL.DrawArrays(PrimitiveType.Points, 0, vb.visibleVoxelCount);
        }

    }
}
