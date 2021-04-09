using Medallion;
using OpenTK.Mathematics;
using RabbetGameEngine.Debugging;
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
    public class World
    {
        public static readonly float minSkyLuminosity = 0.03F;
        public static readonly float maxSkyLuminosity = 1.33F;
        private Color fogColor;
        private Color horizonColorAmbient;
        private Color horizonColor;
        private Color skyColor;
        private Color skyAmbientColor;
        private Color sunColor;
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
        private EntityPlayer playerEntity = null;
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();//the int is the given ID for the entity
        public List<VFX> vfxList = new List<VFX>();
        public List<Entity> projectiles = new List<Entity>();
        public List<VFXMovingText3D> debugLabelList = new List<VFXMovingText3D>();
        public List<AABB> worldColliders = new List<AABB>();//list of colliders with no parent, ie, walls.
        private string groundTextureName = "sand";   
        private static readonly Vector3 fallPlaneRespawnPos = new Vector3(0, 128, 0);
        private static readonly float fallPlaneHeight = -10.0F;
        private Random random;
        private float fogStart;
        private float fogEnd;
        private float drawDistance = 0;
        public World(long seed)
        {
            random = Rand.CreateJavaRandom(seed);
            horizonColor = Color.lightOrange.reduceVibrancy(-1.5F);
            horizonColorAmbient = Color.dusk.reduceVibrancy(-0.5F);
            skyAmbientColor = Color.skyBlue.reduceVibrancy(-1F).setBrightPercent(0.5F);
            skyColor = Color.skyBlue.reduceVibrancy(-1.5F);
            fogColor = Color.lightGrey;
            sunColor = Color.lightOrange.reduceVibrancy(-0.5F);
            totalDayNightTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(dayNightCycleMinutes * 60);
            dayNightTicks =(int) ((float)(totalDayNightTicks / 4) * 2F);//setting to sunset
            setDrawDistanceAndFog(1500.0F);
            buildMoons();
            buildStars();
            SkyboxRenderer.setSkyboxToDraw(this);
            generateWorld();
        }

        private void buildMoons()
        {
            totalMoons = rand.Next(1, 4);
            moons = new SkyMoon[totalMoons];
            float moonColorStrength = 0.082F;
            float maxMoonRadius = 0.025F;
            float minMoonRadius = 0.015F;
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
                    rand.Next(5,15),
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
            totalStars = rand.Next(4000,4501);
            PointParticle[] points = new PointParticle[totalStars];
            float starColorStrength = 0.2F;
            float maxStarRadius = 0.01F;
            float minStarRadius = 0.0025F;
            float minStarLuminance = maxSkyLuminosity * 0.35F;
            float maxStarLuminance = maxSkyLuminosity * 0.72F;
            float luminance;
            float radius;
            Vector3 pos;
            Vector4 color;
            for (int i = 0; i < totalStars; i++)
            {
                luminance = MathUtil.lerpF(minStarLuminance, maxStarLuminance, (float)rand.NextDouble());
                pos = new Vector3(0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble());
                radius = (float)rand.NextDouble() * maxStarRadius + minStarRadius;
                color = new Vector4(luminance - (float)rand.NextDouble() * starColorStrength, luminance - (float)rand.NextDouble() * starColorStrength, luminance - (float)rand.NextDouble() * starColorStrength, 1.0F);
                
                points[i] = new PointParticle( pos.Normalized(), color, radius, false);
            }
            Vector3 galaxyPlaneNormal = new Vector3(0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble()).Normalized();
            float galaxyClusterStrength = 0.85F;

            float distToPlane;
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
            //TODO: make fog color match horizon in direction player is looking.
            return fogColor.toNormalVec3();
        }

        public float getSkyLuminosity()
        {
            float sunHeight = (sunDirection.Y + 1.0F) * 0.5F;
            float nightFactor = MathUtil.clamp(1.0F - sunHeight, 0.05F, 1.0F);//0.05 to 1.0 depending how close the sun is to being down
            return MathUtil.lerpF(minSkyLuminosity, maxSkyLuminosity, 1.0F - System.MathF.Pow(nightFactor, 0.15F));
        }

        public Vector3 getHorizonColor()
        { 
            return horizonColor.toNormalVec3();
        }

        public Vector3 getHorizonAmbientColor()
        {
            return horizonColorAmbient.toNormalVec3();
        }

        public Vector3 getSkyColor()
        {
            return skyColor.toNormalVec3();
        }

        public Vector3 getSkyAmbientColor()
        {
            return skyAmbientColor.toNormalVec3();
        }
        public Vector3 getSunDirection()
        {
            return sunDirection;
        }
    
        public Vector3 getSunColor()
        {
            return sunColor.reduceVibrancy(sunHeight - 0.5F).toNormalVec3();
        }

        public string get24HourTimeString()
        {
            return ((int)(24.0F * dayNightPercent)).ToString("00.#") + ":" + ((int)(60.0F * (dayNightPercent * 24.0F)) % 60).ToString("00.#");
        }
        public string get12HourTimeString()
        {
            int twelveHour = (int)(24.0F * dayNightPercent) % 12;
            twelveHour = twelveHour < 1 ? 12 : twelveHour;//make range from 12 - 12
            return twelveHour.ToString("0.#") + ":" + ((int)(60.0F * (dayNightPercent * 24.0F)) % 60).ToString("00.#") + (isDawn() ? " am" : " pm");
        }
        public float getGlobalBrightness()
        {
            return MathHelper.Clamp(MathF.Pow(sunHeight, 4) + ambientBrightness, 0, 1);
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
            Model[] unbatchedGroundQuads = new Model[4101];//all ground and wall quads, walls are divided into 4 quads each.

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


            Model batchedGround = TriangleCombiner.combineData(unbatchedGroundQuads);

           // Renderer.addStaticDrawTriangles("ground", groundTextureName, batchedGround);

            //adding world colliders
            this.addWorldAABB(new AABB(new Vector3(-640, -2, -640), new Vector3(640, 0, 640)));//AABB for ground
            this.addWorldAABB(new AABB(new Vector3(-1, 0, -1), new Vector3(1, 1, 1)));//2x1x2 lump in middle of playground
        }

        public void onVideoSettingsChanged()
        {
            setDrawDistanceAndFog(GameSettings.maxDrawDistance.floatValue);
        }

        public void onTick()
        {
            Profiler.startSection("skyUpdate");
            Profiler.startTickSection("skyUpdate");
            updateDayNightCycle();
            Profiler.endStartSection("entCol");
            Profiler.endStartTickSection("entCol");
            CollisionHandler.collideEntities(entities);
            Profiler.endStartSection("entTick");
            Profiler.endStartTickSection("entTick");
            tickEntities();
            Profiler.endStartSection("projTick");
            Profiler.endStartTickSection("projTick");
            tickProjectiles();
            Profiler.endStartSection("projColEnt");
            Profiler.endStartTickSection("projColEnt");
            CollisionHandler.testProjectilesAgainstEntities(entities, projectiles);
            Profiler.endStartSection("vfxTick");
            Profiler.endStartTickSection("vfxTick");
            tickVFX();
            Profiler.endCurrentSection();
            Profiler.endCurrentTickSection();
        }

        public void onRenderUpdate()
        {
            foreach(Entity ent in entities.Values)
            {
                if(ent.getHasModel())
                {
                    ent.getEntityModel().sendRenderRequest();
                }
            }

            foreach(Entity p in projectiles)
            {
                if (p.getHasModel())
                {
                    p.getEntityModel().sendRenderRequest();
                }
            }

            foreach(VFX v in vfxList)
            {
                v.sendRenderRequest();
            }

            if(GameSettings.entityLabels)
            foreach(VFXMovingText3D label in debugLabelList)
            {
                label.sendRenderRequest();
            }
            
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
                Profiler.startSection("projWorldCol");
                Profiler.startTickSection("projWorldCol");
                CollisionHandler.tryToMoveObject(entAt, worldColliders);
                Profiler.endCurrentTickSection();
                Profiler.endCurrentSection();

                if (entAt.getHasModel())
                {
                    entAt.getEntityModel().onTick();
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
                    if (entAt.getIsPlayer()) playerEntity = null;
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
                    Profiler.startSection("entWorldCol");
                    Profiler.startTickSection("entWorldCol");
                    CollisionHandler.tryToMoveObject(entAt, worldColliders);
                    Profiler.endCurrentTickSection();
                    Profiler.endCurrentSection();
                }

                if(entAt.getHasModel())
                {
                    entAt.getEntityModel().onTick();
                }
            }
        }

        private void tickVFX()
        {
            for (int i = 0; i < vfxList.Count; i++)
            {
                VFX vfx = vfxList.ElementAt(i);
                if (!vfx.exists())
                {
                    vfxList.RemoveAt(i);
                    i--;
                    continue;
                }
                if (vfx.isTickable())
                {
                    vfx.preTick();
                    vfx.onTick();
                    vfx.postTick();
                    if (vfx.isMovable())
                    {
                        Profiler.startSection("vfxWorldCol");
                        Profiler.startTickSection("vfxWorldCol");
                        CollisionHandler.tryToMoveObject(vfx, worldColliders);
                        Profiler.endCurrentTickSection();
                        Profiler.endCurrentSection();
                    }
                }
            }

            if(GameSettings.entityLabels)
            {
                for (int i = 0; i < debugLabelList.Count; i++)
                {
                    VFXMovingText3D label = debugLabelList.ElementAt(i);
                    if (label == null)
                    {
                        debugLabelList.Remove(label);
                        i--;
                    }
                    else if (!label.exists())
                    {
                        debugLabelList.Remove(debugLabelList.ElementAt(i));
                        i--;
                    }
                    else
                    {
                        label.onTick();
                    }
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
            drawDistance = dist;
            fogStart = Math.Clamp(dist, 0, GameSettings.maxDrawDistance.floatValue) / 16;
            fogEnd = Math.Clamp(dist, 0, GameSettings.maxDrawDistance.floatValue) - 1.0F;
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
                if (theEntity.getIsPlayer()) playerEntity = (EntityPlayer)theEntity;
                entities.Add(entityIDItterator++, theEntity);
                if (GameSettings.entityLabels)
                {
                    addDebugLabel(new VFXMovingText3D(theEntity, "debugLabel", "arial", "Entity: " + (entityIDItterator - 1).ToString(), new Vector3(0, 1, 0), 2.0F, Color.white));
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

        public Matrix4 getViewMatrix()
        {
            if (playerEntity != null) return playerEntity.getViewMatrix();
            return Matrix4.LookAt(new Vector3(0,2,0), new Vector3(0,0,-1), new Vector3(0,1,0));
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
