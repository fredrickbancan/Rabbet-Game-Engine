using OpenTK;
using RabbetGameEngine.SubRendering;
using System;
namespace RabbetGameEngine.VFX
{
    public class VFXExplosion : VFXBase
    {
        protected static readonly string shaderName = "EntityWorld_F";
        protected static readonly string textureName = "Explosion";
        protected static readonly string modelname = "IcoSphere.obj";
        private static Random rand = new Random();

        [Obsolete("This vfx was used for explosions before the addition of point particle vfx")]
        public VFXExplosion(Vector3 pos) : base(pos, 1.0F, shaderName, textureName, modelname, 0.5F, BatchType.triangles)
        {
            scale = 3.0F;
            setPitch((float)(rand.NextDouble() * 180D));
            setYaw((float)(rand.NextDouble() * 180D));
            setRoll((float)(rand.NextDouble() * 180D));
        }

    }
}
