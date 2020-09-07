using System;

namespace Coictus.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static String shaderDir = ResourceUtil.getShaderFileDir("ColorTextureFog3D.shader");
        private static String textureDir = ResourceUtil.getTextureFileDir("EntityCactus.png");
        private static String modelPath = ResourceUtil.getOBJFileDir("Cactus.obj");

        public EntityCactusModel(Entity parent) : base(parent, shaderDir, textureDir, modelPath)
        {

        }
    }
}
