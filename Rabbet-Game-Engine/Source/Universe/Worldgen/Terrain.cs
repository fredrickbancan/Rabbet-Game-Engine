using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class Terrain
    {
        //radius of chunks to initially generate around origin/spawn
        public static readonly int spawnChunkRadius = 1;

        private Dictionary<Vector3i, Chunk> chunkMap = null;
        private List<Chunk> chunksNeedingUpdates = null;
        private Random genRand;

        public Terrain(Random rand)
        {
            genRand = rand;
            chunkMap = new Dictionary<Vector3i, Chunk>();
            chunksNeedingUpdates = new List<Chunk>();
        }

        public void onFrame(float ptnt)
        {
            updateChunks();
        }
        public void onTick(float ts)
        {

        }

        /// <summary>
        /// updates all chunks needing updates
        /// </summary>
        public void updateChunks()
        {
            foreach(Chunk c in chunksNeedingUpdates)
            {
                c.update();
            }
            chunksNeedingUpdates.Clear();
        }
        /// <summary>
        /// returns voxel type at the provided world coordinates. returns null for air or if coordinate is in null chunk.
        /// </summary>
        public VoxelType getVoxelAtVoxelCoord(int x, int y, int z)
        {
            Chunk c = getChunkAtVoxelCoord(x, y, z);
            return c == null ? null : VoxelType.getVoxelById(c.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE));
        }

        /// <summary>
        /// returns voxel ID at the provided world coordinates. returns 0 for air or if coordinate is in null chunk.
        /// </summary>
        public byte getVoxelIdAtVoxelCoord(int x, int y, int z)
        {
            Chunk c = getChunkAtVoxelCoord(x, y, z);
            return c == null ? (byte)0 : c.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }

        public void setVoxelIdAtVoxelCoord(int x, int y, int z, byte id)
        {
            Chunk c = getChunkAtVoxelCoord(x, y, z);
            if(c != null) 
                c.setVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE, id);
        }

        public byte getLightLevelAtVoxelCoord(int x, int y, int z)
        {
            Chunk c = getChunkAtVoxelCoord(x, y, z);
            return c == null ? (byte)63 : c.getLightLevelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }

        public void setLightLevelAtVoxelCoord(int x, int y, int z, byte l)
        {
            Chunk c = getChunkAtVoxelCoord(x, y, z);
            if(c != null) c.setLightLevelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE, l);
        }

        public Chunk getChunkAtVoxelCoord(int x, int y, int z)
        {
            if (chunkMap.TryGetValue(new Vector3i(x >> 6, y >> 6, z >> 6), out Chunk c))
            {
                return c;
            }
            return null;
        }

        private void debugRandom(Chunk c)
        {
             for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                 for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                     for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                     {
                         if (genRand.Next(2) == 0)
                             c.setVoxelAt(x,y,z,(byte)genRand.Next(1, 5));
                        c.setLightLevelAt(x, y, z, (byte)genRand.Next(64));
                     }
        }
        
        public void generateSpawnChunks(Vector3 spawnPos)
        {
            for (int x = -spawnChunkRadius; x < spawnChunkRadius; x++)
                for (int z = -spawnChunkRadius; z < spawnChunkRadius; z++)
                {
                    Vector3i pos = new Vector3i(x, 0, z);
                    Chunk c = new Chunk(pos, this);
                    debugRandom(c);
                    chunkMap.Add(pos, c);
                }
        }

        public void scheduleUpdateForChunk(Chunk c)
        {
            chunksNeedingUpdates.Add(c);
        }

      

        public void unLoad()
        {
            foreach(Chunk c in chunkMap.Values)
            {
                c.unLoad();
            }
        }

        public Dictionary<Vector3i, Chunk> chunks
        {
            get => chunkMap;
        }
    }
}
