using Medallion;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class Terrain
    {
        public int genChunksWide { get; private set; }
        public int genChunksRadius{ get; private set; }
        private Dictionary<Vector2i, ChunkColumn> chunkMap = null;
        public List<Chunk> sortedChunks { get; private set; }
        private ChunkComparer sorter = new ChunkComparer();
        private Vector4i chunkRanges;//minX maxX minZ maxZ
        private Random genRand = null;
        public ChunkPopulator populator { get; private set; }
        private Vector3i currentVoxelMiddlePos;
        private Vector3i currentChunkMiddlePos;

        public Terrain(long seed)
        {
            populator = new ChunkPopulator(seed);
            genRand = Rand.CreateJavaRandom(seed);
            chunkMap = new Dictionary<Vector2i, ChunkColumn>();
            sortedChunks = new List<Chunk>();
            currentChunkMiddlePos = currentVoxelMiddlePos = new Vector3i(-99999, -99999, -99999);
            TerrainRenderer.setTerrainToRender(this);
        }

        public void onTick(Vector3 playerPos, float ts)
        {
            foreach (KeyValuePair<Vector2i, ChunkColumn> kvp in chunkMap)
            {
                if(kvp.Value.isMarkedForRemoval)
                {
                    if (deleteChunkColumn(kvp.Value)) break;
                }
            }
            checkChunkGenAreaChanged(playerPos);
        }

        private void checkChunkGenAreaChanged(Vector3 playerPos)
        {
            bool shouldRefreshChunkArea = false;
            bool shouldDoSorting = false;
            int currentChunkGenDistance = (int)(GameSettings.maxDrawDistance.floatValue / Chunk.CHUNK_PHYSICAL_SIZE);
            if (currentChunkGenDistance != genChunksRadius)
            {
                shouldRefreshChunkArea = true;
                genChunksRadius = currentChunkGenDistance;
                genChunksWide = genChunksRadius * 2;
            }
               
            Vector3i distances = MathUtil.manhattanDistances(Chunk.worldToVoxelPos(playerPos), currentVoxelMiddlePos);
            if(distances.X >= Chunk.CHUNK_SIZE || distances.Z >= Chunk.CHUNK_SIZE)
            {
                shouldRefreshChunkArea = true;
                currentVoxelMiddlePos = Chunk.worldToVoxelPos(playerPos);
                currentChunkMiddlePos = Chunk.worldToChunkPos(playerPos);
            }

            if(distances.Y >= Chunk.CHUNK_SIZE)
            {
                shouldDoSorting = true;
                currentVoxelMiddlePos = Chunk.worldToVoxelPos(playerPos);
                currentChunkMiddlePos = Chunk.worldToChunkPos(playerPos);
            }

            if (shouldRefreshChunkArea)
            {
                refreshAllChunkColumns();
            }
            if(shouldDoSorting || shouldRefreshChunkArea)
            {
                Profiler.startTickSection("chunkSort");
                sortedChunks.Sort(sorter.setCenter(currentChunkMiddlePos));
                Profiler.endCurrentTickSection();
            }
        }

        /// <summary>
        /// Will check each chunk column to see if it should be unloaded or if new ones should be loaded.
        /// </summary>
        private void refreshAllChunkColumns()
        {
            Profiler.startTickSection("chunkRefresh");
            chunkRanges.X = currentChunkMiddlePos.X - genChunksRadius - 1;
            chunkRanges.Y = currentChunkMiddlePos.X + genChunksRadius + 1;
            chunkRanges.Z = currentChunkMiddlePos.Z - genChunksRadius - 1;
            chunkRanges.W = currentChunkMiddlePos.Z + genChunksRadius + 1;

            //unload any chunks outside of the radius
            foreach(ChunkColumn c in chunkMap.Values)
            {
                if(Math.Abs(currentChunkMiddlePos.X - c.coord.X) > genChunksRadius || Math.Abs(currentChunkMiddlePos.Z - c.coord.Y) > genChunksRadius)
                {
                    markColumnForRemoval(c);
                }
            }

            Vector2i cPos;
            for(int x = chunkRanges.X; x < chunkRanges.Y; x++)
            {
                cPos.X = x;
                for(int z = chunkRanges.Z; z < chunkRanges.W; z++)
                {
                    cPos.Y = z;
                    if(!chunkMap.TryGetValue(cPos, out ChunkColumn c))
                    {
                        ChunkColumn newChunkColumn = new ChunkColumn(cPos);
                        initChunkColumn(cPos, newChunkColumn);
                        chunkMap.Add(cPos, newChunkColumn);
                        continue;
                    }
                    //if chunkcolumn is on the edge
                    if(cPos.X <= chunkRanges.X+1 || cPos.X >= chunkRanges.Y-1 || cPos.Y <= chunkRanges.Z+1 || cPos.Y >= chunkRanges.W-1)
                    {
                        foreach (Chunk cAt in c.getVerticalChunks()) cAt.isOnWorldEdge = true;
                        continue;
                    }
                    foreach (Chunk cAt in c.getVerticalChunks()) cAt.isOnWorldEdge = false;
                }
            }
            Profiler.endCurrentTickSection();
        }

        /// <summary>
        /// called each frame by terrain renderer before rendering
        /// </summary>
        public void doFrustumCheck(Camera viewer)
        {
            foreach (ChunkColumn c in chunkMap.Values)
            {
                if(c.isInFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), c.columnBounds))
                foreach (Chunk cr in c.getVerticalChunks()) cr.isInFrustum = !WorldFrustum.isBoxNotWithinFrustum(viewer.getCameraWorldFrustum(), cr.chunkBounds);
                else foreach (Chunk cr in c.getVerticalChunks()) cr.isInFrustum = false;
            }
        }

        /// <summary>
        /// returns true if the chunk column is found and removed from the chunk map
        /// </summary>
        private bool deleteChunkColumn(ChunkColumn c)
        {
            //do any chunk saving and stuff here
            //like if chunk is edited save and then remove
            foreach(Chunk cAt in c.getVerticalChunks())
            {
                cAt.localRenderer.delete();
                sortedChunks.Remove(cAt);
            }
            return chunkMap.Remove(c.coord);
        }

        private void markColumnForRemoval(ChunkColumn c)
        {
            foreach (Chunk cAt in c.getVerticalChunks())
            {
                cAt.isMarkedForRemoval = true;
            }
            c.isMarkedForRemoval = true;
        }

        private void initChunkColumn(Vector2i coord, ChunkColumn cc)
        {
            Chunk[] verticalChunks = cc.getVerticalChunks();
            bool isOnEdge = coord.X <= chunkRanges.X + 1 || coord.X >= chunkRanges.Y - 1 || coord.Y <= chunkRanges.Z + 1 || coord.Y >= chunkRanges.W - 1;
            for (int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
            {
                Chunk c = new Chunk(new Vector3i(coord.X, y, coord.Y));
                c.isScheduledForPopulation = true;
                c.isMarkedForRenderUpdate = true;
                c.isOnWorldEdge = isOnEdge;
                sortedChunks.Add(c);
                verticalChunks[y] = c; 
            }
        }

        public Chunk getChunkAtChunkCoords(int x, int y, int z)
        {
            if (y < 0 || y >= ChunkColumn.NUM_CHUNKS_HEIGHT) return null;
            chunkMap.TryGetValue(new Vector2i(x, z), out ChunkColumn c);
            return c != null ? c.getChunkAtYChunkCoord(y) : null;
        }

        public int getLocalVoxelFromChunk(Chunk c, int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNK_SIZE || y < 0 || y >= Chunk.CHUNK_SIZE || z < 0 || z >= Chunk.CHUNK_SIZE)
            {
                int worldY = c.worldCoord.Y + y;
                int inBoundChunkY = worldY >> Chunk.Z_SHIFT;
                if (inBoundChunkY >= ChunkColumn.NUM_CHUNKS_HEIGHT || inBoundChunkY < 0) return 0;
                int worldX = c.worldCoord.X + x;
                int worldZ = c.worldCoord.Z + z;
                int inBoundChunkX = worldX >> Chunk.Z_SHIFT;
                int inBoundChunkZ = worldZ >> Chunk.Z_SHIFT;
                if(chunkMap.TryGetValue(new Vector2i(inBoundChunkX, inBoundChunkZ), out ChunkColumn inBoundChunkColumn))
                {
                    Chunk cAt = inBoundChunkColumn.getChunkAtYChunkCoord(inBoundChunkY);
                    if (cAt.isScheduledForPopulation) populator.populateChunk(cAt);
                    return cAt.getVoxelAt(worldX & Chunk.CHUNK_SIZE_MINUS_ONE, worldY & Chunk.CHUNK_SIZE_MINUS_ONE, worldZ & Chunk.CHUNK_SIZE_MINUS_ONE);
                }
                return 0;
            }
            return c.getVoxelAt(x, y, z);
        }

        public int getLocalLightLevelFromChunk(Chunk c, int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNK_SIZE || y < 0 || y >= Chunk.CHUNK_SIZE || z < 0 || z >= Chunk.CHUNK_SIZE)
            {
                int worldY = c.worldCoord.Y + y;
                int inBoundChunkY = worldY >> Chunk.Z_SHIFT;
                if (inBoundChunkY >= ChunkColumn.NUM_CHUNKS_HEIGHT || inBoundChunkY < 0) return 63;
                int worldX = c.worldCoord.X + x;
                int worldZ = c.worldCoord.Z + z;
                int inBoundChunkX = worldX >> Chunk.Z_SHIFT;
                int inBoundChunkZ = worldZ >> Chunk.Z_SHIFT;
                if (chunkMap.TryGetValue(new Vector2i(inBoundChunkX, inBoundChunkZ), out ChunkColumn inBoundChunkColumn))
                {
                    Chunk cAt = inBoundChunkColumn.getChunkAtYChunkCoord(inBoundChunkY);
                    if (cAt.isScheduledForPopulation) populator.populateChunk(cAt);
                    return cAt.getLightLevelAt(worldX & Chunk.CHUNK_SIZE_MINUS_ONE, worldY & Chunk.CHUNK_SIZE_MINUS_ONE, worldZ & Chunk.CHUNK_SIZE_MINUS_ONE);
                }
                return 63;
            }
            return c.getLightLevelAt(x, y, z);
        }

        public void unLoad()
        {
            foreach (ChunkColumn c in chunkMap.Values) deleteChunkColumn(c);
            
        }
    }
}
