using OpenTK;
using System;

namespace Coictus
{
    class EntityProjectile : Entity, IDisposable
    {
        protected double maxExistedTicks;

        public EntityProjectile(Vector3d pos, Vector3d direction, double initialVelocity = 2.5D, double maxLivingSeconds = 2) : base(pos)
        {
            airResistance = 0.001F;
            velocity += direction * initialVelocity;
            this.maxExistedTicks = TicksAndFps.getNumOfTicksForSeconds(maxLivingSeconds);
            this.setCollider(new SphereCollider(pos, 0.15F, this));
        }

        public override void onTick()
        {
            base.onTick();

            //rotate to match direction


            //do basic collisions
            if(isGrounded)
            {
                onCollideWithGround();
            }

            //do last
            if(existedTicks > maxExistedTicks)//delete this projectile if it has reached its limit of existance time
            {
                Dispose();
            }
        }

        public virtual void onCollideWithGround()
        {
            Dispose();
        }

        

        public void Dispose()
        {
            ceaseToExist();
            GC.SuppressFinalize(this);
        }
    }
}
