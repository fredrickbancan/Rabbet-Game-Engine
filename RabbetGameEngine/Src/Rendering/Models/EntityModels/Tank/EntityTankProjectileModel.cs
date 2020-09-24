namespace RabbetGameEngine.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static string shaderName = "EntityWorld_F";
        private static string textureName = "Camo";
        private static string modelName = @"Tank\TankProjectile.obj";

        public EntityTankProjectileModel(Entity parent) : base (parent, shaderName, textureName, modelName)
        {
        }
    }
}
