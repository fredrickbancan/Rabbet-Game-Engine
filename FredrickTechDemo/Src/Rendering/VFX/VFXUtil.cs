using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo.VFX
{
    /*A static class for cleaning up some code involving VFX. Calls can be made to the functions here to create or 
      change certain VFX*/
    public static class VFXUtil
    {
        public static void doExplosionEffect(Planet planet, Vector3D location, float radius)
        {
            VFXBase smoke = new VFXPointParticles(location, ColourF.darkGrey, 25, radius / 2, 0.15F, true, false, 6F, 2F);
            smoke.addYVelocity(0.1D);
            VFXBase fire = new VFXPointParticles(location, ColourF.orange, 15, radius / 2, 0.15F, true, false, 4F, 0.75F);
            fire.addYVelocity(0.15D);
            smoke.setExpansionDeceleration(0.1F);
            fire.setExpansionDeceleration(0.1F);
            planet.spawnVFXInWorld(smoke);
            planet.spawnVFXInWorld(fire);
            planet.spawnVFXInWorld(new VFXExplosion(location));
        }

        public static void doSmallSmokePuffEffect(Planet planet, Vector3D location)
        {
            VFXBase smoke = new VFXPointParticles(location, ColourF.darkGrey, 7, 0.1F, 0.075F, true, false, 50F, 2F);
            smoke.addYVelocity(0.05D);
            smoke.setExpansionDeceleration(0.2F);
            planet.spawnVFXInWorld(smoke);
        }
    }
}
