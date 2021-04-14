using Medallion;
using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    public class World
    {
        public static readonly int NUM_CHUNKS_HIGH = 16;
        private Random random;
        private Terrain chunks;
        private Sky theSky;
        private float grav;

        public World(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            chunks = new Terrain(random);
            chunks.generateSpawnChunks(new Vector3(0,0,0));//temp, no player entity yet
            theSky = new Sky(random);
            grav = 9.807F * (0.5F + (float)random.NextDouble() * 1.5F);
            SkyboxRenderer.setSkyboxToDraw(theSky);
        }

        public void onTick(float timeStep)
        {
            Profiler.startTickSection("tickWorld");
            theSky.onTick(timeStep);

            Profiler.endCurrentTickSection();
        }
        public VoxelType getBlockAt(int x, int y, int z)
        {
            return chunks.getVoxelAt(x, y, z);
        }

        public void unLoad()
        {
            chunks.unLoad();
        }

        public Sky sky
        {
            get =>theSky;
        }

        public float gravity
        {
            get => grav;
        }

        public Terrain terrain
        { get => chunks; }
    }
}
