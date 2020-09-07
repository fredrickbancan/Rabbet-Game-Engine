using System;

namespace Coictus.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static String shaderDir = ResourceUtil.getShaderFileDir("ColorTextureFog3D.shader");
        private static String textureDir = ResourceUtil.getTextureFileDir("Camo.png");
        private static String modelPath = ResourceUtil.getOBJFileDir(@"Tank\TankProjectile.obj");

        public EntityTankProjectileModel(Entity parent) : base (parent, shaderDir, textureDir, modelPath)
        {
        }
    }
}
