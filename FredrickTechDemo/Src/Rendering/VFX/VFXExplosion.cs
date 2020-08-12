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
        public VFXExplosion(Vector3D pos) : base(pos, 0.0F, shaderDir, textureDir, modelDir, 5.5D)
        {
            scale = 3.0F;
            rotation.x = (float)rand.NextDouble() * 360;
            rotation.y = (float)rand.NextDouble() * 360;
            rotation.z = (float)rand.NextDouble() * 360;
        }

    }
}
