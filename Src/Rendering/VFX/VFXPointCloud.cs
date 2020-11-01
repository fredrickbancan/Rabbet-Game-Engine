using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine.VisualEffects
{
    //TODO: Create vfx class for singular point particles.
    /*Base class for any VFX using point particles*/
    public class VFXPointCloud : VFX
    {
        protected PointCloudModel cloudModel;
        protected CustomColor pointColor;
        protected float pointRadius;
        protected bool pointAmbientOcclusion = false;//if ambient occlusion is true, the point will be rendered with a spherical ambient occlusion giving the illusion of a sphere instead of a 2d circular point
        protected float colorAlpha;
        protected Matrix4 modelMatrix = Matrix4.Identity;
        protected Matrix4 prevTickModelMatrix = Matrix4.Identity;
        protected bool transparency = false;

        public VFXPointCloud(Vector3 pos, CustomColor color, bool transparency, bool ambientOcclusion, float maxExistingSeconds, float radius, float alpha) : base(pos, 1.0F, "none", null, maxExistingSeconds, transparency ? BatchType.lerpISpheresTransparent : BatchType.lerpISpheres)
        {
            this.transparency = transparency;
            if(!transparency)
            {
                colorAlpha = 1.0F;
            }
            else
            {
                colorAlpha = alpha;
            }
            pointColor = color;
            pointRadius = radius;
            pointAmbientOcclusion = ambientOcclusion;
            this.modelMatrix = Matrix4.CreateScale(new Vector3(scale * scaleXModifyer, scale * scaleYModifyer, scale * scaleZModifyer)) * MathUtil.createRotation(new Vector3(pitch, -yaw - 90, roll)) * Matrix4.CreateTranslation(pos);
            this.prevTickModelMatrix = this.modelMatrix;
        }

        /*Builds the vertices for the point cloud to be rendered. By default this method will build a randomized point cloud
          at the position of this VFX with the parameters provided. It can be overwritten to set specific positions for multiple 
          points and specific colors.*/
        public virtual VFXPointCloud constructRandomPointCloudModel(int count, float randomPointPositionSpread, bool randomBrightness)
        {
            PointParticle[] points = new PointParticle[count];
            for(int i = 0; i < count; i++)
            {
                points[i] = new PointParticle(getRandomParticleOffset(randomPointPositionSpread), getRandomPointColor(randomBrightness), pointRadius, pointAmbientOcclusion);
            }

            cloudModel = new PointCloudModel(points);
            return this;
        }

        protected Vector3 getRandomParticleOffset(float randomPointPositionSpread)
        {
            return Vector3.Normalize(new Vector3( 0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble())) * (randomPointPositionSpread / 2 +( randomPointPositionSpread / 4 + randomPointPositionSpread / 2 *  (float)GameInstance.rand.NextDouble()));
        }

        protected Vector4 getRandomPointColor(bool randomBright)
        {
            if(randomBright)
            {
                Vector4 colorVec = pointColor.toNormalVec4();
                float randomBrightnessAmount = 0.2F - 0.4F * (float)GameInstance.rand.NextDouble();
                return new Vector4(colorVec.X + randomBrightnessAmount, colorVec.Y + randomBrightnessAmount, colorVec.Z + randomBrightnessAmount, colorAlpha);
            }
            Vector4 nonRandBrightColor = pointColor.toNormalVec4();
            nonRandBrightColor.W = colorAlpha;
            return nonRandBrightColor;
        }

        public virtual void setPointCloudModel(PointCloudModel mod)
        {
            this.cloudModel = mod;
        }

        public override void preTick()
        {
            if(cloudModel != null)
            cloudModel.preTick();
            base.preTick();
        }

        protected override void updateVFXModel()
        {
            this.prevTickModelMatrix = this.modelMatrix;
            scaleVelocity += scaleAcceleration - scaleResistance * scaleVelocity; //decrease expansion rate
            scale += scaleVelocity;
            this.modelMatrix = Matrix4.CreateScale(new Vector3(scale * scaleXModifyer, scale * scaleYModifyer, scale * scaleZModifyer)) * MathUtil.createRotation(new Vector3(pitch, -yaw - 90,roll)) * Matrix4.CreateTranslation(pos);
       
            //temp, sandboxing.
            if (transparency)
            {
                this.colorAlpha *= 1 - (float)GameInstance.rand.NextDouble() * 0.052F;
                if (colorAlpha < 0.01F)
                {
                    ceaseToExist();
                }
                cloudModel.setAlpha(colorAlpha);
                cloudModel.scaleRadii(0.99F);
            }
        }

        public override void sendRenderRequest()
        {
            if(!removalFlag)
            Renderer.requestRender(cloudModel.createTransformedCopy(modelMatrix, prevTickModelMatrix), this.transparency, true);
        }
    }
}
