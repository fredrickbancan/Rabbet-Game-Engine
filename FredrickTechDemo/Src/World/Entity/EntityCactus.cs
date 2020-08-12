using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo
{
    public class EntityCactus : EntityLiving
    {
        public EntityCactus() : base()
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
        }
        public EntityCactus(Vector3D pos) : base(pos)
        {
            this.entityModel = new EntityCactusModel(this);
            this.hasModel = true;
        }

        public override void onTick()
        {
            base.onTick();//do first
            rotateYaw(-1.0F);
            walkFowards();
           
            jump();
        }
    }
}
