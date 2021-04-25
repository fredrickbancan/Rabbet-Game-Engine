using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
namespace RabbetGameEngine
{
    public static class TerrainRenderer
    {
        private static ChunkRendererCollection chunkRenderers = null;
        private static Shader voxelShader = null;
        private static Texture terrainTex = null;
        private static Terrain theTerrain = null;
        public static int chunkDrawCalls = 0;
        private static int tickChunkUpdates = 0;
        private static Vector3i prevSortViewerChunkPos;
        private static int renderChunksWide = 0;
        private static bool renderChunksDistChanged = true;
        public static void init()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
            TextureUtil.tryGetTexture("mcterrain", out terrainTex);
            chunkRenderers = new ChunkRendererCollection();
        }

        public static void setTerrainToRender(Terrain t)
        {
            theTerrain = t; 
        }

        public static void loadRenderers(Camera viewer)
        {
            if (theTerrain == null) return;
            renderChunksDistChanged = false;
            renderChunksWide = theTerrain.genChunksWide - 2;// -2 so the outer 2 chunks can be generated first before the furthest chunk renderer is meshed. Important incase the meshing requires the outer 2 chunk data.
            chunkRenderers.loadRenderers(theTerrain, renderChunksWide, viewer);

        }

        public static void onChunkGenDistanceChanged()
        {
            renderChunksDistChanged = true;
        }

        public static void doRenderUpdate(Camera viewer)
        {
            if (chunkRenderers == null) return;
            if (renderChunksDistChanged || !chunkRenderers.hasInitiallyLoaded()) loadRenderers(viewer);
            chunkRenderers.updateAndSortChunkRenderers(viewer);
        }

        public static void renderTerrain(Camera viewer)
        {
            Profiler.startSection("renderChunks");
            chunkDrawCalls = 0;
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            voxelShader.use();
            terrainTex.bind();
            foreach(ChunkRenderer cr in chunkRenderers.getSortedRenderers())
            {
                Profiler.startSection("chunkFrustumCheck");
                if(cr.shouldRender)
                {
                    renderChunk(cr);
                }
                Profiler.endCurrentSection();
            }
            Profiler.endCurrentSection();
        }

        public static void renderChunk(ChunkRenderer c)
        {
            voxelShader.setUniformMat4F("projViewModel", Matrix4.CreateTranslation((Vector3)c.pos * Chunk.CHUNK_PHYSICAL_SIZE) * Renderer.viewMatrix * Renderer.projMatrix);
            c.bindVAO();
            GL.DrawElements(PrimitiveType.Patches, c.visibleVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
        }

        public static void unLoad()
        {
            if (chunkRenderers != null)
            {
                chunkRenderers.delete();
                chunkRenderers = null;
                theTerrain = null;
            }
        }

        public static void onClosing()
        {
            unLoad();
            //delete any static render objects
        }

    }
}
