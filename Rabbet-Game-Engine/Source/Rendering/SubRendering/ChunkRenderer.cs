using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public static class ChunkRenderer
    {
        private static Shader voxelShader = null;
        private static int chunkDrawCalls = 0;
        private static Texture terrainTex;
        static ChunkRenderer()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
            voxelShader.use();
            voxelShader.setUniform1I("voxelBuffer", 0);
            TextureUtil.tryGetTexture("mcterrain", out terrainTex);
        }

        public static void renderAllChunksInWorld(World w)
        {
            Profiler.startSection("renderChunks");
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            chunkDrawCalls = 0;
            voxelShader.use();
            voxelShader.setUniform1I("voxelBuffer", 0);
            terrainTex.bind();
            Dictionary<Vector3i, Chunk> chunks = w.terrain.chunks;
            foreach (KeyValuePair<Vector3i, Chunk> kvp in chunks)
            {
                renderChunk(kvp.Key, kvp.Value);
            }
            Profiler.endCurrentSection();
        }

        public static void renderChunk(Vector3i pos, Chunk c)
        {
            voxelShader.setUniformMat4F("projViewModel", Matrix4.CreateTranslation((Vector3)pos * Chunk.CHUNK_PHYSICAL_SIZE)  * Renderer.viewMatrix * Renderer.projMatrix);
            VoxelBatcher vb = c.voxelMesh;
            vb.bindVAO();
            GL.DrawElements(PrimitiveType.Patches, vb.visibleVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
        }

        public static int chunkDraws
        { get => chunkDrawCalls; set => chunkDrawCalls = value; }

    }
}
