using OpenTK;
using System.Drawing;

namespace RabbetGameEngine.VFX
{
    /*A static class for cleaning up some code involving VFX. Calls can be made to the functions here to create or 
      change certain VFX*/
    public static class VFXUtil
    {
        public static void doExplosionEffect(World planet, Vector3 location, float radius, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXBase smoke = new VFXPointParticles(location, CustomColor.darkGrey, (25 - (int)radius / 2) + (int)(radius / 2 * radius / 2), radius / 15, 0.15F, true, false, 2F, 0.8F);

            VFXBase sparks = new VFXPointParticles(location, CustomColor.orange, (12 - (int)radius / 4) + (int)(radius / 4 * radius / 4), radius / 15, 0.05F, true, false, 0.75F);

            VFXBase explosion1 = new VFXPointParticles(location, CustomColor.flame, (128 - (int)radius/2) + (int)(radius/2*radius/2), radius / 15, 0.125F, false, false, 0.2F);

            VFXBase explosion2 = new VFXPointParticles(location, CustomColor.ember, (128 - (int)radius/2) + (int)(radius/2*radius/2), radius / 20, 0.125F, true, false, 0.15F);

            smoke.setYAccel(0.003572F);
            smoke.setExpansionResistance(0.2F);
            smoke.setExpansionVelocity(2.5F);
            sparks.setYAccel(-sparks.gravity);
            sparks.setExpansionResistance(0.1F);
            sparks.setExpansionVelocity(5.5F);
            explosion1.setExpansionResistance(0.3F);
            explosion1.setExpansionVelocity(3F);
            explosion1.setYAccel(0.003572F);
            explosion2.setExpansionResistance(0.3F);
            explosion2.setExpansionVelocity(3F);
            explosion2.setYAccel(0.003472F);

            planet.spawnVFXInWorld(smoke);
            planet.spawnVFXInWorld(sparks);
            planet.spawnVFXInWorld(explosion1);
            planet.spawnVFXInWorld(explosion2);
        }

        public static void doSmallSmokePuffEffect(World planet, Vector3 location, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXBase smoke = new VFXPointParticles(location, CustomColor.darkGrey, 7, 0.05F, 0.05F, true, false, .5F, 0.8F);

            smoke.setPitch(pitch);
            smoke.setYaw(yaw);
            smoke.setRoll(roll);

            smoke.setExpansionZModifyer(0.0F);

            smoke.setYAccel(0.003572F);
            smoke.setExpansionResistance(0.3572F);
            smoke.setExpansionVelocity(2.5F);
            planet.spawnVFXInWorld(smoke);
        }

        public static void doDebugSmokeEffect(World planet)
        {
            VFXBase smoke = new VFXPointParticles(new Vector3(0, 2F, 0), new CustomColor(Color.DarkGray), 5, 0.5F, 0.5F, true, false, 2F, 0.5F);
            smoke.setExpansionResistance(0.05F);
            smoke.setExpansionVelocity(0.5F);
            planet.spawnVFXInWorld(smoke);
        }
    }
}
