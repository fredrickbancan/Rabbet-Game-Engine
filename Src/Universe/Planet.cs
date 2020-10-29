using Medallion;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using RabbetGameEngine.SubRendering;
using RabbetGameEngine.VFX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    /*This class will be the abstraction of any environment constructed for the player and entities to exist in.*/
    public class Planet
    {
        private Vector3 fogColor;
        private int entityIDItterator = 0;//increases with each ent added, used as an ID for each world entity.
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public List<AABB> worldColliders = new List<AABB>();//list of colliders with no parent, ie, walls.
        public List<VFXBase> vfxList = new List<VFXBase>();
        private Skybox planetSkybox;
        private string wallTextureName = "transparent";
        private string groundTextureName = "wood";
        private Random random;

        public Planet(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            fogColor = CustomColor.lightGrey.toNormalVec3();
            planetSkybox = new Skybox(CustomColor.lightSkyBlue.toNormalVec3(), this);
            SkyboxRenderer.setSkyboxToDraw(planetSkybox);
            generateWorld();
        }
        public Vector3 getFogColor()
        {
            return fogColor;
        }

        private void generateWorld()//creates the playground and world colliders
        {
            float groundHeight = 0;
            float playgroundLength = 100;//"playground" is the term i am using for the playable area of the world
            float playgroundWidth = 100;
            float wallHeight = 20;
            Model[] unbatchedGroundQuads = new Model[4101];//all ground and wall quads, walls are divided into 4 quads each.
            Model[] unbatchedWallQuads = new Model[16];

            //Generating ground quads
            for (int x = 0; x < 64; x++)
            {
                for (int z = 0; z < 64; z++)
                {
                    unbatchedGroundQuads[x * 64 + z] = PlanePrefab.copyModel().scaleVertices(new Vector3(20, 1, 20)).scaleUV(new Vector2(5F, 5F)).translateVertices(new Vector3((x - 32) * 20, groundHeight, (z - 32) * 20));
                }
            }

            //building the lump in middle of map, quads added after all the flat plane quads.

            //top face
            unbatchedGroundQuads[4096] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2, 1, 2)).translateVertices(new Vector3(0, 1F, 0));

            //negZ face
            unbatchedGroundQuads[4097] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2, 1, 1)).rotateVertices(new Vector3(90, 0, 0)).translateVertices(new Vector3(0, 0.5F, -1F)).setColor(new Vector4(0.8F, 0.8F, 0.8F, 1.0F));//pseudoLighting

            //posZ face
            unbatchedGroundQuads[4098] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(2, 1, 1)).rotateVertices(new Vector3(-90, 0, 0)).translateVertices(new Vector3(0, 0.5F, 1F)).setColor(new Vector4(0.7F, 0.7F, 0.7F, 1.0F));

            //negX face
            unbatchedGroundQuads[4099] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(1, 1, 2)).rotateVertices(new Vector3(0, 0, -90)).translateVertices(new Vector3(-1f, 0.5F, 0)).setColor(new Vector4(0.9F, 0.9F, 0.9F, 1.0F));

            //posX face
            unbatchedGroundQuads[4100] = PlanePrefab.copyModel().scaleVerticesAndUV(new Vector3(1, 1, 2)).rotateVertices(new Vector3(0, 0, 90)).translateVertices(new Vector3(1f, 0.5F, 0)).setColor(new Vector4(0.65F, 0.65F, 0.65F, 1.0F));

            //build negZ wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[i] = QuadPrefab.copyModel().scaleVertices(new Vector3(playgroundWidth / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundWidth / (wallHeight * 4), 1)).translateVertices(new Vector3((-playgroundWidth / 2) + ((playgroundWidth / 4) / 2) + ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, -playgroundLength / 2)).setColor(new Vector4(0.7F, 0.7F, 0.7F, 1.0F));
            }

            //build posZ wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[4 + i] = QuadPrefab.copyModel().scaleVertices(new Vector3(playgroundWidth / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundWidth / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, 180, 0)).translateVertices(new Vector3((playgroundWidth / 2) - ((playgroundWidth / 4) / 2) - ((playgroundWidth / 4) * i), groundHeight + wallHeight / 2, playgroundLength / 2)).setColor(new Vector4(0.8F, 0.8F, 0.8F, 1.0F)); ;
            }

            //build negX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[8 + i] = QuadPrefab.copyModel().scaleVertices(new Vector3(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, -90, 0)).translateVertices(new Vector3(-playgroundWidth / 2, groundHeight + wallHeight / 2, (-playgroundLength / 2) + ((playgroundLength / 4) / 2) + ((playgroundLength / 4) * i))).setColor(new Vector4(0.65F, 0.65F, 0.65F, 1.0F));
            }

            //build posX wall
            for (int i = 0; i < 4; i++)
            {
                unbatchedWallQuads[12 + i] = QuadPrefab.copyModel().scaleVertices(new Vector3(playgroundLength / 4, wallHeight, 1)).scaleUV(new Vector2(playgroundLength / (wallHeight * 4), 1)).rotateVertices(new Vector3(0, 90, 0)).translateVertices(new Vector3(playgroundWidth / 2, groundHeight + wallHeight / 2, (playgroundLength / 2) - ((playgroundLength / 4) / 2) - ((playgroundLength / 4) * i))).setColor(new Vector4(0.9F, 0.9F, 0.9F, 1.0F));
            }

            Model batchedGround = TriangleCombiner.combineData(unbatchedGroundQuads);
            Model batchedWalls = TriangleCombiner.combineData(unbatchedWallQuads);

            Renderer.addStaticDrawTriangles("ground", groundTextureName, batchedGround);
            Renderer.addStaticDrawTriangles("walls", wallTextureName, batchedWalls);

            //adding world colliders
            this.addWorldAABB(new AABB(new Vector3(-(playgroundWidth * 0.5F), -2, -(playgroundLength * 0.5F)), new Vector3(playgroundWidth * 0.5F, 0, playgroundLength * 0.5F)));//AABB for ground
            this.addWorldAABB(new AABB(new Vector3(-(playgroundWidth * 0.5F), 0, playgroundLength * 0.5F), new Vector3(playgroundWidth * 0.5F, wallHeight, playgroundLength * 0.5F + 2)));//AABB for pos Z wall
            this.addWorldAABB(new AABB(new Vector3(-(playgroundWidth * 0.5F), 0, -(playgroundLength * 0.5F) - 2), new Vector3(playgroundWidth * 0.5F, wallHeight, -(playgroundLength * 0.5F))));//AABB for neg Z wall
            this.addWorldAABB(new AABB(new Vector3(playgroundWidth * 0.5F, 0, -(playgroundLength * 0.5F)), new Vector3(playgroundWidth * 0.5F + 2, wallHeight, playgroundLength * 0.5F)));//AABB for pos X wall
            this.addWorldAABB(new AABB(new Vector3(-(playgroundWidth * 0.5F) - 2, 0, -(playgroundLength * 0.5F)), new Vector3(-(playgroundWidth * 0.5F), wallHeight, playgroundLength * 0.5F)));//AABB for neg X wall
            this.addWorldAABB(new AABB(new Vector3(-1, 0, -1), new Vector3(1, 1, 1)));//2x1x2 lump in middle of playground
        }

        public void onTick()
        {
            planetSkybox.onTick();

            tickVFX();

            doEntityCollisions();

            tickEntities();

            if (GameSettings.drawHitboxes)
            {
                HitboxRenderer.addAllHitboxesToBeRendered(worldColliders, entities);
            }
        }

        /*Called every frame. Should not do any heavy computation, only for preparing certain things for rendering. E.G: correcting models.*/
        public void onFrame()
        {
            foreach (Entity ent in entities.Values)
            {
                ent.onFrame();
            }
        }

        private void tickEntities()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entAt = entities.ElementAt(i).Value;
                if (entAt == null)
                {
                    entities.Remove(entities.ElementAt(i).Key);
                    i--;
                    continue;
                }
                else if (entAt.getIsMarkedForRemoval())
                {
                    entAt.setCurrentPlanet(null);
                    entities.Remove(entities.ElementAt(i).Key);
                    i--;
                    continue;
                }
                entAt.preTick();
                entAt.onTick();
                entAt.postTick();
                if (entAt.getIsPlayer() && GameSettings.noclip)
                {
                    entAt.getColliderHandle().offset(entAt.getVelocity());
                    entAt.setPosition(entAt.getPosition() + entAt.getVelocity());
                }
                else
                {
                    CollisionHandler.tryToMoveObject(entAt, worldColliders);
                }
                if(entAt.getHasModel())
                {
                    entAt.getEntityModel().onTick();
                    entAt.getEntityModel().sendRenderRequest();
                }
            }

        }
        private void tickVFX()
        {
            for (int i = 0; i < vfxList.Count; i++)
            {
                VFXBase vfx = vfxList.ElementAt(i);
                if (vfx == null)
                {
                    vfxList.Remove(vfx);
                    i--;
                }
                else if (!vfx.exists())
                {
                    vfxList.Remove(vfxList.ElementAt(i));
                    i--;
                }
                else
                {
                    vfx.preTick();
                    vfx.onTick();
                    vfx.postTick();
                    CollisionHandler.tryToMoveObject(vfx, worldColliders);
                }

                vfx.sendRenderRequest();
            }
        }

        private void doEntityCollisions()
        {
            CollisionHandler.collideEntities(entities);
        }


        /*For adding colliders with no entity parent, eg, for map collisions, inanimate immovable uncolliding objects.*/
        public void addWorldAABB(AABB collider)
        {
            worldColliders.Add(collider);
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

        public void spawnEntityInWorld(Entity theEntity)
        {
            entities.Add(entityIDItterator++, theEntity);
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

        public void onLeavingPlanet()
        {

        }

        public Random rand { get => this.random; }
    }
}
