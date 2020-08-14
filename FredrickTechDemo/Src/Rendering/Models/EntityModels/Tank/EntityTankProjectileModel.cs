using System;

namespace FredrickTechDemo.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static String shaderDir = ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        private static String textureDir = ResourceHelper.getTextureFileDir("Camo.png");
        private static String modelPath = ResourceHelper.getOBJFileDir(@"Tank\TankProjectile.obj");

        public EntityTankProjectileModel(Entity parent) : base (parent, shaderDir, textureDir, modelPath)
        {
        }
    }
}
