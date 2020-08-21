using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using FredrickTechDemo.SubRendering;
using FredrickTechDemo.VFX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FredrickTechDemo
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class World
    {
        private ModelDrawableDynamic particleModel;//TODO: impliment dynamic global particle model for batching
        private ModelDrawable groundModel;
        private ModelDrawable wallsModel;
        private ModelDrawable skyboxModel;
        private Vector3F skyColor;
        private Vector3F fogColor;
        private int entityIDItterator = 0;//increases with each ent added, used as an ID for each world entity.
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public Dictionary<int, ICollider> entityColliders = new Dictionary<int, ICollider>();// the int is the given ID for the parent entity
        public List<ICollider> worldColliders = new List<ICollider>();//list of colliders with no parent, ie, walls, ground planes.
        public List<VFXBase> vfxList = new List<VFXBase>();
        private String wallTextureDir = ResourceHelper.getTextureFileDir("plasterwall.png"); 


        public World()
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
            float groundHeight = 0;
            float playgroundLength = 72;//"playground" is the term i am using for the playable area of the world
            float playgroundWidth = 50;
            float wallHeight = 8;
            Model[] unbatchedGroundQuads = new Model[4096];//all ground and wall quads, walls are divided into 4 quads each.
            Model[] unbatchedWallQuads = new Model[16];

            //Generating ground quads
            for(int x = 0; x < 64; x++)
            {
                for(int z = 0; z < 64; z++)
                {
                    unbatchedGroundQuads[x * 64 + z] = PlanePrefab.getNewModel().scaleVerticesAndUV(new Vector3F(20,1,20)).translateVertices(new Vector3F((x-32)*20, groundHeight, (z-32)*20));
                }
            }
            groundModel = QuadBatcher.batchQuadModels(unbatchedGroundQuads, PlanePrefab.getShaderDir(), PlanePrefab.getTextureDir()); 


            //build negZ wall
            for(int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[i] = QuadPrefab.getNewModel().scaleVertices(new Vector3F(playgroundWidth/4, wallHeight, 1)).scaleUV(new Vector2F(playgroundWidth / (wallHeight * 4), 1)).translateVertices(new Vector3F((-playgroundWidth/2) + ((playgroundWidth/4)/2) + ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, -playgroundLength/2));
            }

            //build posZ wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[ 4 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3F(playgroundWidth / 4, wallHeight, 1)).scaleUV(new Vector2F(playgroundWidth / (wallHeight * 4), 1)).rotateVertices(new Vector3F(0, 180, 0)).translateVertices(new Vector3F((playgroundWidth / 2) - ((playgroundWidth / 4) / 2) - ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, playgroundLength / 2));
            }

            //build negX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[8 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3F(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2F(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3F(0, -90, 0)).translateVertices(new Vector3F(-playgroundWidth / 2, groundHeight + wallHeight / 2, (-playgroundLength / 2) + ((playgroundLength / 4) / 2) + ((playgroundLength / 4) * i)));
            }

            //build posX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[12 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3F(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2F(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3F(0, 90, 0)).translateVertices(new Vector3F(playgroundWidth / 2, groundHeight + wallHeight / 2, (playgroundLength / 2) - ((playgroundLength / 4) / 2) - ((playgroundLength / 4) * i)));
            }


            wallsModel = QuadBatcher.batchQuadModels(unbatchedWallQuads, QuadPrefab.getShaderDir(), wallTextureDir);
            //adding all world collider planes
            this.addWorldCollider(new PlaneCollider(new Vector3D(0,1,0), groundHeight));//ground plane at y groundHeight, facing positive Y
            this.addWorldCollider(new PlaneCollider(new Vector3D(0,0,1), -playgroundLength / 2));//Wall at negZ, playgroundLength / 2 units away, facing pos Z
            this.addWorldCollider(new PlaneCollider(new Vector3D(0,0,-1), -playgroundLength / 2));//Wall at posZ, playgroundLength / 2 units away, facing negZ
            this.addWorldCollider(new PlaneCollider(new Vector3D(1,0,0), -playgroundWidth / 2));//Wall at negX, playgroundWidth / 2 units away, facing pos X
            this.addWorldCollider(new PlaneCollider(new Vector3D(-1,0,0), -playgroundWidth / 2));//Wall at posX, playgroundWidth / 2 units away, facint negX
        }
        
        public void onTick()
        {
            removeMarkedEntities();
            removeMarkedVFX();
            tickEntities();
            tickVFX();
            doCollisions();//this should be done last
        }

        private void tickEntities()
        {
            foreach(Entity ent in entities.Values)
            {
                if (ent != null)
                {
                    ent.preTickMovement();
                    ent.onTick();
                    ent.postTickMovement();
                }
            }
        }

        private void tickVFX()
        {
            foreach (VFXBase vfx in vfxList)
            {
                if (vfx != null)
                {
                    vfx.preTickMovement();
                    vfx.onTick();
                    vfx.postTickMovement();
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
                else if(entAt == null)
                {
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
                else if(vfxList.ElementAt(i) == null)
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

        public ModelDrawable getGroundModel()
        {
            return groundModel;
        }
        public ModelDrawable getWallsModel()
        {
            return wallsModel;
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
