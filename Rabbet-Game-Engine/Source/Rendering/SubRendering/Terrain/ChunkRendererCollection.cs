using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    //TODO: Implement recycling of chunks when moving viewer more than a chunk's worth of distance
    public class ChunkRendererCollection
    {
        /// <summary>
        /// must be kept in order of world position
        /// </summary>
        private ChunkRenderer[] renderers = null;

        /// <summary>
        /// only holds references to the chunk renderers in the first list, used for sorting to mesh and render chunks front to back
        /// </summary>
        private ChunkRenderer[] renderersSorted = null;
        private int rendererCount = 0;
        private int rendererWidthHeight = 0;
        private int renderChunksWide = 0;
        private int renderChunksRadius = 0;
        private int minChunkX = 0;
        private int maxChunkX = 0;
        private int minChunkZ = 0;
        private int maxChunkZ = 0;
        private Terrain theTerrain = null;
        private Vector3i prevSortChunkpos;
        private Vector3i prevRecycleViewerVoxelPos;
        private ChunkRendererSorter distSorter = new ChunkRendererSorter();
        private bool renderersNeedSorting = false;
        private bool hasLoaded = false;

        public void loadRenderers(Terrain t, int renderChunksWide, Camera viewer)
        {
            theTerrain = t;
            this.renderChunksWide = renderChunksWide;
            prevSortChunkpos = Chunk.worldToChunkPos(viewer.getCamPos());
            prevRecycleViewerVoxelPos = Chunk.worldToVoxelPos(viewer.getCamPos());
            rendererCount = renderChunksWide * renderChunksWide * ChunkColumn.NUM_CHUNKS_HEIGHT;
            rendererWidthHeight = renderChunksWide * ChunkColumn.NUM_CHUNKS_HEIGHT;
            renderChunksRadius = renderChunksWide / 2;
            minChunkX = prevSortChunkpos.X - renderChunksRadius;
            maxChunkX = prevSortChunkpos.X + renderChunksRadius;
            minChunkZ = prevSortChunkpos.Z - renderChunksRadius;
            maxChunkX = prevSortChunkpos.Z + renderChunksRadius;
            if (renderers != null) foreach (ChunkRenderer cr in renderers) cr.delete();
            renderers = new ChunkRenderer[rendererCount];
            renderersSorted = new ChunkRenderer[rendererCount];
            for (int x = minChunkX, i = 0; x <= maxChunkX; x++)
                for (int z = minChunkZ; z <= maxChunkX; z++)
                    for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++, i++)
                    {
                        renderersSorted[i] = renderers[i] = new ChunkRenderer(t.getChunkAtChunkCoords(x, y, z));
                    }
            hasLoaded = true;
        }

        public void updateAndSortChunkRenderers(Camera viewer)
        {
            Profiler.startSection("chunkUpdates");
            Profiler.startTickSection("chunkUpdates");
            recycleAndSortRenderersIfNeeded(viewer);
            doFrustumCulling(viewer);
            foreach (ChunkRenderer cr in renderersSorted)
            {
                cr.createVaoIfNeeded();

                if(cr.shouldDoRenderUpdate())
                {
                    cr.doChunkRenderUpdate(new NeighborChunkColumnGroup(theTerrain, cr.parentChunk.coord.X, cr.parentChunk.coord.Y, cr.parentChunk.coord.Z));
                    break;
                }
            }
            Profiler.endCurrentTickSection();
            Profiler.endCurrentSection();
        }

        private void doFrustumCulling(Camera viewer)
        {
            Profiler.startTickSection("chunkFrustumCheck");

            //check column boxes first
            for(int i = 0; i + ChunkColumn.NUM_CHUNKS_HEIGHT < rendererCount; i+= ChunkColumn.NUM_CHUNKS_HEIGHT)
            {
                bool iif = WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), renderers[i].parentColumnBoundingBox);
                for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
                {
                    renderers[i + y].isInFrustum = iif;
                }
            }

            //check each positive testing column's vertical chunk boxes.
            for (int i = 0; i + ChunkColumn.NUM_CHUNKS_HEIGHT < rendererCount; i += ChunkColumn.NUM_CHUNKS_HEIGHT)
            {
                if (!renderers[i].isInFrustum) continue;
                ChunkRenderer crAt;
                for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
                {
                    crAt = renderers[i + y];
                    crAt.isInFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), crAt.boundingBox);
                }
            }

            Profiler.endCurrentTickSection();
        }

        private void recycleAndSortRenderersIfNeeded(Camera viewer)
        {
            Profiler.startTickSection("recycleChunks");
            Vector3i currentViewerChunkPos = Chunk.worldToChunkPos(viewer.getCamPos());
            Vector3i currentViewerVoxelPos = Chunk.worldToVoxelPos(viewer.getCamPos());
            if (MathUtil.manhattanDist(prevRecycleViewerVoxelPos.Xz, currentViewerVoxelPos.Xz)  > Chunk.CHUNK_SIZE)
            {
                minChunkX = currentViewerChunkPos.X - renderChunksRadius;
                maxChunkX = currentViewerChunkPos.X + renderChunksRadius;
                minChunkZ = currentViewerChunkPos.Z - renderChunksRadius;
                maxChunkX = currentViewerChunkPos.Z + renderChunksRadius;
                for (int x = minChunkX, i = 0; x <= maxChunkX; x++)
                    for (int z = minChunkZ; z <= maxChunkX; z++)
                        for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++, i++)
                        {
                            renderers[i].setNewChunkParent(theTerrain.getChunkAtChunkCoords(x, y, z));
                        }
                renderersNeedSorting = true;
                prevRecycleViewerVoxelPos = currentViewerVoxelPos;
            }
            Profiler.endStartTickSection("sortChunks");
            if (renderersNeedSorting || prevSortChunkpos != currentViewerChunkPos)
            {
                Array.Sort(renderersSorted, distSorter.setCenter(currentViewerChunkPos));
                renderersNeedSorting = false;
                prevSortChunkpos = currentViewerChunkPos;
            }
            Profiler.endCurrentTickSection();
        }

        public bool hasInitiallyLoaded()
        {
            return hasLoaded;
        }
        public ChunkRenderer[] getSortedRenderers()
        {
            return renderersSorted;
        }

        public void delete()
        {
            if (renderers != null) foreach (ChunkRenderer cr in renderers) cr.delete();
        }

        
    }
}
