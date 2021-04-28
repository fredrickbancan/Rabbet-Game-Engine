using Medallion;
using NoiseTest;
using System;

namespace RabbetGameEngine
{
    public class ChunkPopulator
    {
        private static readonly double noiseScale = 0.02D;
        private Random genRand;
        private OpenSimplexNoise noise;
        public ChunkPopulator(long worldSeed)
        {
            genRand = Rand.CreateJavaRandom(worldSeed);
            noise = new OpenSimplexNoise(worldSeed);
        }

        public void populateChunk(Chunk c)
        {
            debugRandom(c);
        }

        private void debugRandom(Chunk c)
        {
            int chunkYVoxelPos = c.coord.Y << Chunk.Z_SHIFT;
            double chunkXNoisePos = (c.coord.X << Chunk.Z_SHIFT) * noiseScale;
            double chunkYNoisePos = (c.coord.Y << Chunk.Z_SHIFT) * noiseScale;
            double chunkZNoisePos = (c.coord.Z << Chunk.Z_SHIFT) * noiseScale;
            double offsetX;
            double offsetY;
            double offsetZ;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                offsetX = chunkXNoisePos + x * noiseScale;
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    offsetZ = chunkZNoisePos + z * noiseScale;
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        offsetY = chunkYNoisePos + y * noiseScale;
                        //randomly fill blocks with chance decreasing with altitude.
                        if (noise.Evaluate(offsetZ, offsetX) < -0.0)
                            c.setVoxelAt(x, y, z, 4);
                        c.setLightLevelAt(x, y, z, (int)(((float)(chunkYVoxelPos+y) / 256.0F) * 63.0F));
                        //c.setLightLevelAt(x, y, z, y);
                    }
                }
            }
            c.isScheduledForPopulation = false;
        }
    }
}
