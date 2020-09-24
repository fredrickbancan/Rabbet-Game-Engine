using OpenTK;

namespace RabbetGameEngine.Models
{
    public class EntityTankModel : EntityModel
    {
        private new EntityTank parent;
        private ModelDrawable tankWheelModel;
        private ModelDrawable tankBodyModel;
        private ModelDrawable tankBarrelModel;
        private Matrix4 prevTickTankWheelsModelMatrix;//previous tick model matrices for interpolating each part of the tank model.
        private Matrix4 prevTickTankBodyModelMatrix;
        private Matrix4 prevTickTankBarrelModelMatrix;
        private Matrix4 tankWheelsModelMatrix = Matrix4.Identity;
        private Matrix4 tankBodyModelMatrix = Matrix4.Identity;
        private Matrix4 tankBarrelModelMatrix = Matrix4.Identity;
        private string shaderName = "EntityWorld_F";
        private string textureName = "Camo";
        private string wheelModelName = @"Tank\TankWheels.obj";
        private string bodyModelName = @"Tank\TankBody.obj";
        private string barrelModelName = @"Tank\TankBarrel.obj";
        

        public EntityTankModel(EntityTank parent)//dont want to call the base constructor for this model
        {
            this.parent = parent;
            tankWheelModel = ModelUtil.createModelDrawable(shaderName, textureName, wheelModelName);
            tankBodyModel = ModelUtil.createModelDrawable(shaderName, textureName, bodyModelName);
            tankBarrelModel = ModelUtil.createModelDrawable(shaderName, textureName, barrelModelName);
            onTick();
            onTick();
        }

        /*Called every tick by base*/
        public override void onTick()
        {
            prevTickTankWheelsModelMatrix = tankWheelsModelMatrix;
            prevTickTankBodyModelMatrix = tankBodyModelMatrix;
            prevTickTankBarrelModelMatrix = tankBarrelModelMatrix;
            
            tankWheelsModelMatrix = Matrix4.CreateScale(new Vector3(0.5F, 0.5F, 0.5F)) * MathUtil.createRotation(new Vector3((float)parent.getPitch(), -(float)parent.getYaw() - 90, (float)parent.getRoll())) * Matrix4.CreateTranslation(parent.getPosition());
            tankBodyModelMatrix = Matrix4.CreateScale(new Vector3(0.5F, 0.5F, 0.5F)) * MathUtil.createRotation(new Vector3(0, -(float)parent.getBodyYaw, 0)) * Matrix4.CreateTranslation(parent.getPosition());
            tankBarrelModelMatrix = MathUtil.createRotation(new Vector3((float)parent.getBarrelPitch, 0, 0)) * Matrix4.CreateTranslation(0, 1.7F, -2F) * tankBodyModelMatrix;
            
        }

        /*Called every frame by base*/
        public override void draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 fogColor)
        {
            float lerpFactor = TicksAndFps.getPercentageToNextTick();
            tankWheelModel.draw(viewMatrix, projectionMatrix, MathUtil.lerp(prevTickTankWheelsModelMatrix, tankWheelsModelMatrix, lerpFactor), fogColor);
            tankBodyModel.draw(viewMatrix, projectionMatrix, MathUtil.lerp(prevTickTankBodyModelMatrix, tankBodyModelMatrix, lerpFactor), fogColor);
            tankBarrelModel.draw(viewMatrix, projectionMatrix, MathUtil.lerp(prevTickTankBarrelModelMatrix, tankBarrelModelMatrix, lerpFactor), fogColor);
        }

        public override EntityModel setModel(ModelDrawable newModel)
        {
            return this;
        }

        public override bool exists()
        {
            return tankWheelModel != null && tankBodyModel != null && tankBarrelModel != null;
        }

        public Matrix4 getBarrelModelMatrix()
        {
            return tankBarrelModelMatrix;
        }
    }
}
