using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    public class VFXExplosion : VFX
    {
        protected static readonly String shaderDir = ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        protected static readonly String textureDir = ResourceHelper.getTextureFileDir("Explosion.png");
        protected static readonly String modelDir = ResourceHelper.getOBJFileDir("IcoSphere.obj");
        private static Random rand = new Random();
        public VFXExplosion(Vector3D pos) : base(pos, 0.0F, shaderDir, textureDir, modelDir, 0.5D)
        {
            scale = 3.0F;
            setPitch((float)rand.NextDouble() * 180D);
            setYaw((float)rand.NextDouble() * 180D);
            setRoll((float)rand.NextDouble() * 180D);
        }

    }
}
