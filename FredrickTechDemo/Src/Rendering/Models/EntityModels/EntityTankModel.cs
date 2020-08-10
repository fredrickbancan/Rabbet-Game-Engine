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
        private bool modelsNeedUpdating = true;

        public EntityTankModel(EntityTank parent)//dont want to call the base constructor for this model
        {
            this.parent = parent;
            tankWheelModel = (ModelDrawable)OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankWheels.obj")).scaleVertices(new Vector3F(.5f,.5f,.5f));
            tankBodyModel = (ModelDrawable)OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankBody.obj")).scaleVertices(new Vector3F(.5f, .5f, .5f));
            tankBarrelModel = (ModelDrawable)OBJLoader.loadModelDrawableFromObjFile(shaderDir, textureDir, ResourceHelper.getOBJFileDir(@"Tank\TankBarrel.obj")).scaleVertices(new Vector3F(.5f, .5f, .5f)).translateVertices(new Vector3F(0, 2, -1F));
            updateModel();
            updateModel();
        }

        /*Called every tick by base*/
        public override void updateModel()
        {
            prevTickTankWheelsModelMatrix = tankWheelsModelMatrix;
            prevTickTankBodyModelMatrix = tankBodyModelMatrix;
            prevTickTankBarrelModelMatrix = tankBarrelModelMatrix;

            tankWheelsModelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getPitch(), -(float)parent.getWheelsYaw - 90, (float)parent.getRoll())) * Matrix4F.translate(Vector3F.convert(parent.getPosition()));
            tankBodyModelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getPitch(), -(float)parent.getBodyYaw, (float)parent.getRoll())) * Matrix4F.translate(Vector3F.convert(parent.getPosition()));
            tankBarrelModelMatrix = Matrix4F.rotate(new Vector3F((float)parent.getBarrelPitch, -(float)parent.getBarrelYaw, (float)parent.getRoll())) * Matrix4F.translate(Vector3F.convert(parent.getBarrelpos));
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
    }
}
