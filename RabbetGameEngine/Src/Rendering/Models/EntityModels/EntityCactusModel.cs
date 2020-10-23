namespace RabbetGameEngine.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static string textureName = "EntityCactus";
        private static string modelName = "Cactus";

        public EntityCactusModel(Entity parent) : base(parent, textureName, ModelUtil.getModel(modelName))
        {

        }
    }
}
