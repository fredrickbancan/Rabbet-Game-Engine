using OpenTK.Mathematics;

using System;

namespace RabbetGameEngine
{
    public class Sky
    {
        public static readonly float minSkyLuminosity = 0.0175F;
        public static readonly float maxSkyLuminosity = 1.25F;
        private Color fogColor;
        private Color horizonColorAmbient;
        private Color horizonColor;
        private Color skyColor;
        private Color sunColor;
        private Vector3 sunDirection;
        private float sunAngle = 0;
        private int totalStars = 0;
        private int totalMoons = 0;
        private PointCloudModel stars = null;
        private SkyMoon[] moons = null;

        /// <summary>
        /// 0 at midnight, 1 at midday
        /// </summary>
        private float sunHeight = 0;

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

        public Sky(Random rand)
        {
            horizonColor = Color.lightOrange.reduceVibrancy(-1.5F);
            horizonColorAmbient = Color.dusk.reduceVibrancy(-0.5F);
            skyColor = Color.skyBlue.reduceVibrancy(-1.5F);
            fogColor = Color.lightGrey;
            sunColor = Color.lightOrange.reduceVibrancy(-0.5F);
            totalDayNightTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(dayNightCycleMinutes * 60);
            dayNightTicks = (int)((float)(totalDayNightTicks / 4) * 2F);
            buildStars(rand);
            buildMoons(rand);
        }

        public SkyMoon[] getMoons()
        {
            return moons;
        }

        private void buildMoons(Random rand)
        {
            totalMoons = rand.Next(1, 4);
            moons = new SkyMoon[moonCount];
            float moonColorStrength = 0.082F;
            float maxMoonRadius = 0.025F;
            float minMoonRadius = 0.015F;
            float spacing = 1.0F / (float)moonCount * 0.2F;
            for (int i = 0; i < moonCount; i++)
            {
                float orbitScale = 1 - i * spacing;
                moons[i] = new SkyMoon(
                    new Vector3((0.9999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.9999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.9999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F).Normalized() * orbitScale,
                    new Vector2((0.4999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F, (0.4999F - (float)rand.NextDouble() * 0.9999F) + 0.0001F).Normalized(),
                    new Vector4(1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F - (float)rand.NextDouble() * moonColorStrength, 1.0F),
                    (float)rand.NextDouble() * maxMoonRadius + minMoonRadius,
                    (float)rand.NextDouble() * 360.0F,
                    rand.Next(0, SkyMoon.totalMoonTextures),
                    rand.Next(5, 15),
                    orbitScale);
            }
        }

        private void buildStars(Random rand)//must be done before building skybox.
        {
            totalStars = rand.Next(4000, 4501);
            PointParticle[] points = new PointParticle[totalStars];
            float starColorStrength = 0.2F;
            float maxStarRadius = 0.015F;
            float minStarRadius = 0.001F;
            float minStarLuminance = maxSkyLuminosity * 0.45F;
            float maxStarLuminance = maxSkyLuminosity * 0.82F;
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

                points[i] = new PointParticle(pos.Normalized(), color, radius, false);
            }
            Vector3 galaxyPlaneNormal = new Vector3(0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble(), 0.5F - (float)rand.NextDouble()).Normalized();
            float galaxyClusterStrength = 0.85F;

            float distToPlane;
            for (int i = 0; i < totalStars / 1.5F; i++)
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

        public Vector3 getSunDirection()
        {
            return sunDirection;
        }

        public Vector3 getSunColor()
        {
            return sunColor.toNormalVec3();
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

        /// <summary>
        /// returns true if the planet time is closer to dawn than dusk.
        /// </summary>
        public bool isDawn()
        {
            return dayNightPercent < 0.5F;
        }


        public void onTick(float timeStep)
        {
            Profiler.startSection("skyUpdate");
            Profiler.startTickSection("skyUpdate");
            updateDayNightCycle(timeStep);
            Profiler.endCurrentTickSection();
            Profiler.endCurrentSection();
        }


        private void updateDayNightCycle(float ts)
        {
            dayNightTicks++;

            //finished a day night cycle
            if (dayNightTicks >= totalDayNightTicks)
            {
                dayNightTicks = 0;
            }

            dayNightPercent = MathUtil.normalizeClamped(0, totalDayNightTicks, dayNightTicks);
            sunAngle = MathUtil.radians(dayNightPercent * 360.0F) - MathUtil.radians(90.0F);
            sunDirection = new Vector3(MathF.Cos(sunAngle), MathF.Sin(sunAngle), 0.0F).Normalized();
            sunHeight = (MathF.Sin(sunAngle) + 1) * 0.5F;

            for (int i = 0; i < moonCount; i++)
            {
                moons[i].onTick(ts);
            }
        }

        public int moonCount
        {
            get { return totalMoons; }
        }

        public int starCount
        {
            get { return totalStars; }
        }

    }
}
