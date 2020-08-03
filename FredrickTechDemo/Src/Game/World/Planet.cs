using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering;
using System.Collections.Generic;

namespace FredrickTechDemo
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class Planet
    {
        private ModelDrawable tempWorldModel;

        private List<Entity> entities;
        public Planet()
        {
            entities = new List<Entity>();
            generateWorld();
        }

        public void generateWorld()
        {
            Model[] unBatchedQuads = new Model[256];
            for(int x = 0; x < 16; x++)
            {
                for(int z = 0; z < 16; z++)
                {
                    unBatchedQuads[x * 16 + z] = PlanePrefab.getNewModel().translateVertices(new Vector3F(x-8, 0, z-8));
                }
            }
            tempWorldModel = QuadBatcher.batchQuadModels(unBatchedQuads, PlanePrefab.getShaderDir(), PlanePrefab.getTextureDir());
        }

        public ModelDrawable getTerrainModel()
        {
            return tempWorldModel;
        }

        public void spawnEntityInWorld(Entity theEntity)
        {
            entities.Add(theEntity);
        }
        public void spawnEntityInWorldAtPosition(Entity theEntity, Vector3D atPosition)
        {
            theEntity.setPosition(atPosition);
            entities.Add(theEntity);
        }
        
        public void removeEntity(Entity theEntity)
        {
            entities.Remove(theEntity);
        }
    }
}
