using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static String shaderDir = ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        private static String textureDir = ResourceHelper.getTextureFileDir("Camo.png");
        private static String modelPath = ResourceHelper.getOBJFileDir(@"Tank\TankProjectile.obj");
        private Vector3D direction;
        private Vector3D up = new Vector3D(0,1,0);

        public EntityTankProjectileModel(Entity parent, Vector3D direction) : base (parent, shaderDir, textureDir, modelPath)
        {
            this.direction = direction;
        }
    }
}
