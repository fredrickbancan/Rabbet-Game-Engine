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
        private static Shader voxelShader = null;
        private static Texture terrainTex = null;
        private static Terrain theTerrain = null;
        private static List<ChunkRenderer> renderersNeedingUpdates = null;
        private static List<ChunkRenderer> chunkrenderers = null;
        private static int chunkDrawCalls = 0;
        private static int tickChunkUpdates = 0;

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
        }

        public static void addChunkToBeRendered(Chunk c)
        {
            chunkrenderers.Add(new ChunkRenderer(c));
        }

        public static void doRenderUpdate(Camera viewer)
        {
            Profiler.startTickSection("chunkUpdates");
            updateAndSortChunkRenderers(viewer);
            Profiler.endCurrentTickSection();
        }

        private static void updateAndSortChunkRenderers(Camera viewer)
        {
            Profiler.startTickSection("checkAndSortChunks");
            //collect or remove chunk renderers that need updates
            for(int i = 0; i < chunkrenderers.Count; i++)
            {
                ChunkRenderer cr = chunkrenderers.ElementAt(i);
                if(cr.isChunkMarkedForRemoval())
                {
                    cr.delete();
                    chunkrenderers.Remove(cr);
                    renderersNeedingUpdates.Remove(cr);
                    i--;
                    continue;
                }

                if (cr.isChunkMarkedForRenderUpdate() && !renderersNeedingUpdates.Contains(cr)) renderersNeedingUpdates.Add(cr);
            }

            Profiler.endStartTickSection("chunkMeshing");
            tickChunkUpdates = 0;
            long startTime = 0;
            long nanosDelta = 0;
            for(int i = 0; i < renderersNeedingUpdates.Count && nanosDelta < chunkUpdateTimeLimitNanos; i++)
            {
                startTime = TicksAndFrames.nanoTime();
                ChunkRenderer cm = renderersNeedingUpdates.ElementAt(i);

                Profiler.startTickSection("chunkFrustumCheck");
                if (WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cm.boundingBox))
                {
                    Profiler.endCurrentTickSection();
                    continue;
                }
                Profiler.endCurrentTickSection();

                cm.updateVoxelMesh(new ChunkCache(theTerrain, cm.pos, 1, false));
                renderersNeedingUpdates.RemoveAt(i--);
                tickChunkUpdates++;
                nanosDelta += TicksAndFrames.nanoTime() - startTime;
            }
            Profiler.endCurrentTickSection();
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
                if (!WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cr.boundingBox))
                {
                    Profiler.endCurrentSection();
                    renderChunk(cr.pos, cr);
                }
                else
                {
                    Profiler.endCurrentSection();
                }
            }
            Profiler.endCurrentSection();
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
                cr.delete();
            }
            chunkrenderers.Clear();
            renderersNeedingUpdates.Clear();
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
