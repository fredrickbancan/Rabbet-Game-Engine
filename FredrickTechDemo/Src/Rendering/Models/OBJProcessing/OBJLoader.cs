using System;

namespace FredrickTechDemo.Models
{
    /*This class will be responsable for loading models and converting them to vertex arrays for use with rendering.*/
    public static class OBJLoader
    {
        public static Vertex[] loadModelFromObjFile(String filePath)
        {

            return EntityCactusModel.getNewModel().vertices;//returns cactus model by defualt or failing
        }
    }
}
