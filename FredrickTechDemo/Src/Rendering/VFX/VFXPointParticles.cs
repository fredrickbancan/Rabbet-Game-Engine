using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo.VFX
{
    /*Base class for any VFX using point particles*/
    public class VFXPointParticles : VFXBase
    {
        private static String defaultShaderDir = ResourceHelper.getShaderFileDir(@"VFX\PointParticleFog.shader");
        protected ColourF pointColor;
        protected float randomPointPositionSpread;//the maximum distance between points when randomizing a point cloud (e.g, a puff of smoke)
        protected float pointRadius;
        protected bool randomBrightness = false;
        protected bool pointAmbientOcclusion = false;//if ambient occlusion is true, the point will be rendered with a spherical ambient occlusion giving the illusion of a sphere instead of a 2d circular point
        protected int particleCount;
       

        /*this constructor is for creating a point particle based VFX which does not create a random point cloud. maybe you will want to construct the points in a specific manner with colors or use a model.*/
        public VFXPointParticles(Vector3D pos, ColourF color, float radius, bool ambientOcclusion, float maxExistingSeconds = 2F) : base(pos, 1.0F, defaultShaderDir, "none", "none", maxExistingSeconds, VFXRenderType.points)
        {
            pointColor = color;
            pointRadius = radius;
            pointAmbientOcclusion = ambientOcclusion;
        }

        /*this constructor is for creating a randomized particle cloud at the position using the provided parameters*/
        public VFXPointParticles(Vector3D pos, ColourF color, int particleCount, float randomPointPositionSpread, float radius, bool randomBrightness, bool ambientOcclusion, float expansionEverySecond = 0, float maxExistingSeconds = 2F) : base(pos, 1.0F, defaultShaderDir, "none", "none", expansionEverySecond, maxExistingSeconds, VFXRenderType.points)
        {
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
            Vertex[] points = new Vertex[particleCount];
            for(int i = 0; i < particleCount; i++)
            {
                points[i] = new Vertex(Vector3F.convert(getRandomParticleOffset()), getPointColor(), Vector2F.zero);
            }

            vfxModel = new ModelDrawable(defaultShaderDir, "none", points);
        }

        protected Vector3D getRandomParticleOffset()
        {
            return Vector3D.normalize(new Vector3D( 0.5 - GameInstance.rand.NextDouble(), 0.5 - GameInstance.rand.NextDouble(), 0.5 - GameInstance.rand.NextDouble())) * (randomPointPositionSpread / 2);
        }

        protected Vector4F getPointColor()
        {
            if(randomBrightness)
            {
                Vector4F colorVec = pointColor.normalVector4F();
                float randomBrightnessAmount = 0.2F - 0.4F * (float)GameInstance.rand.NextDouble();
                return new Vector4F(colorVec.r + randomBrightnessAmount, colorVec.g + randomBrightnessAmount, colorVec.b + randomBrightnessAmount, 1.0F);
            }

            return pointColor.normalVector4F();
        }

        public override void draw(Matrix4F viewMatrix, Matrix4F projectionMatrix, Vector3F fogColor)
        {
            if (vfxModel != null && !removalFlag)
            {
                vfxModel.drawPoints(viewMatrix, projectionMatrix, prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * TicksAndFps.getPercentageToNextTick(), fogColor, pointRadius, pointAmbientOcclusion);
            }
            else
            {
                Application.warn("An attempt was made to render a null or disposed GL_POINTS VFX.");
            }
        }
    }
}
