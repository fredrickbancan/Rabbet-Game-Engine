using Medallion;
using System;

namespace RabbetGameEngine
{
    public class World
    {
        private Random random;
        private Sky theSky;
        private float grav;

        public World(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            theSky = new Sky(random);
            grav = 9.807F * (0.5F + (float)random.NextDouble() * 1.5F);
            SkyboxRenderer.setSkyboxToDraw(theSky);
        }

        public void onTick(float timeStep)
        {
            theSky.onTick(timeStep);
        }

        public Sky sky
        {
            get =>theSky;
        }

        public float gravity
        {
            get => grav;
        }
    }
}
