using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering;
using System.Collections.Generic;
using System.Linq;

namespace FredrickTechDemo
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class Planet
    {
        private ModelDrawable tempWorldModel;
        private ModelDrawable skyboxModel;
        private Vector3F skyColor;
        private Vector3F fogColor;

        public List<Entity> entities;
        public Planet()
        {
            fogColor = ColourF.lightBlossom.normalVector3F();
            skyColor = ColourF.skyBlue.normalVector3F();
            entities = new List<Entity>();
            buildSkyBox();
            generateWorld();
        }

        public void setSkyColor(Vector3F skyColor)
        {
            this.skyColor = skyColor;
        }
        public void setFogColor(Vector3F skyColor)
        {
            this.fogColor = skyColor;
        }

        public Vector3F getSkyColor()
        {
            return skyColor;
        }public Vector3F getFogColor()
        {
            return fogColor;
        }

        /*Loop through each entity and render them with a seperate draw call (INEFFICIENT)*/
        public void drawEntities(Matrix4F viewMatrix, Matrix4F projectionMatrix)
        {
            foreach(Entity ent in entities)
            {
                if(ent.getHasModel())
                {
                    ent.getEntityModel().draw(viewMatrix, projectionMatrix, fogColor);
                }
            }
        }
        private void buildSkyBox()
        {
            Model[] temp = new Model[6];
            temp[0] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, 180, 0), new Vector3F(0, 0, 0.5F));//posZ
            temp[1] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, -90, 0), new Vector3F(-0.5F, 0, 0));//negX
            temp[2] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, 90, 0), new Vector3F(0.5F, 0, 0));//posX
            temp[3] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(0, 0, 0), new Vector3F(0, 0, -0.5F));//negZ
            temp[4] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(-90, 0, 0), new Vector3F(0, 0.5F, 0));//top
            temp[5] = QuadPrefab.getNewModel().transformVertices(new Vector3F(1, 1, 1), new Vector3F(90, 0, 0), new Vector3F(0, -0.5F, 0));//bottom
            skyboxModel = QuadBatcher.batchQuadModels(temp, ResourceHelper.getShaderFileDir("SkyboxShader3D.shader"), QuadPrefab.getTextureDir());
        }

        private void generateWorld()
        {
            Model[] unBatchedQuads = new Model[4096];
            for(int x = 0; x < 64; x++)
            {
                for(int z = 0; z < 64; z++)
                {
                    unBatchedQuads[x * 64 + z] = PlanePrefab.getNewModel().translateVertices(new Vector3F(x-32, 0, z-32));
                }
            }
            tempWorldModel = QuadBatcher.batchQuadModels(unBatchedQuads, PlanePrefab.getShaderDir(), PlanePrefab.getTextureDir());
        }
        
        public void onTick()
        {
            removeMarkedEntities();
            tickEntities();
        }

        private void tickEntities()
        {
            foreach(Entity ent in entities)
            {
                if (ent != null)
                {
                    ent.onTick();
                }
            }
        }

        private void removeMarkedEntities()
        {
            for(int i = 0; i < entities.Count; i++)
            {
                if (entities.ElementAt(i) != null && entities.ElementAt(i).getIsMarkedForRemoval())
                {
                    removeEntity(entities.ElementAt(i));
                }
            }
        }

        /*creates an impulse at the given location which will push entities away, 
          like an explosion.*/
        public void createImpulseAtLocation(Vector3D loc, double radius = 4, float power = 1)
        {
            foreach (Entity ent in entities)
            {
                if (ent != null)
                {
                    double distanceFromLocation = (ent.getPosition() - loc).Magnitude();
                    if (distanceFromLocation < radius)
                    {
                        ent.applyImpulseFromLocation(loc, (1 - MathUtil.normalize(0, (float)radius, (float)distanceFromLocation)) * power);
                    }
                }
            }
        }

        public ModelDrawable getSkyboxModel()
        {
            return skyboxModel;
        }

        public ModelDrawable getTerrainModel()//temporary
        {
            return tempWorldModel;
        }

        public void spawnEntityInWorld(Entity theEntity)
        {
            theEntity.setCurrentPlanet(this);
            entities.Add(theEntity);
        }

        public void spawnEntityInWorldAtPosition(Entity theEntity, Vector3D atPosition)
        {
            theEntity.setCurrentPlanet(this);
            theEntity.setPosition(atPosition);
            entities.Add(theEntity);
        }
        
        public void removeEntity(Entity theEntity)
        {
            theEntity.setCurrentPlanet(null);
            entities.Remove(theEntity);
        }
    }
}
