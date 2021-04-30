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
        public Sky theSky { get; private set; }
        public float gravity { get; private set; }
        private FlyCamera player;//temp
        public World(long seed)
        {
            player = new FlyCamera(new Vector3(0,128,0));
            random = Rand.CreateJavaRandom(seed);
            terrain = new Terrain(seed);
            theSky = new Sky(random);
            gravity = 9.807F * (0.5F + (float)random.NextDouble() * 1.5F);
            SkyboxRenderer.setSkyboxToDraw(theSky);
            Renderer.setCamera(player);
        }

        public void onFrame(float ptnt)
        {
            player.onFrame(ptnt);
        }

        public void onTick(float timeStep)
        {
            Profiler.startTickSection("tickWorld");
            theSky.onTick(timeStep);
            player.onTick(timeStep);
            terrain.onTick(player.getCamPos(),timeStep);// do last
            Profiler.endCurrentTickSection();
        }

        public void onRenderUpdate()
        {
            Profiler.startTickSection("terrainRenderUpdate");
            terrain.onRenderUpdate();
            Profiler.endCurrentTickSection();
        }

        public void unLoad()
        {
            terrain.unLoad();
        }
    }
}
