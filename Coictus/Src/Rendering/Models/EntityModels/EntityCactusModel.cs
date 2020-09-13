using System;

namespace Coictus.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static string shaderDir = ResourceUtil.getShaderFileDir("ColorTextureFog3D.shader");
        private static string textureDir = ResourceUtil.getTextureFileDir("EntityCactus.png");
        private static string modelPath = ResourceUtil.getOBJFileDir("Cactus.obj");

        public EntityCactusModel(Entity parent) : base(parent, shaderDir, textureDir, modelPath)
        {

        }
    }
}
