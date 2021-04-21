using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class Terrain
    {
        //radius of chunks to initially generate around origin/spawn
        public static readonly int spawnChunkRadius = 8;

        private Dictionary<Vector3i, Chunk> chunkMap = null;
        private Random genRand;

        public Terrain(Random rand)
        {
            genRand = rand;
            chunkMap = new Dictionary<Vector3i, Chunk>();
            TerrainRenderer.setTerrainToRender(this);
        }

        public void onTick(float ts)
        {

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

        public void generateSpawnChunks()
        {
            for (int x = -spawnChunkRadius; x < spawnChunkRadius; x++)
                for (int z = -spawnChunkRadius; z < spawnChunkRadius; z++)
                {
                    Vector3i coord = new Vector3i(x, 0, z);
                    loadChunk(coord, new Chunk(coord));
                }
        }

        private void loadChunk(Vector3i coord, Chunk c)
        {
            debugRandom(c);
            TerrainRenderer.addChunkToBeRendered(c);
            chunkMap.Add(coord, c);
        }

        /// <summary>
        /// returns true if chunk is removed from chunkmap
        /// </summary>
        private bool unLoadChunk(Vector3i coord, Chunk c)
        {
            c.markForRemoval();
            chunkMap.Remove(coord, out Chunk c1);
            return c1 != null && c1 == c;
        }

        public Chunk getChunkAtChunkCoords(int x, int y, int z)
        {
            chunkMap.TryGetValue(new Vector3i(x, y, z), out Chunk c);
            return c;
        }

        public void unLoad()
        {
            TerrainRenderer.unLoad();
        }

        public Dictionary<Vector3i, Chunk> chunks
        {
            get => chunkMap;
        }
    }
}
