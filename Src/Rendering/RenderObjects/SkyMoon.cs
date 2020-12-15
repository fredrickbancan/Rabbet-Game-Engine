using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    public class SkyMoon
    {
        public static readonly int totalMoonTextures = 4;
        public static readonly int moonTextureSheetWidthHeight = 1024;
        public static readonly int moonTextureWidthHeight = 256;
        public Sprite3D sprite;
        private int maxCycleTicks = 0;
        private int cycleTicks = 0;
        public Vector3 skyDirection;
        public Vector2 orbitDirection;
        private float cyclePercent = 0;
        private float skyAngle = 0;
        private float angleOffset = 0;
        private float orbitScale = 0;
        public SkyMoon(Vector3 initialPos, Vector2 orbitDirectionXZ, Vector4 color, float radius, float angleOffset, int textureIndex, int cycleMinutes, float orbitScale)
        {
            this.orbitScale = orbitScale;
            this.orbitDirection = orbitDirectionXZ;
            this.angleOffset = angleOffset;
            skyDirection = initialPos;
            maxCycleTicks = (int)TicksAndFrames.getNumOfTicksForSeconds(cycleMinutes * 60);
            sprite.position = initialPos;
            sprite.scale = new Vector3(radius, 1, 1) * orbitScale;
            sprite.color = color;
            int indNumWidthHeight = moonTextureSheetWidthHeight / moonTextureWidthHeight;
            float texFract = MathUtil.normalize(0, moonTextureSheetWidthHeight, moonTextureWidthHeight);
            float uMin = texFract * (textureIndex % indNumWidthHeight);
            float uMax = uMin + texFract;
            float vMin = texFract * (textureIndex / indNumWidthHeight);
            float vMax = vMin + texFract;
            sprite.uvMinMax = new Vector4(uMin, vMin, uMax, vMax);
        }
        

        public void onTick()
        {
            cycleTicks++;
            if(cycleTicks >= maxCycleTicks)
            {
                cycleTicks = 0;
            }
            cyclePercent = MathUtil.normalizeClamped(0, maxCycleTicks, cycleTicks);
            skyAngle = MathUtil.radians(cyclePercent * 360.0F) + angleOffset;
            float rotX = MathF.Cos(skyAngle);
            float rotY = MathF.Sin(skyAngle);
            float rotZ = -MathF.Cos(skyAngle);
            skyDirection = new Vector3(rotX * orbitDirection.X, rotY, rotZ * orbitDirection.Y).Normalized();
            sprite.position = skyDirection * orbitScale;
        }
    }
}
