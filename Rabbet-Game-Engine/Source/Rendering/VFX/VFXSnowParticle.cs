using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class VFXSnowParticle : VFXSinglePoint
    {
        private Color snowCol;
        public VFXSnowParticle(World w, Vector3 initialPos) : base(w, initialPos, Color.white, 0.05F - (float)GameInstance.rand.NextDouble() * 0.02F, 30.0F, false, true)
        {
            snowCol = Color.white;
            float blueNess = (float)GameInstance.rand.NextDouble() * 0.15F;
            snowCol.r = 0.96F - blueNess;
            snowCol.g = 1.0F - blueNess;
            snowCol.setAlphaF(0.8F - (float)GameInstance.rand.NextDouble() * 0.2F);
            setColorNonLerp(snowCol);
            setRandomHorizontalVelocity(0.005F);
            // setBox(new AABB(new Vector3(-theParticle.radius, -theParticle.radius, -theParticle.radius), new Vector3(theParticle.radius, theParticle.radius, theParticle.radius), this));
            // setYAccel(-gravity * (0.01F - (float)GameInstance.rand.NextDouble() * 0.005F));
        }
    }
}
