using OpenTK;
using RabbetGameEngine.Models;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine.VFX
{
    /*A static class for cleaning up some code involving VFX. Calls can be made to the functions here to create or 
      change certain VFX*/
    public static class VFXUtil
    {
        //temp
        private static List<PointCloudModel > debugPointClouds = new List<PointCloudModel>();
        public static void doExplosionEffect(Planet planet, Vector3 location, float radius, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXPointCloud smoke = new VFXPointCloud(location, CustomColor.darkGrey, true, false, 4F, 0.15F, 0.8F);

            VFXPointCloud sparks = new VFXPointCloud(location, CustomColor.orange,  false, false, 0.75F, 0.05F, 1);

            VFXPointCloud explosion1 = new VFXPointCloud(location, CustomColor.flame, false, false, 0.2F, 0.125F, 1);

            VFXPointCloud explosion2 = new VFXPointCloud(location, CustomColor.ember,  false, true, 0.125F, 0.15F, 1);

            smoke.constructRandomPointCloudModel((32 - (int)radius / 2) + (int)(radius / 2 * radius / 2), radius / 16, true);
            sparks.constructRandomPointCloudModel((16 - (int)radius / 4) + (int)(radius / 4 * radius / 4), radius / 16, true);
            explosion1.constructRandomPointCloudModel((256 - (int)radius / 2) + (int)(radius / 2 * radius / 2), radius / 16, false);
            explosion2.constructRandomPointCloudModel((256 - (int)radius / 2) + (int)(radius / 2 * radius / 2), radius / 32, true);
            smoke.setYAccel(0.002572F);
            smoke.setExpansionResistance(0.15F);
            smoke.setExpansionVelocity(2.5F);
            sparks.setYAccel(-sparks.gravity);
            sparks.setExpansionResistance(0.1F);
            sparks.setExpansionVelocity(5.5F);
            explosion1.setExpansionResistance(0.3F);
            explosion1.setExpansionVelocity(3F);
            explosion1.setYAccel(0.004572F);
            explosion2.setExpansionResistance(0.3F);
            explosion2.setExpansionVelocity(3F);
            explosion2.setYAccel(0.005472F);

            planet.spawnVFXInWorld(smoke);
            planet.spawnVFXInWorld(sparks);
            planet.spawnVFXInWorld(explosion1);
            planet.spawnVFXInWorld(explosion2);
        }

        public static void doSmallSmokePuffEffect(Planet planet, Vector3 location, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXPointCloud smoke = new VFXPointCloud(location, CustomColor.darkGrey, true, false, 4F, 0.05F, 0.7F);
            smoke.constructRandomPointCloudModel(7, 0.05F, true);
            smoke.setPitch(pitch);
            smoke.setYaw(yaw);
            smoke.setRoll(roll);
            smoke.setYAccel(0.001572F);
            smoke.setExpansionResistance(0.2572F);
            smoke.setExpansionVelocity(2.5F);
            planet.spawnVFXInWorld(smoke);
        }

        public static void doDebugSmokeEffect(Planet planet)
        {
            VFXPointCloud smoke = new VFXPointCloud(new Vector3(0, 2F, 0), CustomColor.darkGrey, true, false, 4F, 0.5F, 0.7F);
            smoke.constructRandomPointCloudModel(15, 0.5F, true);
            smoke.setExpansionResistance(0.05F);
            smoke.setExpansionVelocity(0.5F);
            planet.spawnVFXInWorld(smoke);
        }
        public static void createDebugVoxels()
        {
            float diam = ((float)Math.Sqrt(2)) / 4;
            for (int i = 0; i < 256; i++)//layers
            {
                PointParticle[] points = new PointParticle[64 * 64];
                for (int j = 0; j < 64 * 64; j++)
                {
                    points[j].pos = new Vector3(j / 64 * diam / 2, (float)i * diam / 2, j % 64 * diam / 2);
                    points[j].radius = diam / 2;
                    points[j].color = CustomColor.darkGrey.toNormalVec4() * (1.0F - (float)GameInstance.rand.NextDouble() * 0.2F);
                    points[j].aoc = 0.0F;
                }
                debugPointClouds.Add(new PointCloudModel(points, null));
            }
        }
        public static void doDebugVoxels()
        {
            foreach(PointCloudModel p in debugPointClouds)
            { 
                Renderer.requestRender(p, false, false);
            }
        }

        public static void doDebugSnowEffect(Planet planet)
        {

        }
    }
}
