using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo
{
    class EntityTankProjectile : EntityProjectile
    {
        public EntityTankProjectile(Vector3D pos, Vector3D direction, double barrelPitch, double yaw) : base(pos, direction)
        {
            this.yaw = yaw - 90;
            this.pitch = barrelPitch;
            this.entityModel = new EntityTankProjectileModel(this, direction);
            this.hasModel = true;
        }

        public override void onTick()
        {
            base.onTick();
        }

        public override void onCollideWithGround()
        {
            currentPlanet.createImpulseAtLocation(pos);
            base.onCollideWithGround();//do last
        }
    }
}
