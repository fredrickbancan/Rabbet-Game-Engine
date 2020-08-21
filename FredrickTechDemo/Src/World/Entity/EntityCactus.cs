using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo
{
    public class EntityCactus : EntityLiving
    {
        private bool turn = true;
        private static Random rand = new Random();
        public EntityCactus() : base()
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3D(-0.5, -0.5, -0.5), new Vector3D(0.5, 0.5, 0.5), this), 1);
            yaw = rand.NextDouble() * 360;
            walkFowards();
        }
        public EntityCactus(Vector3D pos) : base(pos)
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            this.setCollider(new AABBCollider(new Vector3D(-0.5, -0.5, -0.5), new Vector3D(0.5, 0.5, 0.5), this), 1);
            yaw = rand.NextDouble() * 360;
            walkFowards();
        }

        public override void onTick()
        {
            base.onTick();//do first

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
