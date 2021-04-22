using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;
namespace RabbetGameEngine
{
    //TODO: Optimize batching and draw calls so each batch has the ideal amount of data
    //TODO: Render chunks front to back
    //TODO: Fustrum cull chunks each frame
    public static class TerrainRenderer
    {
        public static readonly long chunkUpdateTimeLimitNanos = 1000000L;
        private static ChunkRendererSorter distSorter = new ChunkRendererSorter();
        private static Shader voxelShader = null;
        private static Texture terrainTex = null;
        private static Terrain theTerrain = null;
        private static List<ChunkRenderer> chunkrenderers = null;
        private static int chunkDrawCalls = 0;
        private static int tickChunkUpdates = 0;
        private static Vector3i prevSortViewerChunkPos;

        public static void init()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out voxelShader);
            TextureUtil.tryGetTexture("mcterrain", out terrainTex);
            chunkrenderers = new List<ChunkRenderer>(16);
        }

        public static void setTerrainToRender(Terrain t)
        {
            theTerrain = t; 
        }

        public static void addChunkToBeRendered(Chunk c)
        {
            insertChunkRenderer(new ChunkRenderer(c));
        }

        public static void doRenderUpdate(Camera viewer)
        {
            Profiler.startTickSection("chunkUpdates");
            updateAndSortChunkRenderers(viewer);
            Profiler.endCurrentTickSection();
        }

        private static void updateAndSortChunkRenderers(Camera viewer)
        {
            removeAnyUnloadedChunkRenderersAndSort(viewer);
            tickChunkUpdates = 0;
            long startTime = 0;
            long nanosDelta = 0;
            foreach (ChunkRenderer cr in chunkrenderers)
            {
                startTime = TicksAndFrames.nanoTime();

                Profiler.startTickSection("chunkFrustumCheck");
                bool inFrustum = WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cr.boundingBox);
                Profiler.endCurrentTickSection();
                if (inFrustum) continue;

                if (cr.isChunkMarkedForRenderUpdate())
                {
                    cr.updateVoxelMesh(new ChunkCache(theTerrain, cr.pos, 1, false));
                    tickChunkUpdates++;
                }
                nanosDelta += TicksAndFrames.nanoTime() - startTime;
                if (nanosDelta >= chunkUpdateTimeLimitNanos) break;
            }
        }

        private static void removeAnyUnloadedChunkRenderersAndSort(Camera viewer)
        {
            //remove chunk renderers that have been unloaded
            for (int i = 0; i < chunkrenderers.Count; i++)
            {
                ChunkRenderer cr = chunkrenderers.ElementAt(i);
                if (cr.isChunkMarkedForRemoval())
                {
                    cr.delete();
                    chunkrenderers.Remove(cr);
                    i--;
                }
            }
            Profiler.startTickSection("sortChunks");
            Vector3i currentViewerChunkPos;
            if(chunkrenderers.Count > 1 && prevSortViewerChunkPos != (currentViewerChunkPos = Chunk.worldToChunkPos(viewer.getCamPos())))
            {
                chunkrenderers.Sort(distSorter.setCenter(currentViewerChunkPos));
                prevSortViewerChunkPos = currentViewerChunkPos;
            }
            Profiler.endCurrentTickSection();
        }

        private static void insertChunkRenderer(ChunkRenderer cr)
        {
            if(chunkrenderers.Count < 1)
            {
                chunkrenderers.Add(cr);
                return;
            }

            int index = chunkrenderers.BinarySearch(cr, distSorter);
            if (index < 0) index = ~index;
            chunkrenderers.Insert(index, cr);
        }

        public static void renderTerrain(Camera viewer)
        {
            Profiler.startSection("renderChunks");
            chunkDrawCalls = 0;
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            voxelShader.use();
            terrainTex.bind();
            foreach( ChunkRenderer cr in chunkrenderers)
            {
                Profiler.startSection("chunkFrustumCheck");
                bool inFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cr.boundingBox);
                Profiler.endCurrentSection();
                if (inFrustum) renderChunk(cr.pos, cr);
            }
            Profiler.endCurrentSection();
        }

        public static void renderChunk(Vector3i pos, ChunkRenderer c)
        {
            Profiler.startSection("chunkDraw");
            voxelShader.setUniformMat4F("projViewModel", Matrix4.CreateTranslation((Vector3)pos * Chunk.CHUNK_PHYSICAL_SIZE) * Renderer.viewMatrix * Renderer.projMatrix);
            c.bindVAO();
            GL.DrawElements(PrimitiveType.Patches, c.visibleVoxelFaceCount * 4, DrawElementsType.UnsignedInt, 0);
            chunkDrawCalls++;
            Profiler.endCurrentSection();
        }

        public static void unLoad()
        {
            foreach(ChunkRenderer cr in chunkrenderers)
            {
                cr.delete();
            }
            chunkrenderers.Clear();
            theTerrain = null;
        }

        public static void onClosing()
        {
            unLoad();
            //delete any static render objects
        }

        public static int chunkUpdates
        { get => tickChunkUpdates; }

        public static int chunkDraws
        { get => chunkDrawCalls; set => chunkDrawCalls = value; }

    }
}
