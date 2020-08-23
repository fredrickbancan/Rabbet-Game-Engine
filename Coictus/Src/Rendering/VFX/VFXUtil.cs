using Coictus.FredsMath;

namespace Coictus.VFX
{
    /*A static class for cleaning up some code involving VFX. Calls can be made to the functions here to create or 
      change certain VFX*/
    public static class VFXUtil
    {
        public static void doExplosionEffect(World planet, Vector3D location, float radius, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXBase smoke = new VFXPointParticles(location, ColourF.darkGrey, (25 - (int)radius / 2) + (int)(radius / 2 * radius / 2), radius / 15, 0.15F, true, false, 2F, 0.8F);

            VFXBase sparks = new VFXPointParticles(location, ColourF.orange, (12 - (int)radius / 4) + (int)(radius / 4 * radius / 4), radius / 15, 0.05F, true, false, 0.75F);

            VFXBase explosion1 = new VFXPointParticles(location, ColourF.flame, (128 - (int)radius/2) + (int)(radius/2*radius/2), radius / 15, 0.125F, false, false, 0.2F);

            VFXBase explosion2 = new VFXPointParticles(location, ColourF.ember, (128 - (int)radius/2) + (int)(radius/2*radius/2), radius / 20, 0.125F, true, false, 0.15F);

            smoke.setYAccel(0.003572D);
            smoke.setExpansionResistance(0.2F);
            smoke.setExpansionVelocity(2.5F);
            sparks.setYAccel(-sparks.gravity);
            sparks.setExpansionResistance(0.1F);
            sparks.setExpansionVelocity(5.5F);
            explosion1.setExpansionResistance(0.3F);
            explosion1.setExpansionVelocity(3F);
            explosion1.setYAccel(0.003572D);
            explosion2.setExpansionResistance(0.3F);
            explosion2.setExpansionVelocity(3F);
            explosion2.setYAccel(0.003472D);

            planet.spawnVFXInWorld(smoke);
            planet.spawnVFXInWorld(sparks);
            planet.spawnVFXInWorld(explosion1);
            planet.spawnVFXInWorld(explosion2);
        }

        public static void doSmallSmokePuffEffect(World planet, Vector3D location, float pitch = 0, float yaw = -90, float roll = 0)
        {
            VFXBase smoke = new VFXPointParticles(location, ColourF.darkGrey, 7, 0.05F, 0.05F, true, false, 2F, 0.8F);

            smoke.setPitch(pitch);
            smoke.setYaw(yaw);
            smoke.setRoll(roll);

            smoke.setExpansionZModifyer(0.0F);

            smoke.setYAccel(0.003572D);
            smoke.setExpansionResistance(0.3572F);
            smoke.setExpansionVelocity(2.5F);
            planet.spawnVFXInWorld(smoke);
        }

        public static void doDebugSmokeEffect(World planet)
        {
            VFXBase smoke = new VFXPointParticles(Vector3D.zero + 2, ColourF.darkGrey, 5, 0.5F, 0.5F, true, false, 2F, 0.5F);
            smoke.setExpansionResistance(0.05F);
            smoke.setExpansionVelocity(0.5F);
            planet.spawnVFXInWorld(smoke);
        }
    }
}
