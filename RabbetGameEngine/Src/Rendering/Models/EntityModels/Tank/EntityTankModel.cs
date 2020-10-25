using OpenTK;

namespace RabbetGameEngine.Models
{
    public class EntityTankModel : EntityModel
    {
        private Model tankWheelModel;
        private Model tankBodyModel;
        private Model tankBarrelModel;
        private static string textureName = "Camo";
        private string wheelModelName = "TankWheels";
        private string bodyModelName = "TankBody";
        private string barrelModelName = "TankBarrel";

        public EntityTankModel(EntityTank parent) : base(parent, textureName, null)
        {
            tankWheelModel = MeshUtil.getMeshForModel(wheelModelName);
            tankBodyModel = MeshUtil.getMeshForModel(bodyModelName);
            tankBarrelModel = MeshUtil.getMeshForModel(barrelModelName);
            onTick();
            onTick();
        }

        /*Called every tick by base*/
        public override void onTick()
        {
            tankWheelModel.prevModelMatrix = tankWheelModel.modelMatrix;
            tankBodyModel.prevModelMatrix = tankBodyModel.modelMatrix;
            tankBarrelModel.prevModelMatrix = tankBarrelModel.modelMatrix;

            tankWheelModel.modelMatrix = Matrix4.CreateScale(new Vector3(0.5F, 0.5F, 0.5F)) * MathUtil.createRotation(new Vector3(parent.getPitch(), -parent.getYaw() - 90, parent.getRoll())) * Matrix4.CreateTranslation(parent.getPosition());
            tankBodyModel.modelMatrix = Matrix4.CreateScale(new Vector3(0.5F, 0.5F, 0.5F)) * MathUtil.createRotation(new Vector3(0, -((EntityTank)parent).getBodyYaw, 0)) * Matrix4.CreateTranslation(parent.getPosition());
            tankBarrelModel.modelMatrix = MathUtil.createRotation(new Vector3(((EntityTank)parent).getBarrelPitch, 0, 0)) * Matrix4.CreateTranslation(0, 1.7F, -2F) * tankBodyModel.modelMatrix;
            
        }

        public override void sendRenderRequest()
        {
            Renderer.requestRender(batchType, tex, tankWheelModel);
            Renderer.requestRender(batchType, tex, tankBodyModel);
            Renderer.requestRender(batchType, tex, tankBarrelModel);
        }

        public Matrix4 getBarrelModelMatrix()
        {
            return tankBodyModel.modelMatrix;
        }
    }
}
