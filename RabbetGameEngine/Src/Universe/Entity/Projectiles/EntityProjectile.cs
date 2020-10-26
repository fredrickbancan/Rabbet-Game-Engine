using OpenTK;
using RabbetGameEngine.Physics;
using System;
namespace RabbetGameEngine
{
    class EntityProjectile : Entity, IDisposable
    {
        protected float maxExistedTicks;

        public EntityProjectile(Planet p, Vector3 pos, Vector3 direction, float initialVelocity = 2.5F, float maxLivingSeconds = 20) : base(p, pos)
        {
            airResistance = 0.001F;
            velocity += direction * initialVelocity;
            this.maxExistedTicks = TicksAndFrames.getNumOfTicksForSeconds(maxLivingSeconds);
            this.collider = new AABB(new Vector3(-0.125F, -0.125F, -0.125F), new Vector3(0.125F, 0.125F, 0.125F), this);
            this.hasCollider = true;
        }

        public override void onTick()
        {
            base.onTick();

            //rotate to match direction


            
            if(hasCollided)
            {
                onCollide();
            }

            //do last
            if(existedTicks > maxExistedTicks)//delete this projectile if it has reached its limit of existance time
            {
                Dispose();
            }
        }

        public virtual void onCollide()
        {
            Dispose();
        }

        public override void onCollideWithEntity(Entity ent)
        {
            base.onCollideWithEntity(ent);
            onCollide();
        }

        public void Dispose()
        {
            ceaseToExist();
            GC.SuppressFinalize(this);
        }
    }
}
