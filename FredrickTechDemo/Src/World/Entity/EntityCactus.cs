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
            yaw = rand.NextDouble() * 360;
        }
        public EntityCactus(Vector3D pos) : base(pos)
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
            yaw = rand.NextDouble() * 360;
        }

        public override void onTick()
        {
            base.onTick();//do first

            if(rand.Next(0, 25) == 1)
            {
                turn = !turn;
            }

            if (turn) rotateYaw(-5.0F); else rotateYaw(5.0F);

            walkFowards();

            if (rand.Next(0, 50) == 1)
            {
                jump();
            }
        }
    }
}
