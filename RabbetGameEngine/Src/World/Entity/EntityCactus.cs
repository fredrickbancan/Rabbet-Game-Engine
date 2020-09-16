using RabbetGameEngine.Models;
using OpenTK;
using System;
namespace RabbetGameEngine
{
    public class EntityCactus : EntityLiving
    {
        private bool turn = true;
        private static Random rand = new Random();
        public EntityCactus() : base()
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this), 1);
            yaw = (float)rand.NextDouble() * 360;
            walkFowards();
        }
        public EntityCactus(Vector3 pos) : base(pos)
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this), 1);
            yaw = (float)rand.NextDouble() * 360;
            walkFowards();
        }

        public override void onTick()
        {
            if(rand.Next(0, 25) == 1)
            {
                turn = !turn;
            }

            if (turn) rotateYaw(-3.0F); else rotateYaw(3.0F);

            walkFowards();

            if (rand.Next(0, 50) == 1)
            {
                jump();
            }
            base.onTick();//do last
        }
        public override void onCollidedBy(PositionalObject other)
        {
            /*TEMPORARY, for arcade effects*/
            if (other is EntityProjectile )
            {
                GameInstance.onDirectHit();
                if (!getIsGrounded())
                {
                    GameInstance.onAirShot();
                }
            }
            base.onCollidedBy(other);
        }
    }
}
