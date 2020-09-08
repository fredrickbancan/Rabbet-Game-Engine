
using Coictus.Models;
using OpenTK;
using System;
namespace Coictus
{
    class EntityTankProjectile : EntityProjectile
    {
        public EntityTankProjectile(Vector3d pos, Vector3d direction, double barrelPitch, double initialYaw) : base(pos, direction)
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
            Vector3d velocityNormalVec = Vector3d.Normalize(velocity);
            yaw = (Math.Atan2(velocityNormalVec.Z, velocityNormalVec.X) * 180) / Math.PI ;
            pitch = Math.Atan2(velocityNormalVec.Y, velocityNormalVec.Xz.Length) * (180D / Math.PI);
        }

        public override void ceaseToExist()
        {
            currentPlanet.doExplosionAt(pos);
            base.ceaseToExist();
        }
        public override void applyCollision(Vector3d direction, double overlap)
        {
            base.applyCollision(direction, overlap);
            Dispose();
        }
    }
}
