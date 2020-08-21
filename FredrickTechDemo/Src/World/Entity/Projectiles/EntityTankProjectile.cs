using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo
{
    class EntityTankProjectile : EntityProjectile
    {
        public EntityTankProjectile(Vector3D pos, Vector3D direction, double barrelPitch, double initialYaw) : base(pos, direction)
        {
            this.yaw = initialYaw - 90;
            this.pitch = barrelPitch;
            this.entityModel = new EntityTankProjectileModel(this);
            this.hasModel = true;
        }

        public override void onTick()
        {
            base.onTick();

            //rotating projectile based on velocity to simulate areodynamics
            Vector3D velocityNormalVec = Vector3D.normalize(velocity);
            yaw = (Math.Atan2(velocityNormalVec.z, velocityNormalVec.x) * 180) / Math.PI ;
            pitch = Math.Atan2(velocityNormalVec.y, velocityNormalVec.horizontalMagnitude()) * (180D / Math.PI);
        }

        public override void ceaseToExist()
        {
            currentPlanet.doExplosionAt(pos);
            base.ceaseToExist();
        }
        public override void applyCollision(Vector3D direction, double overlap)
        {
            base.applyCollision(direction, overlap);
            Dispose();
        }
    }
}
