using System;

namespace Coictus.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static string shaderDir = ResourceUtil.getShaderFileDir("ColorTextureFog3D.shader");
        private static string textureDir = ResourceUtil.getTextureFileDir("Camo.png");
        private static string modelPath = ResourceUtil.getOBJFileDir(@"Tank\TankProjectile.obj");

        public EntityTankProjectileModel(Entity parent) : base (parent, shaderDir, textureDir, modelPath)
        {
        }
    }
}
