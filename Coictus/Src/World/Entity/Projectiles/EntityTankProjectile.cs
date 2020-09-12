
using Coictus.Models;
using OpenTK;
using System;
namespace Coictus
{
    class EntityTankProjectile : EntityProjectile
    {
        public EntityTankProjectile(Vector3 pos, Vector3 direction, float barrelPitch, float initialYaw) : base(pos, direction)
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
            Vector3 velocityNormalVec = Vector3.Normalize(velocity);
            yaw = (float)((Math.Atan2(velocityNormalVec.Z, velocityNormalVec.X) * 180) / Math.PI );
            pitch = (float)(Math.Atan2(velocityNormalVec.Y, velocityNormalVec.Xz.Length) * (180D / Math.PI));
        }

        public override void ceaseToExist()
        {
            currentPlanet.doExplosionAt(pos);
            base.ceaseToExist();
        }
        public override void applyCollision(Vector3 direction, float overlap)
        {
            base.applyCollision(direction, overlap);
            Dispose();
        }
    }
}
