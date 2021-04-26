using Medallion;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    public class Terrain
    {
        public int genChunksWide { get; private set; }
        public int genChunksRadius{ get; private set; }
        private Dictionary<Vector2i, ChunkColumn> chunkMap = null;
        private Vector4i chunkRanges;//minX maxX minZ maxZ
        private Random genRand = null;
        private ChunkPopulator populator = null;
        private Vector3i currentVoxelMiddlePos;
        private Vector3i currentChunkMiddlePos;

        public Terrain(long seed)
        {
            populator = new ChunkPopulator(seed);
            genRand = Rand.CreateJavaRandom(seed);
            chunkMap = new Dictionary<Vector2i, ChunkColumn>();
            currentChunkMiddlePos = currentVoxelMiddlePos = new Vector3i(-99999, -99999, -99999);
            TerrainRenderer.setTerrainToRender(this);
        }

        public void onTick(Vector3 playerPos, float ts)
        {
            checkChunkGenAreaChanged(playerPos);
            populator.onTick(ts);
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
            if (MathUtil.manhattanDist(Chunk.worldToVoxelPos(playerPos), currentVoxelMiddlePos, out Vector3i distances) >= Chunk.CHUNK_SIZE)
            {
                currentVoxelMiddlePos = Chunk.worldToVoxelPos(playerPos);
                currentChunkMiddlePos = Chunk.worldToChunkPos(playerPos);
                populator.sortQueue(currentChunkMiddlePos);
                shouldRefreshChunkArea = (distances.X >= Chunk.CHUNK_SIZE || distances.Z >= Chunk.CHUNK_SIZE);
                shouldDoSorting = shouldRefreshChunkArea || distances.Y > Chunk.CHUNK_SIZE;
            }
            if(shouldRefreshChunkArea)
            {
                refreshAllChunkColumns();
                TerrainRenderer.onChunkGenAreaChanged(genChunksRadius, chunkRanges, currentChunkMiddlePos);
                //generation range should be greater than rendering range for population reasons
            }
            if(shouldDoSorting)
            {
                populator.sortQueue(currentChunkMiddlePos);
                TerrainRenderer.sortRenderersByProximity(currentChunkMiddlePos);
            }
        }

        /// <summary>
        /// Will check each chunk column to see if it should be unloaded or if new ones should be loaded.
        /// </summary>
        private void refreshAllChunkColumns()
        {
            Profiler.startTickSection("chunkRefresh");
            chunkRanges.X = currentChunkMiddlePos.X - genChunksRadius;
            chunkRanges.Y = currentChunkMiddlePos.X + genChunksRadius;
            chunkRanges.Z = currentChunkMiddlePos.Z - genChunksRadius;
            chunkRanges.W = currentChunkMiddlePos.Z + genChunksRadius;

            //unload any chunks outside of the radius
            for(int i = 0; i < chunkMap.Count; i++)
            {
                ChunkColumn c = chunkMap.ElementAt(i).Value;
                if(Math.Abs(currentChunkMiddlePos.X - c.coord.X) > genChunksRadius || Math.Abs(currentChunkMiddlePos.Z - c.coord.Y) > genChunksRadius)
                {
                    if (unloadChunkColumn(c)) i--;
                }
            }
            Vector2i cPos;
            for(int x = chunkRanges.X - 2; x < chunkRanges.Y + 2; x++)
            {
                cPos.X = x;
                for(int z = chunkRanges.Z - 2; z < chunkRanges.W + 2; z++)
                {
                    cPos.Y = z;
                    if(!chunkMap.ContainsKey(cPos))
                    {
                        ChunkColumn newChunkColumn = new ChunkColumn(cPos);
                        populateChunkColumn(cPos, newChunkColumn);
                        chunkMap.Add(cPos, newChunkColumn);
                    }
                }
            }
            Profiler.endCurrentTickSection();
        }

        /// <summary>
        /// returns true if the chunk column is found and removed from the chunk map
        /// </summary>
        private bool unloadChunkColumn(ChunkColumn c)
        {
            //do any chunk saving and stuff here
            //like if chunk is edited save and then remove
            foreach(Chunk cAt in c.getVerticalChunks())
            {
                if(cAt.isScheduledForPopulation)
                {
                    populator.unScheduleScheduledChunk(cAt);
                }
            }
            return chunkMap.Remove(c.coord);
        }

        private void populateChunkColumn(Vector2i coord, ChunkColumn cc)
        {
            Chunk[] verticalChunks = cc.getVerticalChunks();
            for(int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
            {
                Chunk c = new Chunk(new Vector3i(coord.X, y, coord.Y));
                c.isScheduledForPopulation = true;
                populator.scheduleChunkForPopulating(c);
                verticalChunks[y] = c; 
            }
        }

        public Chunk getChunkAtChunkCoords(int x, int y, int z)
        {
            chunkMap.TryGetValue(new Vector2i(x, z), out ChunkColumn c);
            return c != null ? c.getChunkAtYChunkCoord(y) : null;
        }

        public ChunkColumn getChunkColumnAtChunkCoords(int x, int z)
        {
            chunkMap.TryGetValue(new Vector2i(x, z), out ChunkColumn c);
            return c;
        }

        public void unLoad()
        {

        }
    }
}
