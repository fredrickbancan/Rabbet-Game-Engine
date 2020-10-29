using OpenTK;

namespace RabbetGameEngine
{
    public class Skybox
    {
        public Vector3 horizonColor;
        public Vector3 skyColor;
        private Planet currentPlanet;

        public Skybox(Vector3 skyColor, Planet p)
        {
            this.skyColor = skyColor;
            this.currentPlanet = p;
            this.horizonColor = p.getFogColor();
        }

        public void onTick()
        {
            this.horizonColor = currentPlanet.getFogColor();
        }
    }
}
