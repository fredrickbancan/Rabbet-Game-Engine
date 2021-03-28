using OpenTK.Mathematics;
using RabbetGameEngine.Physics;
namespace RabbetGameEngine
{
    public class EntityProjectile : Entity
    {
        protected float maxExistedTicks;

        public EntityProjectile(World p, Vector3 pos, Vector3 direction, float initialVelocity = 2.5F, float maxLivingSeconds = 20) : base(p, pos)
        {
            airResistance = 0.001F;
            velocity += direction * initialVelocity;
            this.maxExistedTicks = TicksAndFrames.getNumOfTicksForSeconds(maxLivingSeconds);
            this.collider = new AABB(new Vector3(-0.125F, -0.125F, -0.125F), new Vector3(0.125F, 0.125F, 0.125F), this);
            this.hasCollider = true;
            this.isProjectile = true;
        }

        public override void onTick()
        {
            base.onTick();
            if(hasCollided)
            {
                onCollide();
            }

            //do last
            if(existedTicks > maxExistedTicks)//delete this projectile if it has reached its limit of existance time
            {
                ceaseToExist();
            }
        }

        public virtual void onCollide()
        {
            ceaseToExist();
        }

        public override void onCollideWithEntity(Entity ent)
        {
            base.onCollideWithEntity(ent);
            onCollide();
        }
    }
}
