using OpenTK;
using RabbetGameEngine.Models;
using RabbetGameEngine.Physics;
using System;
namespace RabbetGameEngine
{
    public class EntityCactus : EntityLiving
    {
        private bool turn = true;
        private static Random rand = new Random();
        public EntityCactus() : base()
        {
            this.walkSpeed = 0.025F;
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this);
            this.hasCollider = true;
            yaw = (float)rand.NextDouble() * 360;
            walkFowards();
        }
        public EntityCactus(Vector3 pos) : base(pos)
        {
            this.walkSpeed = 0.025F;
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.collider = new AABB(new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), this);
            this.hasCollider = true;
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
    }
}
