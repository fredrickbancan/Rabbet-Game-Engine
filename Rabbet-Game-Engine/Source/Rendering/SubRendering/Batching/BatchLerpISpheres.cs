using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine
{
    public class BatchLerpISpheres : Batch
    {
        public BatchLerpISpheres(int renderLayer = 0) : base(RenderType.lerpISpheres, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
            batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l9 = new VertexBufferLayout();
            PointParticle.configureLayout(l9);
            PointParticle.configureLayout(l9); 
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l9);
            vao.drawType = PrimitiveType.Points;
        }

        public override bool tryToFitInBatchPoints(PointCloudModel mod)
        {
            int n = batchedPoints.Length;
            if (!BatchUtil.canFitOrResize(ref batchedPoints, mod.points.Length * 2, pointsItterator, maxPointCount)) return false;
            if (batchedPoints.Length != n)
            {
                vao.resizeBuffer(0, batchedPoints.Length * PointParticle.SIZE_BYTES);
            }
            for (n = 0; n < mod.points.Length; n++)
            {
                batchedPoints[pointsItterator] = mod.points[n];
                batchedPoints[pointsItterator + 1] = mod.prevPoints[n];
                pointsItterator += 2;
            }
            hasBeenUsed = true;
            return true;
        }

        public override bool tryToFitInBatchLerpPoint(PointParticle p, PointParticle prevP)
        {
            int n = batchedPoints.Length;
            if (!BatchUtil.canFitOrResize(ref batchedPoints, 2, pointsItterator, maxPointCount)) return false;
            if (batchedPoints.Length != n)
            {
                vao.resizeBuffer(0, batchedPoints.Length * PointParticle.SIZE_BYTES);
            }
            batchedPoints[pointsItterator] = p;
            batchedPoints[pointsItterator + 1] = prevP;
            pointsItterator += 2;
            hasBeenUsed = true;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, batchedPoints, pointsItterator * PointParticle.SIZE_BYTES);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.DrawArrays(PrimitiveType.Points, 0, pointsItterator / 2);
            vao.unBind();
        }
    }
}
