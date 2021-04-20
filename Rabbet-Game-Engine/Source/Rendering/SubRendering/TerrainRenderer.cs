using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    //TODO: Optimize batching and draw calls so each batch has the ideal amount of data
    //TODO: Render chunks front to back
    //TODO: Fustrum cull chunks each frame
    public static class TerrainRenderer
    {
        private static Shader voxelShader = null;
        private static Texture terrainTex = null;
        private static Terrain theTerrain = null;
        private static List<ChunkRenderer> renderersNeedingUpdates = null;
        private static List<ChunkRenderer> chunkrenderers = null;
        private static Dictionary<Vector3i, Chunk> chunksReference = null;
        private static int chunkDrawCalls = 0;

        public static void init()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
            TextureUtil.tryGetTexture("mcterrain", out terrainTex);
            renderersNeedingUpdates = new List<ChunkRenderer>();
            chunkrenderers = new List<ChunkRenderer>();
        }

        public static void setTerrainToRender(Terrain t)
        {
            theTerrain = t;
            chunksReference = t.chunks;
        }

        public static void renderAllChunksInWorld(World w)
        {
         /*   Profiler.startSection("renderChunks");
            chunkDrawCalls = 0;
            voxelShader.use();
            terrainTex.bind();
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            Dictionary<Vector3i, Chunk> chunks = w.terrain.chunks;
            foreach (KeyValuePair<Vector3i, Chunk> kvp in chunks)
            {
                renderChunk(kvp.Key, kvp.Value);
            }
            Profiler.endCurrentSection();*/
        }

        public static void renderChunk(Vector3i pos, ChunkRenderer c)
        {
            voxelShader.setUniformMat4F("projViewModel", Matrix4.CreateTranslation((Vector3)pos * Chunk.CHUNK_PHYSICAL_SIZE) * Renderer.viewMatrix * Renderer.projMatrix);
            c.bindVAO();
            GL.DrawElements(PrimitiveType.Patches, c.visibleVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
        }

        public static void unLoad()
        {
            foreach(ChunkRenderer cr in chunkrenderers)
            {
                cr.deleteVAO();
            }
            chunkrenderers.Clear();
        }

        public static void onClosing()
        {
            unLoad();
            //delete any static render objects
        }

        public static int chunkDraws
        { get => chunkDrawCalls; set => chunkDrawCalls = value; }

    }
}
