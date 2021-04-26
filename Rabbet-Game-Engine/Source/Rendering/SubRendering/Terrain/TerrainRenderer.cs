using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RabbetGameEngine
{
    public static class TerrainRenderer
    {
        private static Dictionary<Vector2i, ChunkRendererColumn> rendererMap = null;
        private static List<ChunkRenderer> sortedRenderers = null;
        private static ChunkRendererSorter sorter = new ChunkRendererSorter();
        private static Shader VOXEL_SHADER = null;
        private static Texture TERRAIN_TEX = null;
        private static Terrain TERRAIN_REF = null;
        public static int NUM_DRAWCALLS = 0;

        public static void init()
        {
            ShaderUtil.tryGetShader(ShaderUtil.voxelName, out VOXEL_SHADER);
            TextureUtil.tryGetTexture("mcterrain", out TERRAIN_TEX);
            ChunkRenderer.init();
            rendererMap = new Dictionary<Vector2i, ChunkRendererColumn>(256);
            sortedRenderers = new List<ChunkRenderer>(256);
        }

        public static void setTerrainToRender(Terrain t)
        {
            TERRAIN_REF = t;
        }

        public static void onChunkGenAreaChanged(int chunkGenRadius, Vector4i chunkRanges, Vector3i chunkMiddlePos)
        {
            Profiler.startTickSection("chunkRenRefresh");
            refreshAllChunkRendererColumns(chunkGenRadius, chunkRanges, chunkMiddlePos);
            Profiler.endCurrentTickSection();
        }
        /// <summary>
        /// Will check each chunk column to see if it should be unloaded or if new ones should be loaded.
        /// </summary>
        private static void refreshAllChunkRendererColumns(int chunkGenRadius, Vector4i chunkRanges, Vector3i chunkMiddlePos)
        {
            //unload any chunks outside of the radius
            for (int i = 0; i < rendererMap.Count; i++)
            {
                ChunkRendererColumn c = rendererMap.ElementAt(i).Value;
                if (Math.Abs(chunkMiddlePos.X - c.coord.X) > chunkGenRadius || Math.Abs(chunkMiddlePos.Z - c.coord.Y) > chunkGenRadius)
                {
                    if (deleteChunkRendererColumn(c)) i--;
                }
            }
            Vector2i cPos;
            for (int x = chunkRanges.X; x <=chunkRanges.Y; x++)
            {
                cPos.X = x;
                for (int z = chunkRanges.Z; z < chunkRanges.W; z++)
                {
                    cPos.Y = z;
                    if (!rendererMap.ContainsKey(cPos))
                    {
                        ChunkRendererColumn newChunkColumn = new ChunkRendererColumn(TERRAIN_REF.getChunkColumnAtChunkCoords(x,z), cPos);
                        scheduleChunkRendererColumnForUpdate(newChunkColumn);
                        rendererMap.Add(cPos, newChunkColumn);
                    }
                }
            }
        }

        /// <summary>
        /// returns true if the chunk column is found and removed from the chunk map
        /// </summary>
        private static bool deleteChunkRendererColumn(ChunkRendererColumn c)
        {
            //do any chunk saving and stuff here
            //like if chunk is edited save and then remove
            foreach (ChunkRenderer cAt in c.getVerticalChunkRenderers())
            {
                if (cAt.parentChunk.isMarkedForRenderUpdate)
                {
                    unScheduleScheduledChunkRenderer(cAt);
                }
                cAt.delete();
            }
            return rendererMap.Remove(c.coord);
        }

        private static void unScheduleScheduledChunkRenderer(ChunkRenderer c)
        {
            c.parentChunk.isMarkedForRenderUpdate = false;
            sortedRenderers.Remove(c);
        }

        private static void scheduleChunkRendererColumnForUpdate(ChunkRendererColumn cc)
        {
            ChunkRenderer[] verticalChunks = cc.getVerticalChunkRenderers();
            for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
            {
                scheduleChunkRendererForUpdate(verticalChunks[y]);
            }
        }
        
        private static void scheduleChunkRendererForUpdate(ChunkRenderer c)
        {
            if (sortedRenderers.Count < 2)
            {
                sortedRenderers.Add(c);
                return;
            }

            int index = sortedRenderers.BinarySearch(c, sorter);
            if (index < 0) index = ~index;
            sortedRenderers.Insert(index, c);
        }

        public static void sortRenderersByProximity(Vector3i newMiddleChunkPos)
        {
            if (sortedRenderers.Count < 2) return;
            Profiler.startTickSection("chunkRenSort");
            sortedRenderers.Sort(sorter.setCenter(newMiddleChunkPos));
            Profiler.endCurrentTickSection();
        }

        public static void doRenderUpdate(Camera viewer)
        {
            Profiler.startTickSection("chunkRenderUpdate");
            foreach(ChunkRenderer c in sortedRenderers)
            {
                if (c.parentChunk.isEmpty) continue;
                if (c.isInFrustum && c.parentChunk.isMarkedForRenderUpdate) c.doChunkRenderUpdate(new NeighborChunkColumnGroup(TERRAIN_REF, c.parentChunk.coord));
            }
            Profiler.endCurrentTickSection();
        }

        public static void renderTerrain(Camera viewer)
        {
            Profiler.startSection("frustumCheck");
            doFrustumCheck(viewer);
            Profiler.endStartSection("renderChunks");
            NUM_DRAWCALLS = 0;
            VOXEL_SHADER.use();
            TERRAIN_TEX.bind();
            GL.PatchParameter(PatchParameterInt.PatchVertices, 4);
            Matrix4 projView = viewer.getViewMatrix() * Renderer.projMatrix;
            foreach (ChunkRenderer cr in sortedRenderers)
            {
                if(!cr.parentChunk.isEmpty) cr.renderChunk(VOXEL_SHADER, projView);
            }
            Profiler.endCurrentSection();
        }

        private static void doFrustumCheck(Camera viewer)
        {
            foreach(ChunkRendererColumn c in rendererMap.Values)
            {
                c.isInFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), c.columnBounds);
            }

            foreach (ChunkRendererColumn c in rendererMap.Values)
            {
                if (c.isInFrustum) foreach (ChunkRenderer cr in c.getVerticalChunkRenderers()) cr.isInFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cr.rendererBoundingBox);
            }
        }

        public static void onClosing()
        {
            ChunkRenderer.onClosing();
            if(rendererMap != null)
            {
                foreach (ChunkRenderer c in sortedRenderers)
                    c.delete();
                sortedRenderers.Clear();
            }
            if (rendererMap != null) rendererMap.Clear();
            TERRAIN_REF = null;
        }

        private class ChunkRendererSorter : IComparer<ChunkRenderer>
        {
            private Vector3i centralCoordinate;
            public int Compare([AllowNull] ChunkRenderer a, [AllowNull] ChunkRenderer b)
            {
                //y * 2 for bias towards horizontal priority
                int xDist = MathUtil.manhattanDistHorizontalBias(a.parentChunk.coord, centralCoordinate, 2);
                int yDist = MathUtil.manhattanDistHorizontalBias(b.parentChunk.coord, centralCoordinate, 2);
                return a == null ? 1 : (b == null ? -1 : (xDist > yDist ? 1 : (xDist == yDist ? 0 : -1)));
            }

            public ChunkRendererSorter setCenter(Vector3i cent)
            {
                centralCoordinate = cent;
                return this;
            }
        }
    }
}
