using Medallion;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using RabbetGameEngine.Sound;
using RabbetGameEngine.SubRendering;
using RabbetGameEngine.VisualEffects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    //TODO: Implement changing of sky colors for dusk and dawn.
    //TODO: add moon with unsynced orbit from sun orbit.
    public class Planet
    {
        private CustomColor fogColor;
        private CustomColor horizonColor;
        private CustomColor horizonColorDawn;
        private CustomColor horizonColorDusk;
        private CustomColor skyColor;
        private CustomColor skyAmbientColor;
        private CustomColor sunColor;
        private CustomColor sunColorDawn;
        private CustomColor sunColorDusk;
        private Vector3 sunDirection;
        private float sunAngle = 0;
        private float ambientBrightness = 0.05F;
        public int totalStars = 0;
        public int totalMoons = 0;
        private PointCloudModel stars = null;
        private SkyMoon[] moons = null;
        /// <summary>
        /// 0 at midnight, 1 at midday
        /// </summary>
        public float sunHeight = 0;
        /// <summary>
        /// How many minutes a day night cycle will take
        /// </summary>
        private int dayNightCycleMinutes = 1;

        /// <summary>
        /// Total number of ticks in a day night cycle from start to finish
        /// </summary>
        private int totalDayNightTicks = 0;

        /// <summary>
        /// Ticks counted in current day night cycle
        /// </summary>
        private int dayNightTicks = 0;

        /// <summary>
        /// percentage of progress from morning to morning. 0 is dawn, 0.5 is dusk. etc.
        /// </summary>
        private float dayNightPercent = 0;
        private int entityIDItterator = 0;//increases with each ent added, used as an ID for each world entity.
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public List<VFX> vfxList = new List<VFX>();
        public List<Entity> projectiles = new List<Entity>();
        public List<VFXMovingText3D> debugLabelList = new List<VFXMovingText3D>();
        public List<AABB> worldColliders = new List<AABB>();//list of colliders with no parent, ie, walls.
        private string wallTextureName = "leafywall";
        private string groundTextureName = "jungleground";
        private static readonly Vector3 fallPlaneRespawnPos = new Vector3(0, 128, 0);
        private static readonly float fallPlaneHeight = -10.0F;
        private Random random;
        private float fogStart;
        private float fogEnd;
        private float drawDistance = 0;
        public Planet(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            horizonColor = CustomColor.lightOrange;
            horizonColorDawn = CustomColor.lightOrange;
            horizonColorDusk = CustomColor.dusk;
            skyAmbientColor = CustomColor.darkBlue.copy().reduceVibrancy(0.5F);
            fogColor = CustomColor.lightGrey;
            skyColor = CustomColor.skyBlue;
            sunColor = CustomColor.lightYellow;
            sunColorDawn = CustomColor.lightYellow;
            sunColorDusk = CustomColor.flame;
            dayNightCycleMinutes = rand.Next(15,61);
            totalDayNightTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(dayNightCycleMinutes * 60);
            dayNightTicks = totalDayNightTicks / 4;
            setDrawDistanceAndFog(150.0F);
            buildMoons();
            buildStars();
            SkyboxRenderer.setSkyboxToDraw(this);
            generateWorld();
        }

        private void buildMoons()
        {
            totalMoons = rand.Next(1, 101);
            moons = new SkyMoon[totalMoons];
            float moonColorStrength = 0.072F;
            float maxMoonRadius = 0.075F;
            float minMoonRadius = 0.05F;
            float spacing = 1.0F / (float)totalMoons * 0.2F;
            for (int i = 0; i < totalMoons; i++)
            {
                float orbitScale = 1 - i * spacing;
                moons[i] = new SkyMoon(
                    new Vector3((0.9999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.9999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.9999F - (float)rand.NextDouble()*0.9999F) + 0.0001F).Normalized() * orbitScale,
                    new Vector2((0.4999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.4999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F).Normalized(),
                    new Vector4(1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F),
                    (float)rand.NextDouble() * maxMoonRadius + minMoonRadius,
                    (float)rand.NextDouble() * 360.0F,
                    rand.Next(0, SkyMoon.totalMoonTextures),
                    1/*rand.Next(5,15)*/,
                    orbitScale);
            }
        }
        private void tickMoons()
        {
            for (int i = 0; i < totalMoons; i++)
            {
                moons[i].onTick();
            }
        }

        public SkyMoon[] getMoons()
        {
            return moons;
        }

        private void buildStars()//must be done before building skybox.
        {
            totalStars = rand.Next(3000,3501);
            PointParticle[] points = new PointParticle[totalStars];
            float starColorStrength = 0.3072F;
            float maxStarRadius = 0.01F;
            for (int i = 0; i < totalStars; i++)
            {
                Vector3 pos = new Vector3(0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble());
                points[i] = new PointParticle(
                    pos.Normalized(),
                    new Vector4(1.0F - (float)rand.NextDouble() * starColorStrength, 1.0F - (float)rand.NextDouble() * starColorStrength, 1.0F - (float)rand.NextDouble() * starColorStrength, 1.0F),
                    (float)rand.NextDouble() * maxStarRadius + 0.0025F,
                    false
                    );
            }
            Vector3 galaxyPlaneNormal = new Vector3(0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble()).Normalized();
            float galaxyClusterStrength = 0.95F;

            float distToPlane = 0;
            for(int i = 0; i < totalStars/1.5F; i++)
            {
                distToPlane = Vector3.Dot(galaxyPlaneNormal, points[i].pos);
                points[i].pos += galaxyPlaneNormal * (-distToPlane * galaxyClusterStrength);
                points[i].pos.Normalize();
            }

            stars = new PointCloudModel(points, false);
        }

        public PointCloudModel getStars()
        {
            return stars;
        }

        public Vector3 getFogColor()
        {
            return fogColor.setBrightPercent(getGlobalBrightness()).toNormalVec3();
        }
        public Vector3 getHorizonColor()
        { 
            return horizonColor.mix(CustomColor.white, MathUtil.normalizeClamped(0.5F, 0.75F, sunHeight * sunHeight)).setBrightPercent(MathHelper.Clamp(sunHeight * 2F, 0, 1)).toNormalVec3();
        }
        public Vector3 getSkyColor()
        {
            return skyColor.setBrightPercent(sunHeight + (0.5F * 1 - sunHeight * 0.5F)).toNormalVec3();
        }

        public Vector3 getSkyAmbientColor()
        {
            return skyAmbientColor.mix(skyColor, MathUtil.normalizeClamped(0.5F, 0.75F, sunHeight * sunHeight)).setBrightPercent(1- sunHeight * sunHeight * 1.25F).toNormalVec3();
        }
        public Vector3 getSunDirection()
        {
            return sunDirection;
        }
    
        public Vector3 getSunColor()
        {
            return sunColor.copy().reduceVibrancy(MathUtil.normalizeClamped(0.5F, 1, sunHeight * 0.85F)).toNormalVec3();
        }

        /// <summary>
        /// Returns float between 0 and 1 for brighness. 1 is fullbright.
        /// </summary>
        public float getGlobalBrightness()
        {
            return MathHelper.Clamp(MathF.Pow(sunHeight, 16) + ambientBrightness, 0, 1);
        }


        /// <summary>
        /// returns true if the planet time is closer to dawn than dusk.
        /// </summary>
        public bool isDawn()
        {
            return dayNightPercent < 0.5F;
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
            updateDayNightCycle();
            CollisionHandler.collideEntities(entities);
            tickEntities();
            tickProjectiles();
            CollisionHandler.testProjectilesAgainstEntities(entities, projectiles);
            tickVFX();

            if (GameSettings.drawHitboxes)
            {
                HitboxRenderer.addAllHitboxesToBeRendered(worldColliders, entities, vfxList);
            }
        }

        private void updateDayNightCycle()
        {
            dayNightTicks++;

            //finished a day night cycle
            if(dayNightTicks >= totalDayNightTicks)
            {
                dayNightTicks = 0;
            }

            dayNightPercent = MathUtil.normalizeClamped(0, totalDayNightTicks, dayNightTicks);
            horizonColor = horizonColorDawn.mix(horizonColorDusk, MathUtil.normalizeClamped(0.25F, 0.75F, dayNightPercent));
            sunColor = sunColorDawn.mix(sunColorDusk, MathUtil.normalizeClamped(0.25F, 0.75F, dayNightPercent));
            sunAngle = MathUtil.radians(dayNightPercent * 360.0F) - MathUtil.radians(90.0F);
            sunDirection = new Vector3(MathF.Cos(sunAngle), MathF.Sin(sunAngle), 0.0F).Normalized();
            sunHeight = (MathF.Sin(sunAngle) + 1) * 0.5F;
            tickMoons();
        }


        /*Called every frame. Should not do any heavy computation, only for preparing certain things for rendering. E.G: correcting models.*/
        public void onFrame()
        {
            foreach (Entity ent in entities.Values)
            {
                ent.onFrame();
            }
        }

        private void tickProjectiles()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                Entity entAt = projectiles.ElementAt(i);
                if (entAt == null)
                {
                    projectiles.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (entAt.getIsMarkedForRemoval())
                {
                    entAt.setCurrentPlanet(null);
                    projectiles.RemoveAt(i);
                    i--;
                    continue;
                }
                entAt.preTick();
                entAt.onTick();
                entAt.postTick();

                if (entAt.getPosition().Y < fallPlaneHeight)
                {
                    entAt.setPosition(fallPlaneRespawnPos);
                }

                CollisionHandler.tryToMoveObject(entAt, worldColliders);

                if (entAt.getHasModel())
                {
                    entAt.getEntityModel().onTick();
                    entAt.getEntityModel().sendRenderRequest();
                }
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

                if(entAt.getPosition().Y < fallPlaneHeight)
                {
                    entAt.setPosition(fallPlaneRespawnPos);
                }

                if (entAt.getIsPlayer() && GameSettings.noclip)
                {
                    entAt.getBoxHandle().offset(entAt.getVelocity());
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
                VFX vfx = vfxList.ElementAt(i);
                if (vfx == null)
                {
                    vfxList.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (!vfx.exists())
                {
                    vfxList.RemoveAt(i);
                    i--;
                    continue;
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

            if(GameSettings.entityLabels)
            {
                for (int i = 0; i < debugLabelList.Count; i++)
                {
                    VFXMovingText3D vfx = debugLabelList.ElementAt(i);
                    if (vfx == null)
                    {
                        debugLabelList.Remove(vfx);
                        i--;
                    }
                    else if (!vfx.exists())
                    {
                        debugLabelList.Remove(debugLabelList.ElementAt(i));
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
            SoundManager.playSoundAt("explosion", loc, 3.572F, 1.2F - (float)GameInstance.rand.NextDouble() * 0.2F);
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

        private void setDrawDistanceAndFog(float dist)
        {
            drawDistance = Math.Clamp(dist, 0, GameSettings.maxDrawDistance);
            fogStart = drawDistance / 16;
            fogEnd = drawDistance - 1.0F;
        }

        public float getFogStart()
        {
            return fogStart;
        }

        public float getFogEnd()
        {
            return fogEnd;
        }

        public float getDrawDistance()
        {
            return drawDistance;
        }

        public void spawnEntityInWorld(Entity theEntity)
        {
            if(theEntity.getIsProjectile())
            {
                projectiles.Add(theEntity);
            }
            else
            {
                entities.Add(entityIDItterator++, theEntity);
                if (GameSettings.entityLabels)
                {
                    addDebugLabel(new VFXMovingText3D(theEntity, "debugLabel", "Arial_Shadow", "Entity: " + (entityIDItterator - 1).ToString(), new Vector3(0, 1, 0), 2.0F, CustomColor.white));
                }
            }
        }

        public void spawnVFXInWorld(VFX vfx)
        {
            vfxList.Add(vfx);
        }

        public void addDebugLabel(VFXMovingText3D label)
        {
            debugLabelList.Add(label);
        }

        public void removeLabelFromObject(PositionalObject obj)
        {
            foreach(VFXMovingText3D v in debugLabelList)
            {
                if(v.parent == obj)
                {
                    v.ceaseToExist();
                }
            }
        }

        public int getEntityCount()
        {
            return entities.Count;
        }
        public int getVFXCount()
        {
            return vfxList.Count;
        }

        public int getProjectileCount()
        {
            return projectiles.Count;
        }
        public void onLeavingPlanet()
        {

        }

        public Random rand { get => this.random; }
    }
}
