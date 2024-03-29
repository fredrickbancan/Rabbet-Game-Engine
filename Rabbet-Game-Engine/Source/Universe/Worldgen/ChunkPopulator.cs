﻿using Medallion;
using System;

namespace RabbetGameEngine
{
    public class ChunkPopulator
    {
        private static int valleyLevel = 95;
        private static int peakLevel = 256 - valleyLevel;
        private Random genRand;
        private EarthNoise noise;

        public ChunkPopulator(long worldSeed)
        {
            genRand = Rand.CreateJavaRandom(worldSeed);
            noise = new EarthNoise(worldSeed);
        }

        public void populateChunk(Chunk c)
        {
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        int chunkXVoxelPos = c.coord.X << Chunk.Z_SHIFT;
                        int chunkYVoxelPos = c.coord.Y << Chunk.Z_SHIFT;
                        int chunkZVoxelPos = c.coord.Z << Chunk.Z_SHIFT;
                        double noiseVal = noise.noise(chunkXVoxelPos + x, chunkZVoxelPos + z);
                        float genHillHeight = (int)(noiseVal * peakLevel) + valleyLevel;
                        if (chunkYVoxelPos + y <= genHillHeight)
                            c.setVoxelIDAt(x, y, z, 145);
                        c.setVoxelLightLevelAt(x, y, z, (int)(noiseVal * 63.0D), (int)(noiseVal * 63.0D), (int)(noiseVal * 63.0D));
                    }
                }
            }
            c.isScheduledForPopulation = false;
            c.isMarkedForRenderUpdate = true;
        }
    }
}