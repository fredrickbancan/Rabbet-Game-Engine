using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine
{
    public class Terrain
    {
        //radius of chunks to initially generate around origin/spawn
        public static readonly int spawnChunkRadius = 2;

        private Dictionary<Vector3i, Chunk> chunkMap = null;
        private Random genRand;
        public Terrain(Random rand)
        {
            genRand = rand;
            chunkMap = new Dictionary<Vector3i, Chunk>();
        }

        /// <summary>
        /// returns voxel type at the provided world coordinates. returns null for air or if coordinate is in null chunk.
        /// </summary>
        public VoxelType getVoxelAt(int x, int y, int z)
        {
            Chunk c = getChunkAtCoord(x, y, z);
            return c == null ? null : VoxelType.getVoxelById(c.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE));
        }

        /// <summary>
        /// returns voxel ID at the provided world coordinates. returns 0 for air or if coordinate is in null chunk.
        /// </summary>
        public byte getVoxelIdAt(int x, int y, int z)
        {
            Chunk c = getChunkAtCoord(x, y, z);
            return c == null ? (byte)0 : c.getVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE);
        }

        public void setVoxelIdAt(int x, int y, int z, byte id)
        {
            Chunk c = getChunkAtCoord(x, y, z);
            if(c != null) 
                c.setVoxelAt(x & Chunk.CHUNK_SIZE_MINUS_ONE, y & Chunk.CHUNK_SIZE_MINUS_ONE, z & Chunk.CHUNK_SIZE_MINUS_ONE, id);
        }


        public Chunk getChunkAtCoord(float x, float y, float z)
        {
            if (chunkMap.TryGetValue(getChunkCoord(new Vector3(x,y,z)), out Chunk c))
            {
                return c;
            }
            return null;
        }

        public Chunk getChunkAtCoord(Vector3 pos)
        {
            if(chunkMap.TryGetValue(getChunkCoord(pos), out Chunk c))
            {
                return c;
            }
            return null;
        }

        public Vector3i getChunkCoord(Vector3 pos)
        {
            pos /= Chunk.CHUNK_SIZE;
            return new Vector3i((int)pos.X, (int)pos.Y, (int)(pos.Z));
        }

        private void debugRandom(Chunk c)
        {
             for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                 for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                     for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                     {
                         if (genRand.Next(64)==0)
                             c.setVoxelAt(x,y,z,1);
                        c.setLightLevelAt(x, y, z, (byte)genRand.Next(64));
                     }
        }

        public void generateSpawnChunks(Vector3 spawnPos)
        {
            Vector3i origin = getChunkCoord(spawnPos);
            for (int x = 0; x < spawnChunkRadius; x++)
                for (int z = 0; z < spawnChunkRadius; z++)
                {
                    Chunk c = new Chunk(x, 0, z);
                    debugRandom(c);
                    c.load();
                    chunkMap.Add(new Vector3i(x, 0, z), c);
                }
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
