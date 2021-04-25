using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    public class Terrain
    {
        public int genChunksWide
        { get; private set; }

        public int genChunksRadius
        { get; private set; }

        private Dictionary<Vector2i, ChunkColumn> chunkMap;

        private int minChunkX;
        private int minChunkZ;
        private int maxChunkX;
        private int maxChunkZ;
        private Random genRand;
        private Vector3i currentChunkMiddlePos;

        public Terrain(Random rand)
        {
            genRand = rand;
            chunkMap = new Dictionary<Vector2i, ChunkColumn>();
            TerrainRenderer.setTerrainToRender(this);
            genChunksWide = (int)(GameSettings.maxDrawDistance.floatValue / Chunk.CHUNK_PHYSICAL_SIZE) + 1;// + 1 for camera middle chunk
            genChunksRadius = genChunksWide / 2;
            onChunkGenDistanceChanged(); 
        }

        public void onTick(Vector3 playerPos, float ts)
        {
            if(GameSettings.videoSettingsChanged)
            {
                int currentChunkGenDistance = (int)(GameSettings.maxDrawDistance.floatValue / Chunk.CHUNK_PHYSICAL_SIZE) + 1;// + 1 for camera middle chunk
                if(currentChunkGenDistance != genChunksWide) { genChunksWide = currentChunkGenDistance; onChunkGenDistanceChanged();}
            }

            if(MathUtil.manhattanDist(Chunk.worldToVoxelPos(playerPos).Xz, currentChunkMiddlePos.Xz) > Chunk.CHUNK_SIZE)
            {
                currentChunkMiddlePos = Chunk.worldToChunkPos(playerPos);
                refreshAllChunkColumns();
            }
        }

        private void onChunkGenDistanceChanged()
        {
            refreshAllChunkColumns();
            TerrainRenderer.onChunkGenDistanceChanged();
        }

        /// <summary>
        /// Will check each chunk column to see if it should be unloaded or if new ones should be loaded.
        /// </summary>
        private void refreshAllChunkColumns()
        {
            minChunkX = currentChunkMiddlePos.X - genChunksRadius;
            maxChunkX = currentChunkMiddlePos.X + genChunksRadius;
            minChunkZ = currentChunkMiddlePos.Z - genChunksRadius;
            maxChunkZ = currentChunkMiddlePos.Z + genChunksRadius;

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
            for(int x = minChunkX; x <= maxChunkX; x++)
            {
                cPos.X = x;
                for(int z = minChunkZ; z <= maxChunkZ; z++)
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
        }

        /// <summary>
        /// returns true if the chunk column is found and removed from the chunk map
        /// </summary>
        private bool unloadChunkColumn(ChunkColumn c)
        {
            //do any chunk saving and stuff here
            //like if chunk is edited save and then remove

            return chunkMap.Remove(c.coord);
        }

        private void populateChunkColumn(Vector2i coord, ChunkColumn cc)
        {
            Chunk[] verticalChunks = cc.getVerticalChunks();
            for(int y = 0; y < ChunkColumn.NUM_CHUNKS_HEIGHT; y++)
            {
                Chunk c = new Chunk(new Vector3i(coord.X, y, coord.Y), cc);
                debugRandom(c);
                verticalChunks[y] = c; 
            }
        }

        private void debugRandom(Chunk c)
        {
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        if (genRand.Next(2) == 0)
                            c.setVoxelAt(x, y, z, (byte)genRand.Next(1, VoxelType.numVoxels + 1));
                        c.setLightLevelAt(x, y, z, (byte)genRand.Next(64));
                    }
            c.markForRenderUpdate();
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
            TerrainRenderer.unLoad();
        }
    }
}
