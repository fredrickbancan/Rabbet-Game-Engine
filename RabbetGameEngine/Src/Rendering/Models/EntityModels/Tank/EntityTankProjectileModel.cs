namespace RabbetGameEngine.Models
{
    public class EntityTankProjectileModel : EntityModel
    {
        private static string textureName = "Camo";
        private static string modelName = "TankProjectile";

        public EntityTankProjectileModel(Entity parent) : base (parent, textureName, MeshUtil.getMeshForModel(modelName))
        {
        }
    }
}
