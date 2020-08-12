using System;

namespace FredrickTechDemo.Models
{
    public class EntityCactusModel : EntityModel
    {
        private static String shaderDir = ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        private static String textureDir = ResourceHelper.getTextureFileDir("EntityCactus.png");
        private static String modelPath = ResourceHelper.getOBJFileDir("Cactus.obj");

        public EntityCactusModel(Entity parent) : base(parent, shaderDir, textureDir, modelPath)
        {

        }
    }
}
