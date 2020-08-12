using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    public class EntityTankModel : EntityModel
    {
        private new EntityTank parent;
        private ModelDrawable tankWheelModel;
        private ModelDrawable tankBodyModel;
        private ModelDrawable tankBarrelModel;
        private Matrix4F prevTickTankWheelsModelMatrix;//previous tick model matrices for interpolating each part of the tank model.
        private Matrix4F prevTickTankBodyModelMatrix;
        private Matrix4F prevTickTankBarrelModelMatrix;
        private Matrix4F tankWheelsModelMatrix = new Matrix4F(1.0F);
        private Matrix4F tankBodyModelMatrix = new Matrix4F(1.0F);
        private Matrix4F tankBarrelModelMatrix = new Matrix4F(1.0F);
        private String shaderDir = ResourceHelper.getShaderFileDir("ColorTextureFog3D.shader");
        private String textureDir = ResourceHelper.getTextureFileDir("Camo.png");

        public EntityTankModel(EntityTank parent)//dont want to call the base constructor for this model
        {
            this.parent = parent;
            tankWheelModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankWheels.obj"));
            tankBodyModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankBody.obj"));
            tankBarrelModel = OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankBarrel.obj"));
            updateModel();
            updateModel();
        }

        /*Called every tick by base*/
        public override void updateModel()
        {
            prevTickTankWheelsModelMatrix = tankWheelsModelMatrix;
            prevTickTankBodyModelMatrix = tankBodyModelMatrix;
            prevTickTankBarrelModelMatrix = tankBarrelModelMatrix;

            //Matrix heirarchy, barel is child of body is child of wheels.
            //body yaw  is also child of camera yaw, which means it has to be additionally rotated by getBodyYaw
            //barrel pitch is also child of camera pitch, which means it has to be additionally rotated by getBarrelPitch
            //barrel also needs to be translated to a specific spot on the body model
            tankWheelsModelMatrix = Matrix4F.scale(new Vector3F(0.5F, 0.5F, 0.5F)) * Matrix4F.rotate(new Vector3F((float)parent.getPitch(), -(float)parent.getYaw() - 90, (float)parent.getRoll())) * Matrix4F.translate(Vector3F.convert(parent.getPosition()));
            tankBodyModelMatrix = Matrix4F.rotate(new Vector3F(0, 90 + (float)parent.getYaw() - (float)parent.getBodyYaw, 0)) * tankWheelsModelMatrix;
            tankBarrelModelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getBarrelPitch, 0, 0)) * Matrix4F.translate(new Vector3F(0, 4, -2F)) * tankBodyModelMatrix;

        }

        /*Called every frame by base*/
        public override void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            tankWheelModel.draw(viewMatrix, projectionMatrix, prevTickTankWheelsModelMatrix + (tankWheelsModelMatrix - prevTickTankWheelsModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            tankBodyModel.draw(viewMatrix, projectionMatrix, prevTickTankBodyModelMatrix + (tankBodyModelMatrix - prevTickTankBodyModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
            tankBarrelModel.draw(viewMatrix, projectionMatrix, prevTickTankBarrelModelMatrix + (tankBarrelModelMatrix - prevTickTankBarrelModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor);
        }

        public override EntityModel setModel(ModelDrawable newModel)
        {
            return this;
        }

        public override bool exists()
        {
            return tankWheelModel != null && tankBodyModel != null && tankBarrelModel != null;
        }

        public Matrix4F getBarrelModelMatrix()
        {
            return tankBarrelModelMatrix;
        }
    }
}
