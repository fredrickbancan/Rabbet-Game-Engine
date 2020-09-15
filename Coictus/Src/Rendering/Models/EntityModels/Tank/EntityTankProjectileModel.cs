namespace Coictus.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static string shaderName = "ColorTextureFog3D.shader";
        private static string textureName = "Camo.png";
        private static string modelName = @"Tank\TankProjectile.obj";

        public EntityTankProjectileModel(Entity parent) : base (parent, shaderName, textureName, modelName)
        {
        }
    }
}
