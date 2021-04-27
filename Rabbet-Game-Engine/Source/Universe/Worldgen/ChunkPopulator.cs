using Medallion;
using System;

namespace RabbetGameEngine
{
    public class ChunkPopulator
    {
        private Random genRand;

        public ChunkPopulator(long worldSeed)
        {
            genRand = Rand.CreateJavaRandom(worldSeed);
        }

        public void populateChunk(Chunk c)
        {
            debugRandom(c);
        }

        private void debugRandom(Chunk c)
        {
            if (c.coord.Y > 1)
            {
                c.isScheduledForPopulation = false;
                return;
            }
            int chunkYVoxelPos = c.coord.Y * Chunk.CHUNK_SIZE;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        //randomly fill blocks with chance decreasing with altitude.
                       if (genRand.Next((int)Math.Pow(chunkYVoxelPos + y, 2)) == 0)
                            c.setVoxelAt(x, y, z, 37);
                        c.setLightLevelAt(x, y, z, (byte)genRand.Next(64));
                        //c.setLightLevelAt(x, y, z, 63);
                    }
            c.isScheduledForPopulation = false;
        }
    }
}
