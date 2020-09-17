using OpenTK;
using RabbetGameEngine.Physics;
using System;
namespace RabbetGameEngine
{
    class EntityProjectile : Entity, IDisposable
    {
        protected float maxExistedTicks;

        public EntityProjectile(Vector3 pos, Vector3 direction, float initialVelocity = 2.5F, float maxLivingSeconds = 20) : base(pos)
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
