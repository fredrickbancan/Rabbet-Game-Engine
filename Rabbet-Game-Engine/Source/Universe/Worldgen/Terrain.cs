using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class Terrain
    {
        public int genChunksWide
        { get; private set; }

        public Dictionary<Vector2i, ChunkColumn> chunkMap
        { get; private set; }
        private Random genRand;

        public Terrain(Random rand)
        {
            genRand = rand;
            chunkMap = new Dictionary<Vector2i, ChunkColumn>();
            TerrainRenderer.setTerrainToRender(this);
            int currentChunkGenDistance = (int)(GameSettings.maxDrawDistance.floatValue / Chunk.CHUNK_PHYSICAL_SIZE) + 1;// + 1 for camera middle chunk
            if (currentChunkGenDistance != genChunksWide) { genChunksWide = currentChunkGenDistance; onChunkGenDistanceChanged(); }
        }

        public void onTick(float ts)
        {
            if(GameSettings.videoSettingsChanged)
            {
                int currentChunkGenDistance = (int)(GameSettings.maxDrawDistance.floatValue / Chunk.CHUNK_PHYSICAL_SIZE) + 1;// + 1 for camera middle chunk
                if(currentChunkGenDistance != genChunksWide) { genChunksWide = currentChunkGenDistance; onChunkGenDistanceChanged();}
            }
        }

        private void onChunkGenDistanceChanged()
        {
            TerrainRenderer.onChunkGenDistanceChanged();
            //do any other stuff
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
