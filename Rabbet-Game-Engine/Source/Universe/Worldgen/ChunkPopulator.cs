using Medallion;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RabbetGameEngine
{
    public class ChunkPopulator
    {
        private List<Chunk> chunkUpdateQueue;
        private Random genRand;
        private PopulationSorter sorter = new PopulationSorter();

        public ChunkPopulator(long worldSeed)
        {
            chunkUpdateQueue = new List<Chunk>();
            genRand = Rand.CreateJavaRandom(worldSeed);
        }

        public void scheduleChunkForPopulating(Chunk c)
        {
            if (chunkUpdateQueue.Count < 2)
            {
                chunkUpdateQueue.Add(c);
                return;
            }

            int index = chunkUpdateQueue.BinarySearch(c, sorter);
            if (index < 0) index = ~index;
            chunkUpdateQueue.Insert(index, c);
        }

        public void unScheduleScheduledChunk(Chunk c)
        {
            c.isScheduledForPopulation = false;
            chunkUpdateQueue.Remove(c);
        }

        public void sortQueue(Vector3i newCentralChunkCoord)
        {
            if (chunkUpdateQueue.Count < 2) return;
            Profiler.startTickSection("populatorSort");
            sorter.setCenter(newCentralChunkCoord);
            chunkUpdateQueue.Sort(sorter);
            Profiler.endCurrentTickSection();
        }

        /// <summary>
        /// populate the first chunk in queue and remove it.
        /// Allows for a more smooth performance doing this just once per tick instead of all of them at once.
        /// </summary>
        public void onTick(float ts)
        {
            if(chunkUpdateQueue.Count > 0)
            {
                debugRandom(chunkUpdateQueue.Last());
                chunkUpdateQueue.RemoveAt(chunkUpdateQueue.Count-1);
            }
        }
        private void debugRandom(Chunk c)
        {
            c.init();
            int chunkYVoxelPos = c.coord.Y * Chunk.CHUNK_SIZE;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        //randomly fill blocks with chance decreasing with altitude.
                      //  if (genRand.Next((chunkYVoxelPos + y) * 8) == 0)
                            c.setVoxelAt(x, y, z, (byte)genRand.Next(1, VoxelType.numVoxels + 1));
                        c.setLightLevelAt(x, y, z, (byte)genRand.Next(64));
                    }
            c.isScheduledForPopulation = false;
            c.isMarkedForRenderUpdate = true;
        }

        /// <summary>
        /// Sorts chunks in collection from furthest to closest with xz bias
        /// </summary>
        private class PopulationSorter : IComparer<Chunk>
        {
            private Vector3i centralCoordinate;
            public int Compare([AllowNull] Chunk a, [AllowNull] Chunk b)
            {
                //y * 2 for bias towards horizontal priority
                int xDist = MathUtil.manhattanDistHorizontalBias(a.coord, centralCoordinate, 2);
                int yDist = MathUtil.manhattanDistHorizontalBias(b.coord, centralCoordinate, 2);
                return a == null ? -1 : (b == null ? 1 : (xDist > yDist ? -1 : (xDist == yDist ? 0 : 1)));
            }
            public void setCenter(Vector3i cent)
            {
                centralCoordinate = cent;
            }
        }
    }
}
