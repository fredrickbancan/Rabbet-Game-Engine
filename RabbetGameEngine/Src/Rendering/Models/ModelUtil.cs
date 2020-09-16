using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbetGameEngine.Models
{
    /*This class is for loading and providing model data to be used throughout the game, without having to 
      re-load them each time they are used.*/
    public static class ModelUtil
    {

        private static Dictionary<string, ModelDrawable> models = null;
        public static void loadAllFoundModelFiles()
        {
            models = new Dictionary<string, ModelDrawable>();
            models.Add("debug", DefaultDebugModel.getNewModelDrawable());
            loadAllModelsRecursive(ResourceUtil.getOBJFileDir());
        }

        private static void loadAllModelsRecursive(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);
            string[] allDirectories = Directory.GetDirectories(directory);
            foreach (string file in allFiles)
            {
                if (file.Contains(".obj"))
                {
                    tryAddNewModel(file);
                }
            }

            foreach (string dir in allDirectories)
            {
                loadAllModelsRecursive(dir);
            }
        }

        private static void tryAddNewModel(string modelDir)
        {
            string shaderName = modelDir.Replace(ResourceUtil.getOBJFileDir(), "");//removes directory
            ModelDrawable addingModel = OBJLoader.loadModelDrawableFromObjFile(modelDir);
            models.Add(shaderName, addingModel);
        }

        /*Returns true if the requested Model was found in the global list*/
        public static bool tryGetModel(string name, out ModelDrawable model)
        {
            bool success = models.TryGetValue(name, out model);
            if (!success)
            {
                Application.error("ModelUtil could not find Model named: " + name + " in global list, assigning debug model.");
                model = models.ElementAt(0).Value;
            }
            return success;
        }

        /*attempts to create a modeldrawable from the provided names*/
        public static ModelDrawable createModelDrawable(string shaderName, string textureName, string modelName)
        {
            if(modelName == "none")
            {
                return null;
            }
            ModelDrawable result;
            if(tryGetModel(modelName, out result))
            {
                Shader shader;
                ShaderUtil.tryGetShader(shaderName, out shader);
                result.setShader(shader);

                Texture tex;
                TextureUtil.tryGetTexture(textureName, out tex);
                result.setTexture(tex);
            }
            return result;
        }


        public static int getModelCount()
        {
            return models.Count;
        }

        public static void deleteAll()
        {
            foreach(ModelDrawable model in models.Values)
            {
                model.delete();
            }
        }
    }
}
