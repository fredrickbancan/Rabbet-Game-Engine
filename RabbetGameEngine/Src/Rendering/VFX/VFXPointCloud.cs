using OpenTK;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine.VFX
{
    /*Base class for any VFX using point particles*/
    public class VFXPointCloud : VFXBase
    {
        protected PointCloudModel pointVfxModel;
        protected CustomColor pointColor;
        protected float randomPointPositionSpread;//the maximum distance between points when randomizing a point cloud (e.g, a puff of smoke)
        protected float pointRadius;
        protected bool randomBrightness = false;
        protected bool pointAmbientOcclusion = false;//if ambient occlusion is true, the point will be rendered with a spherical ambient occlusion giving the illusion of a sphere instead of a 2d circular point
        protected int particleCount;
        protected float colorAlpha;

        /*this constructor is for creating a point particle based VFX which does not create a random point cloud. maybe you will want to construct the points in a specific manner with colors or use a model.*/
        public VFXPointCloud(Vector3 pos, CustomColor color, float radius, bool transparency, bool ambientOcclusion, float maxExistingSeconds = 2F, float alpha = 1) : base(pos, 1.0F, "none", null, maxExistingSeconds, transparency? BatchType.lerpPointsTransparent : BatchType.lerpPoints)
        {
            colorAlpha = alpha;
            pointColor = color;
            pointRadius = radius;
            pointAmbientOcclusion = ambientOcclusion;
        }

        /*this constructor is for creating a randomized particle cloud at the position using the provided parameters*/
        public VFXPointCloud(Vector3 pos, CustomColor color, int particleCount, float randomPointPositionSpread, float radius, bool randomBrightness, bool transparency,  bool ambientOcclusion, float maxExistingSeconds = 2F, float alpha = 1) : base(pos, 1.0F, "none", null, maxExistingSeconds, transparency ? BatchType.lerpPointsTransparent : BatchType.lerpPoints)
        {
            colorAlpha = alpha;
            this.randomBrightness = randomBrightness;
            this.particleCount = particleCount;
            pointColor = color;
            this.randomPointPositionSpread = randomPointPositionSpread;
            pointRadius = radius;
            pointAmbientOcclusion = ambientOcclusion;
            constructPointCloudModel();
        }

        /*Builds the vertices for the point cloud to be rendered. By default this method will build a randomized point cloud
          at the position of this VFX with the parameters provided. It can be overwritten to set specific positions for multiple 
          points and specific colors.*/
        protected virtual void constructPointCloudModel()
        {
            PointParticle[] points = new PointParticle[particleCount];
            for(int i = 0; i < particleCount; i++)
            {
                points[i] = new PointParticle(getRandomParticleOffset(), getPointColor(), pointRadius, pointAmbientOcclusion);
            }

            pointVfxModel = new PointCloudModel(points);
        }

        protected Vector3 getRandomParticleOffset()
        {
            return Vector3.Normalize(new Vector3( 0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble(), 0.5F - (float)GameInstance.rand.NextDouble())) * (randomPointPositionSpread / 2 +( randomPointPositionSpread / 4 + randomPointPositionSpread / 2 *  (float)GameInstance.rand.NextDouble()));
        }

        protected Vector4 getPointColor()
        {
            if(randomBrightness)
            {
                Vector4 colorVec = pointColor.toNormalVec4();
                float randomBrightnessAmount = 0.2F - 0.4F * (float)GameInstance.rand.NextDouble();
                return new Vector4(colorVec.X + randomBrightnessAmount, colorVec.Y + randomBrightnessAmount, colorVec.Z + randomBrightnessAmount, colorAlpha);
            }
            Vector4 nonRandBrightColor = pointColor.toNormalVec4();
            nonRandBrightColor.W = colorAlpha;
            return nonRandBrightColor;
        }
    }
}
