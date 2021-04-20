using Medallion;
using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    public class World
    {
        public static readonly int NUM_CHUNKS_HIGH = 16;
        private Random random;
        private Terrain terrain;
        private Sky theSky;
        private float grav;

        public World(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            terrain = new Terrain(random);
            terrain.generateSpawnChunks(new Vector3(0, 0, 0));//temp, no player entity yet
            theSky = new Sky(random);
            grav = 9.807F * (0.5F + (float)random.NextDouble() * 1.5F);
            SkyboxRenderer.setSkyboxToDraw(theSky);
        }
        public void onFrame(float ptnt)
        {
        }
        public void onTick(float timeStep)
        {
            Profiler.startTickSection("tickWorld");
            theSky.onTick(timeStep);
            Profiler.endCurrentTickSection();
        }
        public VoxelType getVoxelAtVoxelCoord(int x, int y, int z)
        {
            return terrain.getVoxelAtVoxelCoord(x, y, z);
        }

        public byte getVoxelIDAtVoxelCoord(int x, int y, int z)
        {
            return terrain.getVoxelIdAtVoxelCoord(x, y, z);
        }

        public void unLoad()
        {
            terrain.unLoad();
        }

        public Sky sky
        {
            get => theSky;
        }

        public float gravity
        {
            get => grav;
        }
    }
}
