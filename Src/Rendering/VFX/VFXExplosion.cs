using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;
namespace RabbetGameEngine.VisualEffects
{
    public class VFXExplosion : VFX
    {
        protected static readonly string textureName = "Explosion";
        protected static readonly string modelname = "IcoSphere";
        private static Random rand = new Random();

        [Obsolete("This vfx was used for explosions before the addition of point particle vfx")]
        public VFXExplosion(Vector3 pos) : base(pos, 1.0F, textureName, MeshUtil.getMeshForModel(modelname), 0.5F)
        {
            scale = 3.0F;
            setPitch((float)(rand.NextDouble() * 180D));
            setYaw((float)(rand.NextDouble() * 180D));
            setRoll((float)(rand.NextDouble() * 180D));
        }

    }
}
