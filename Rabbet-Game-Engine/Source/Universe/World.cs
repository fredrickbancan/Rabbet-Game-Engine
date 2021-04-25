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

        private FlyCamera player;//temp
        public World(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            terrain = new Terrain(random);
            theSky = new Sky(random);
            grav = 9.807F * (0.5F + (float)random.NextDouble() * 1.5F);
            SkyboxRenderer.setSkyboxToDraw(theSky);
            player = new FlyCamera(new Vector3(0,128,0));
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
