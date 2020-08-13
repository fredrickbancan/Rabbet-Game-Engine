using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    class EntityProjectile : Entity
    {
        protected double maxExistedTicks;

        public EntityProjectile(Vector3D pos, Vector3D direction, double initialVelocity = 2.5D, double maxLivingSeconds = 20) : base(pos)
        {
            velocity += direction * initialVelocity;
            this.maxExistedTicks = TicksAndFps.getNumOfTicksForSeconds(maxLivingSeconds);
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
                ceaseToExist();
            }
        }

        public virtual void onCollideWithGround()
        {
            ceaseToExist();
        }
    }
}
