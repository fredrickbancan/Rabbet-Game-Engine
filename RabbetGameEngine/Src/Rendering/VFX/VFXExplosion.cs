using OpenTK;
using System;
namespace RabbetGameEngine.VFX
{
    public class VFXExplosion : VFXBase
    {
        protected static readonly string shaderName = "ColorTextureFog3D.shader";
        protected static readonly string textureName = "Explosion.png";
        protected static readonly string modelname = "IcoSphere.obj";
        private static Random rand = new Random();

        [Obsolete("This vfx was used for explosions before the addition of point particle vfx")]
        public VFXExplosion(Vector3 pos) : base(pos, 1.0F, shaderName, textureName, modelname, 0.5F, VFXRenderType.tirangles)
        {
            scale = 3.0F;
            setPitch((float)(rand.NextDouble() * 180D));
            setYaw((float)(rand.NextDouble() * 180D));
            setRoll((float)(rand.NextDouble() * 180D));
        }

    }
}
