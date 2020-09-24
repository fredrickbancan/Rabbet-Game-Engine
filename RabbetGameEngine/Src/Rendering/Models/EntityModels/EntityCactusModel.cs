namespace RabbetGameEngine.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static string shaderName = "EntityWorld_F";
        private static string textureName = "EntityCactus";
        private static string modelName = "Cactus.obj";

        public EntityCactusModel(Entity parent) : base(parent, shaderName, textureName, modelName)
        {

        }
    }
}
