using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbetGameEngine.Models
{
    /*This class is for loading and providing model data to be used throughout the game, without having to 
      re-load them each time they are used.*/
    public static class ModelUtil
    {
        private static Dictionary<string, Model> models = null;
        public static void loadAllFoundModelFiles()
        {
            models = new Dictionary<string, Model>();
            models.Add("debug", DefaultDebugModel.getModelCopy());
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
            string shaderName = Path.GetFileName(modelDir).Replace(".obj", "");//removes directory//removes directory
            Model addingModel = OBJLoader.loadModelFromObjFile(modelDir);
            models.Add(shaderName, addingModel);
        }

        /*Returns true if the requested Model was found in the global list*/
        public static bool tryGetModel(string name, out Model model)
        {
            bool success = models.TryGetValue(name, out model);
            if (!success)
            {
                Application.error("ModelUtil could not find Model named: " + name + " in global list, assigning debug model.");
                model = models.ElementAt(0).Value;
            }
            return success;
        }

        
        public static Model getModel(string modelName)
        {
            Model result;
            tryGetModel(modelName, out result);
            return result;
        }


        public static int getModelCount()
        {
            return models.Count;
        }
    }
}
