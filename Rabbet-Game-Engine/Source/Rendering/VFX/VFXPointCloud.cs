using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class VFXPointCloud : VFX
    {
        protected PointCloudModel cloudModel;
        protected Color pointColor;
        protected float pointRadius;
        protected bool pointAmbientOcclusion = false;//if ambient occlusion is true, the point will be rendered with a spherical ambient occlusion giving the illusion of a sphere instead of a 2d circular point
        protected float colorAlpha;
        protected float scale = 1;//scale of the VFX
        protected float scaleVelocity; //how much to expand the VFX model every tick, should be converted from expansion every second
        protected float scaleAcceleration; //how much to expand the VFX model every tick, should be converted from expansion every second
        protected float scaleResistance = 0.03572F; //multiplyer decelerates the expansion of this VFX every tick
        protected float scaleXModifyer = 1;
        protected float scaleYModifyer = 1;
        protected float scaleZModifyer = 1;
        protected Matrix4 modelMatrix = Matrix4.Identity;
        protected Matrix4 prevTickModelMatrix = Matrix4.Identity;
        protected bool transparency = false;

        public VFXPointCloud(World w, Vector3 pos, Color color, bool transparency, bool ambientOcclusion, float maxExistingSeconds, float radius, float alpha) : base(w, pos, maxExistingSeconds, transparency ? RenderType.lerpISpheresTransparent : RenderType.lerpISpheres)
        {
            this.transparency = transparency;
            if (!transparency)
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
        }

        /*Builds the vertices for the point cloud to be rendered. By default this method will build a randomized point cloud
          at the position of this VFX with the parameters provided. It can be overwritten to set specific positions for multiple 
          points and specific colors.*/
        public virtual VFXPointCloud constructRandomPointCloudModel(int count, float randomPointPositionSpread, bool randomBrightness)
        {
            PointParticle[] points = new PointParticle[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = new PointParticle(getRandomParticleOffset(randomPointPositionSpread), getRandomPointColor(randomBrightness), pointRadius, pointAmbientOcclusion);
            }

            cloudModel = new PointCloudModel(points);
            return this;
        }

        protected Vector3 getRandomParticleOffset(float randomPointPositionSpread)
        {
            return Vector3.Normalize(new Vector3(0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble())) * (randomPointPositionSpread / 2 + (randomPointPositionSpread / 4 + randomPointPositionSpread / 2 * (float)GameInstance.rand.NextDouble()));
        }

        protected Vector4 getRandomPointColor(bool randomBright)
        {
            if (randomBright)
            {
                Vector4 colorVec = pointColor.toNormalVec4();
                float randomBrightnessAmount = 0.2F - 0.4F * (float)GameInstance.rand.NextDouble();
                return new Vector4(colorVec.X + randomBrightnessAmount, colorVec.Y + randomBrightnessAmount, colorVec.Z + randomBrightnessAmount, colorAlpha);
            }
            Vector4 nonRandBrightColor = pointColor.toNormalVec4();
            nonRandBrightColor.W = colorAlpha;
            return nonRandBrightColor;
        }

        public void setPointCloudModel(PointCloudModel mod)
        {
            this.cloudModel = mod;
        }

        public void setExpansionResistance(float amount)
        {
            scaleResistance = amount;
        }

        public void setExpansionAccel(float expansionEverySecond)
        {
            scaleAcceleration = expansionEverySecond;
        }

        public void setExpansionVelocity(float expansionvel)
        {
            scaleVelocity = expansionvel;
        }

        public void setExpansionXModifyer(float modifyer)
        {
            scaleXModifyer = modifyer;
        }

        public void setExpansionYModifyer(float modifyer)
        {
            scaleYModifyer = modifyer;
        }

        public void setExpansionZModifyer(float modifyer)
        {
            scaleZModifyer = modifyer;
        }

        public override void sendRenderRequest()
        {
            if (!removalFlag)
                Renderer.requestRender(cloudModel.createTransformedCopy(modelMatrix, prevTickModelMatrix), this.transparency, true);
        }
    }
}
