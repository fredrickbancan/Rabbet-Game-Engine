namespace RabbetGameEngine.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static string shaderName = "ColorTextureFog3D.shader";
        private static string textureName = "EntityCactus.png";
        private static string modelName = "Cactus.obj";

        public EntityCactusModel(Entity parent) : base(parent, shaderName, textureName, modelName)
        {

        }
    }
}
