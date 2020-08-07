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
            this.walkSpeed = 0.015D;
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
            rotatePitch(-1.0F);
            walkFowards();
           
            jump();
        }
    }
}
