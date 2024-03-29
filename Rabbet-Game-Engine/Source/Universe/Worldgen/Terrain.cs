﻿using Medallion;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    public class Terrain
    {
        public int genChunksWide
        {
            get; private set;
        }

        public int genChunksRadius
        {
            get; private set;
        }

        private Dictionary<Vector2i, ChunkColumn> chunkMap = null;
        private List<Chunk> sortedChunks = null;
        private List<Chunk> chunksNeedingPopulation = null;
        private List<Chunk> chunksToBeMeshed = null;

        public List<Chunk> chunksRenderable
        {
            get; private set;
        }

        private ChunkComparer sorter = new ChunkComparer();
        private Vector4i chunkRanges;//minX maxX minZ maxZ
        private Random genRand = null;

        public ChunkPopulator populator
        {
            get; private set;
        }

        private Vector3i currentVoxelMiddlePos;
        private Vector3i currentChunkMiddlePos;

        public Terrain(long seed)
        {
            populator = new ChunkPopulator(seed);
            genRand = Rand.CreateJavaRandom(seed);
            chunkMap = new Dictionary<Vector2i, ChunkColumn>();
            sortedChunks = new List<Chunk>();
            chunksNeedingPopulation = new List<Chunk>();
            chunksToBeMeshed = new List<Chunk>();
            chunksRenderable = new List<Chunk>();
            currentChunkMiddlePos = currentVoxelMiddlePos = new Vector3i(-99999, -99999, -99999);
            TerrainRenderer.setTerrainToRender(this);
        }

        public void onTick(Vector3 playerPos, float ts)
        {
            foreach (KeyValuePair<Vector2i, ChunkColumn> kvp in chunkMap)
            {
                if (kvp.Value.isMarkedForRemoval)
                {
                    if (deleteChunkColumn(kvp.Value))
                        break;
                }
            }
            checkChunkGenAreaChanged(playerPos);
        }

        public void onRenderUpdate()
        {
            if (chunksNeedingPopulation.Count > 0)
            {
                Chunk c = chunksNeedingPopulation.First();
                populator.populateChunk(c);
                notifyAllChunksNeighbors(c, c.coord);
            }
            if (chunksToBeMeshed.Count > 0)
            {
                Chunk c = chunksToBeMeshed.First();
                c.localRenderer.updateVoxelMesh(this);
                c.localRenderer.updateBuffers();
            }
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
            if (distances.X >= Chunk.CHUNK_SIZE || distances.Z >= Chunk.CHUNK_SIZE)
            {
                shouldRefreshChunkArea = true;
                currentVoxelMiddlePos = Chunk.worldToVoxelPos(playerPos);
                currentChunkMiddlePos = Chunk.worldToChunkPos(playerPos);
            }

            if (distances.Y >= Chunk.CHUNK_SIZE)
            {
                shouldDoSorting = true;
                currentVoxelMiddlePos.Y = Chunk.worldToVoxelPos(playerPos).Y;
                currentChunkMiddlePos.Y = Chunk.worldToChunkPos(playerPos).Y;
            }

            if (shouldRefreshChunkArea)
            {
                refreshAllChunkColumns();
            }
            if (shouldDoSorting || shouldRefreshChunkArea)
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
            foreach (ChunkColumn c in chunkMap.Values)
            {
                if (Math.Abs(currentChunkMiddlePos.X - c.coord.X) > genChunksRadius || Math.Abs(currentChunkMiddlePos.Z - c.coord.Y) > genChunksRadius)
                {
                    markColumnForRemoval(c);
                }
            }

            for (int x = chunkRanges.X; x < chunkRanges.Y; x++)
            {
                for (int z = chunkRanges.Z; z < chunkRanges.W; z++)
                {
                    Vector2i cPos = new Vector2i(x, z);
                    if (!chunkMap.TryGetValue(cPos, out ChunkColumn c))
                    {
                        ChunkColumn newChunkColumn = new ChunkColumn(cPos);
                        initChunkColumn(cPos, newChunkColumn);
                        chunkMap.Add(cPos, newChunkColumn);
                        continue;
                    }
                    //if chunkcolumn is on the edge
                    if (cPos.X <= chunkRanges.X + 1 || cPos.X >= chunkRanges.Y - 1 || cPos.Y <= chunkRanges.Z + 1 || cPos.Y >= chunkRanges.W - 1)
                    {
                        foreach (Chunk cAt in c.getVerticalChunks())
                            cAt.isOnWorldEdge = true;
                        continue;
                    }
                    foreach (Chunk cAt in c.getVerticalChunks())
                        cAt.isOnWorldEdge = false;
                }
            }
            Profiler.endCurrentTickSection();
        }

        /// <summary>
        /// called each frame by terrain renderer before rendering
        /// </summary>
        public void doFrameRenderUpdate(Camera viewer)
        {
            chunksNeedingPopulation.Clear();
            chunksToBeMeshed.Clear();
            chunksRenderable.Clear();
            foreach (Chunk c in sortedChunks)
            {
                if (c.isMarkedForRemoval)
                    continue;
                if (!WorldFrustum.isBoxNotWithinFrustumRef(in viewer.getCameraWorldFrustumRef(), in c.getBoundsRef()))
                {
                    if (c.isScheduledForPopulation)
                    {
                        chunksNeedingPopulation.Add(c);
                        continue;
                    }
                    if (!c.isOnWorldEdge && c.isMarkedForRenderUpdate && c.allNeighborsPopulated)
                        chunksToBeMeshed.Add(c);
                    if (c.localRenderer.addedVoxelFaceCount > 0)
                        chunksRenderable.Add(c);
                }
            }
            return;
        }

        private void notifyAllChunksNeighbors(Chunk c, Vector3i chunkCoord)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3i offset = chunkCoord + ChunkMesh.faceDirections[i];
                Chunk neighbor = getChunkAtChunkCoords(offset.X, offset.Y, offset.Z);
                if (neighbor != null)
                    neighbor.notifyChunkOfNeighborChange(c, (ChunkNeighborDirection)(5 - i));
            }
        }

        /// <summary>
        /// returns true if the chunk column is found and removed from the chunk map
        /// </summary>
        private bool deleteChunkColumn(ChunkColumn c)
        {
            //do any chunk saving and stuff here
            //like if chunk is edited save and then remove
            foreach (Chunk cAt in c.getVerticalChunks())
            {
                notifyAllChunksNeighbors(null, cAt.coord);
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
                Vector3i chunkCoord = new Vector3i(coord.X, y, coord.Y);
                Chunk c = new Chunk(chunkCoord);
                c.isScheduledForPopulation = true;
                c.isOnWorldEdge = isOnEdge;
                sortedChunks.Add(c);
                verticalChunks[y] = c;
                notifyAllChunksNeighbors(c, chunkCoord);
            }
        }

        public Chunk getChunkAtChunkCoords(int x, int y, int z)
        {
            if (y < 0 || y >= ChunkColumn.NUM_CHUNKS_HEIGHT)
                return null;
            chunkMap.TryGetValue(new Vector2i(x, z), out ChunkColumn c);
            return c != null ? c.getChunkAtYChunkCoord(y) : null;
        }

        public int getLocalVoxelFromChunk(Chunk c, int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNK_SIZE || y < 0 || y >= Chunk.CHUNK_SIZE || z < 0 || z >= Chunk.CHUNK_SIZE)
            {
                int worldY = c.worldCoord.Y + y;
                int inBoundChunkY = worldY >> Chunk.Z_SHIFT;
                if (inBoundChunkY >= ChunkColumn.NUM_CHUNKS_HEIGHT || inBoundChunkY < 0)
                    return 0;
                int worldX = c.worldCoord.X + x;
                int worldZ = c.worldCoord.Z + z;
                int inBoundChunkX = worldX >> Chunk.Z_SHIFT;
                int inBoundChunkZ = worldZ >> Chunk.Z_SHIFT;
                if (chunkMap.TryGetValue(new Vector2i(inBoundChunkX, inBoundChunkZ), out ChunkColumn inBoundChunkColumn))
                {
                    Chunk cAt = inBoundChunkColumn.getChunkAtYChunkCoord(inBoundChunkY);
                    if (cAt.isScheduledForPopulation)
                        populator.populateChunk(cAt);
                    return (int)cAt.getVoxelAt(worldX & Chunk.CHUNK_SIZE_MINUS_ONE, worldY & Chunk.CHUNK_SIZE_MINUS_ONE, worldZ & Chunk.CHUNK_SIZE_MINUS_ONE);
                }
                return 0;
            }
            return (int)c.getVoxelAt(x, y, z);
        }

        public Vector3i getLocalVoxelLightLevelFromChunk(Chunk c, int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNK_SIZE || y < 0 || y >= Chunk.CHUNK_SIZE || z < 0 || z >= Chunk.CHUNK_SIZE)
            {
                int worldY = c.worldCoord.Y + y;
                int inBoundChunkY = worldY >> Chunk.Z_SHIFT;
                if (inBoundChunkY >= ChunkColumn.NUM_CHUNKS_HEIGHT || inBoundChunkY < 0)
                    return Vector3i.Zero;
                int worldX = c.worldCoord.X + x;
                int worldZ = c.worldCoord.Z + z;
                int inBoundChunkX = worldX >> Chunk.Z_SHIFT;
                int inBoundChunkZ = worldZ >> Chunk.Z_SHIFT;
                if (chunkMap.TryGetValue(new Vector2i(inBoundChunkX, inBoundChunkZ), out ChunkColumn inBoundChunkColumn))
                {
                    Chunk cAt = inBoundChunkColumn.getChunkAtYChunkCoord(inBoundChunkY);
                    if (cAt.isScheduledForPopulation)
                        populator.populateChunk(cAt);
                    return cAt.getVoxelLightLevelAt(worldX & Chunk.CHUNK_SIZE_MINUS_ONE, worldY & Chunk.CHUNK_SIZE_MINUS_ONE, worldZ & Chunk.CHUNK_SIZE_MINUS_ONE);
                }
                return Vector3i.Zero;
            }
            return c.getVoxelLightLevelAt(x, y, z);
        }

        public void unLoad()
        {
            foreach (ChunkColumn c in chunkMap.Values)
                deleteChunkColumn(c);
        }
    }
}