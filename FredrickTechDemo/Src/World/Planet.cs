using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering;
using FredrickTechDemo.VFX;
using System.Collections.Generic;
using System.Linq;

namespace FredrickTechDemo
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class Planet
    {
        private ModelDrawableDynamic particleModel;//TODO: impliment dynamic global particle model for batching
        private ModelDrawable tempWorldModel;
        private ModelDrawable skyboxModel;
        private Vector3F skyColor;
        private Vector3F fogColor;
        private int entityIDItterator = 0;//increases with each ent added, used as an ID for each world entity.
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public Dictionary<int, ICollider> entityColliders = new Dictionary<int, ICollider>();// the int is the given ID for the parent entity
        public List<ICollider> worldColliders = new List<ICollider>();//list of colliders with no parent, ie, walls, ground planes.
        public List<VFXBase> vfxList = new List<VFXBase>();

        public Planet()
        {
            fogColor = ColourF.lightBlossom.normalVector3F();
            skyColor = ColourF.skyBlue.normalVector3F();
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
            foreach(KeyValuePair<int, Entity> ent in entities)
            {
                if(ent.Value.getHasModel())
                {
                    ent.Value.getEntityModel().draw(viewMatrix, projectionMatrix, fogColor);
                }
            }
        }

        /*Loop through each vfx and render them with a seperate draw call (INEFFICIENT)*/
        public void drawVFX(Matrix4F viewMatrix, Matrix4F projectionMatrix, int pass = 1)
        {
            foreach (VFXBase vfx in vfxList)
            {
                if (vfx.exists())
                {
                    vfx.draw(viewMatrix, projectionMatrix, fogColor, pass);
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
                    unBatchedQuads[x * 64 + z] = PlanePrefab.getNewModel().scaleVerticesAndUV(new Vector3F(20,1,20)).translateVertices(new Vector3F((x-32)*20, 0, (z-32)*20));
                }
            }
            tempWorldModel = QuadBatcher.batchQuadModels(unBatchedQuads, PlanePrefab.getShaderDir(), PlanePrefab.getTextureDir());

            this.addWorldCollider(new PlaneCollider(new Vector3D(0,1,0), 0));//ground plane at y 0
        }
        
        public void onTick()
        {
            removeMarkedEntities();
            removeMarkedVFX();
            tickEntities();
            tickVFX();
            doCollisions();//I THINK this should be done LAST.....
        }

        private void tickEntities()
        {
            foreach(KeyValuePair<int, Entity> ent in entities)
            {
                if (ent.Value != null)
                {
                    ent.Value.onTick();
                }
            }
        }

        private void tickVFX()
        {
            foreach (VFXBase vfx in vfxList)
            {
                if (vfx != null)
                {
                    vfx.onTick();
                }
            }
        }

        /*Loops through all the different colliders in the world in the right order and applies physics to the
          parent entities.*/
        private void doCollisions()
        {
            Profiler.beginEndProfile(Profiler.collisionsName);
            if(worldColliders.Count > 0 && entityColliders.Count >= 1)
            {
                CollisionHandler.doWorldCollisions(worldColliders, entityColliders);
            }

            if (entityColliders.Count >= 2)
            {
                CollisionHandler.doEntityCollisions(entityColliders);
            }
            Profiler.beginEndProfile(Profiler.collisionsName);
        }

        /*For adding colliders with no entity parent, eg, for map collisions, inanimate immovable uncolliding objects.*/
        public void addWorldCollider(ICollider collider)
        {
            worldColliders.Add(collider);
        }

        private void removeMarkedEntities()
        {
            for(int i = 0; i < entities.Count; i++)
            {
                Entity entAt = entities.ElementAt(i).Value;

                if (entAt != null && entAt.getIsMarkedForRemoval())
                {
                    entAt.setCurrentPlanet(null);
                    if(entAt.getHasCollider())
                    {
                        entityColliders.Remove(entities.ElementAt(i).Key);
                    }
                    entities.Remove(entities.ElementAt(i).Key);
                    
                }
            }
        }

        private void removeMarkedVFX()
        {
            for (int i = 0; i < vfxList.Count; i++)
            {
                if (vfxList.ElementAt(i) != null && !vfxList.ElementAt(i).exists())
                {
                    vfxList.Remove(vfxList.ElementAt(i));
                }
            }
        }

        /*creates an impulse at the given location which will push entities away, 
          like an explosion.*/
        public void doExplosionAt(Vector3D loc, float radius = 7, float power = 1)
        {
            //render an explosion effect
            VFXUtil.doExplosionEffect(this, loc, radius);

            //force away nearby entities
            foreach (Entity ent in entities.Values)
            {
                if (ent!= null)
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

            entities.Add(entityIDItterator, theEntity);

            if(theEntity.getHasCollider())
            {
                entityColliders.Add(entityIDItterator, theEntity.getCollider());
            }

            entityIDItterator++;
        }

        public void spawnEntityInWorldAtPosition(Entity theEntity, Vector3D atPosition)
        {
            theEntity.setPosition(atPosition);
            spawnEntityInWorld(theEntity);
        }

        public void spawnVFXInWorld(VFXBase vfx)
        {
            vfxList.Add(vfx);
        }
    }
}
