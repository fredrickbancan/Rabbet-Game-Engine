using Coictus.Debugging;
using Coictus.Models;
using Coictus.SubRendering;
using Coictus.VFX;
using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace Coictus
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class World
    {
        private ModelDrawable groundModel;
        private ModelDrawable wallsModel;
        private ModelDrawable skyboxModel;
        private Vector3 skyColor;
        private Vector3 fogColor;
        private int entityIDItterator = 0;//increases with each ent added, used as an ID for each world entity.
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public Dictionary<int, ICollider> entityColliders = new Dictionary<int, ICollider>();// the int is the given ID for the parent entity
        public List<ICollider> worldColliders = new List<ICollider>();//list of colliders with no parent, ie, walls, ground planes.
        public List<VFXBase> vfxList = new List<VFXBase>();
        private string wallTextureName = "plasterwall.png"; 
        private string terrainShaderName = "ColorTextureFog3D.shader"; 


        public World()
        {
            fogColor = CustomColor.lightBlossom.toNormalVec3();
            skyColor = CustomColor.skyBlue.toNormalVec3();
            buildSkyBox();
            generateWorld();
        }

        public void setSkyColor(Vector3 skyColor)
        {
            this.skyColor = skyColor;
        }
        public void setFogColor(Vector3 skyColor)
        {
            this.fogColor = skyColor;
        }

        public Vector3 getSkyColor()
        {
            return skyColor;
        }public Vector3 getFogColor()
        {
            return fogColor;
        }
      
        /*TODO: INNEFICIENT, loops through each entitiy and draws their model with a seperate call*/
        public void drawEntities(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            foreach(KeyValuePair<int, Entity> ent in entities)
            {
                if(ent.Value.getHasModel())
                {
                    ent.Value.getEntityModel().draw(viewMatrix, projectionMatrix, fogColor);
                }
            }
           
        }

        /*TODO: INNEFICIENT, loops through each VFX and draws their model with a seperate call*/
        public void drawVFX(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            foreach (VFXBase vfx in vfxList)
            {
                if (vfx.exists())
                {
                    vfx.draw(viewMatrix, projectionMatrix, fogColor);
                }
            }
        }


        /*creates simple inverted cube at 0,0,0*/
        private void buildSkyBox()
        {
            Model[] temp = new Model[6];
            temp[0] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 180, 0), new Vector3(0, 0, 0.5F));//posZ
            temp[1] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, -90, 0), new Vector3(-0.5F, 0, 0));//negX
            temp[2] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 90, 0), new Vector3(0.5F, 0, 0));//posX
            temp[3] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 0, 0), new Vector3(0, 0, -0.5F));//negZ
            temp[4] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(-90, 0, 0), new Vector3(0, 0.5F, 0));//top
            temp[5] = QuadPrefab.getNewModel().transformVertices(new Vector3(1, 1, 1), new Vector3(90, 0, 0), new Vector3(0, -0.5F, 0));//bottom
            skyboxModel = QuadBatcher.batchQuadModels(temp, "SkyboxShader3D.shader", "none");
        }

        private void generateWorld()//creates the playground and world colliders
        {
            float groundHeight = 0;
            float playgroundLength = 200;//"playground" is the term i am using for the playable area of the world
            float playgroundWidth = 100;
            float wallHeight = 20;
            Model[] unbatchedGroundQuads = new Model[4101];//all ground and wall quads, walls are divided into 4 quads each.
            Model[] unbatchedWallQuads = new Model[16];

            //Generating ground quads
            for(int x = 0; x < 64; x++)
            {
                for(int z = 0; z < 64; z++)
                {
                    unbatchedGroundQuads[x * 64 + z] = PlanePrefab.copyModel().scaleVertices(new Vector3(20,1,20)).scaleUV(new Vector2(5F,5F)).translateVertices(new Vector3((x-32)*20, groundHeight, (z-32)*20));
                }
            }

            //building the lump in middle of map, quads added after all the flat plane quads.

            //top face
            unbatchedGroundQuads[4096] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2,1,2)).translateVertices(new Vector3(0, 1F, 0));

            //negZ face
            unbatchedGroundQuads[4097] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2, 1, 1)).rotateVertices(new Vector3(90,0,0)).translateVertices(new Vector3(0, 0.5F, -1F)).setColor(new Vector4(0.8F, 0.8F, 0.8F, 1.0F));//pseudoLighting
            
            //posZ face
            unbatchedGroundQuads[4098] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2, 1, 1)).rotateVertices(new Vector3(-90,0,0)).translateVertices(new Vector3(0, 0.5F, 1F)).setColor(new Vector4(0.7F, 0.7F, 0.7F, 1.0F));

            //negX face
            unbatchedGroundQuads[4099] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(1, 1, 2)).rotateVertices(new Vector3(0,0,-90)).translateVertices(new Vector3(-1f, 0.5F, 0)).setColor(new Vector4(0.9F, 0.9F, 0.9F, 1.0F));

            //posX face
            unbatchedGroundQuads[4100] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(1, 1, 2)).rotateVertices(new Vector3(0,0,90)).translateVertices(new Vector3(1f, 0.5F, 0)).setColor(new Vector4(0.65F, 0.65F, 0.65F, 1.0F));
            groundModel = QuadBatcher.batchQuadModels(unbatchedGroundQuads, terrainShaderName, "sand.png"); 


            //build negZ wall
            for(int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[i] = QuadPrefab.getNewModel().scaleVertices(new Vector3(playgroundWidth/4, wallHeight, 1)).scaleUV(new Vector2(playgroundWidth / (wallHeight * 4), 1)).translateVertices(new Vector3((-playgroundWidth/2) + ((playgroundWidth/4)/2) + ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, -playgroundLength/2)).setColor(new Vector4(0.7F, 0.7F, 0.7F, 1.0F));
            }

            //build posZ wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[ 4 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3(playgroundWidth / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundWidth / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, 180, 0)).translateVertices(new Vector3((playgroundWidth / 2) - ((playgroundWidth / 4) / 2) - ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, playgroundLength / 2)).setColor(new Vector4(0.8F, 0.8F, 0.8F, 1.0F)); ;
            }

            //build negX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[8 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, -90, 0)).translateVertices(new Vector3(-playgroundWidth / 2, groundHeight + wallHeight / 2, (-playgroundLength / 2) + ((playgroundLength / 4) / 2) + ((playgroundLength / 4) * i))).setColor(new Vector4(0.65F, 0.65F, 0.65F, 1.0F));
            }

            //build posX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[12 + i] = QuadPrefab.getNewModel().scaleVertices(new Vector3(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, 90, 0)).translateVertices(new Vector3(playgroundWidth / 2, groundHeight + wallHeight / 2, (playgroundLength / 2) - ((playgroundLength / 4) / 2) - ((playgroundLength / 4) * i))).setColor(new Vector4(0.9F, 0.9F, 0.9F, 1.0F));
            }


            wallsModel = QuadBatcher.batchQuadModels(unbatchedWallQuads, terrainShaderName, wallTextureName);
            //adding all world collider planes
            this.addWorldCollider(new PlaneCollider(new Vector3(0, 1, 0), groundHeight));//ground plane at y groundHeight, facing positive Y
            this.addWorldCollider(new PlaneCollider(new Vector3(0,0,1), -playgroundLength / 2));//Wall at negZ, playgroundLength / 2 units away, facing pos Z
            this.addWorldCollider(new PlaneCollider(new Vector3(0,0,-1), -playgroundLength / 2));//Wall at posZ, playgroundLength / 2 units away, facing negZ
            this.addWorldCollider(new PlaneCollider(new Vector3(1,0,0), -playgroundWidth / 2));//Wall at negX, playgroundWidth / 2 units away, facing pos X
            this.addWorldCollider(new PlaneCollider(new Vector3(-1,0,0), -playgroundWidth / 2));//Wall at posX, playgroundWidth / 2 units away, facint negX
            this.addWorldCollider(new AABBCollider(new Vector3(-1,0,-1), new Vector3(1,1,1)));//2x2 lump in middle of playground
        }
        
        public void onTick()
        {
            removeMarkedEntities();
            removeMarkedVFX();
            tickEntities();
            tickVFX();
            doCollisions();//this should be done AFTER ticking entities.
            if (GameSettings.drawHitboxes)
            {
                updateAllEntityColliders();//For correcting the drawing of hitboxes after a collision
                HitboxRenderer.addAllHitboxesToBeRendered(worldColliders, entityColliders);
            }
        }

        /*Called every frame. Should not do any heavy computation, only for preparing certain things for rendering. E.G: correcting models.*/
        public void onFrame()
        {
            foreach (Entity ent in entities.Values)
            {
                if (ent != null)
                {
                    ent.onFrame();
                }
            }
        }

        private void tickEntities()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities.Values.ElementAt(i).preTick();
                entities.Values.ElementAt(i).onTick();
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities.Values.ElementAt(i).postTick();
            }
        }

        private void updateAllEntityColliders()
        {
            foreach (Entity ent in entities.Values)
            {
                if (ent != null)
                {
                    ent.tickUpdateCollider();
                }
            }
        }

        private void tickVFX()
        {
            foreach (VFXBase vfx in vfxList)
            {
                if (vfx != null)
                {
                    vfx.preTick();
                    vfx.onTick();
                    vfx.postTick();
                }
            }
        }

        /*Loops through all the different colliders in the world in the right order and applies physics to the
          parent entities.*/
        private void doCollisions()
        {
            Profiler.beginEndProfile(Profiler.collisionsName);
            if (entityColliders.Count >= 2)
            {
                CollisionHandler.doEntityCollisions(entityColliders);
            }
            if(worldColliders.Count > 0 && entityColliders.Count >= 1)
            {
                CollisionHandler.doWorldCollisions(worldColliders, entityColliders);
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
        public void doExplosionAt(Vector3 loc, float radius = 7, float power = 2)
        {
            //render an explosion effect
            VFXUtil.doExplosionEffect(this, loc, radius);

            //force away nearby entities
            foreach (Entity ent in entities.Values)
            {
                if (ent!= null)
                {
                    float distanceFromLocation = (ent.getPosition() - loc).LengthFast;
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

        public void spawnEntityInWorldAtPosition(Entity theEntity, Vector3 atPosition)
        {
            theEntity.setPosition(atPosition);
            spawnEntityInWorld(theEntity);
        }

        public void spawnVFXInWorld(VFXBase vfx)
        {
            vfxList.Add(vfx);
        }

        public int getEntityCount()
        {
            return entities.Count;
        }
        public int getVFXCount()
        {
            return vfxList.Count;
        }
    }
}
