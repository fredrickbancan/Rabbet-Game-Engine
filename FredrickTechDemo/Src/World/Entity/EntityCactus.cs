using FredrickTechDemo.Entities;
using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo
{
    public class EntityCactus : EntityLiving
    {
        public EntityCactus() : base()
        {
            this.entityModel = new EntityModel(this);
            this.entityModel.initModel(EntityCactusModel.getNewModelDrawable());
            this.hasModel = true;

            this.walkSpeed = 0.005D;
        }
        public EntityCactus(Vector3D pos) : base(pos)
        {
            this.entityModel = new EntityModel(this);
            this.entityModel.initModel(EntityCactusModel.getNewModelDrawable());
            this.hasModel = true;
            this.walkSpeed = 0.005D;
        }

        public override void onTick()
        {
            base.onTick();//do first
            walkFowards();
            rotateYaw(1.5F);
            jump();
        }
    }
}
