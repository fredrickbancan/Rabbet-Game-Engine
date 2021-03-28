using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchISpheres : Batch
    {
        public BatchISpheres(int renderLayer = 0) : base(RenderType.guiLines, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l7 = new VertexBufferLayout();
            PointParticle.configureLayout(l7);
            l7.instancedData = true;
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l7);
            VertexBufferLayout instl = new VertexBufferLayout();
            instl.add(VertexAttribPointerType.Float, 2);
            vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl);
            vao.drawType = PrimitiveType.TriangleStrip;
        }
        public override bool tryToFitInBatchPoints(PointCloudModel mod)
        {
            int n = batchedPoints.Length;
            if (!BatchUtil.canFitOrResize(ref batchedPoints, mod.points.Length, pointsItterator, maxPointCount)) return false;
            if (batchedPoints.Length != n)
            {
                vao.resizeBuffer(0, batchedPoints.Length * PointParticle.SIZE_BYTES);
            }
            System.Array.Copy(mod.points, 0, batchedPoints, pointsItterator, mod.points.Length);
            pointsItterator += mod.points.Length;
            return true;
        }

        public override bool tryToFitInBatchSinglePoint(PointParticle p)
        {
            int n = batchedPoints.Length;
            if (!BatchUtil.canFitOrResize(ref batchedPoints, 1, pointsItterator, maxPointCount)) return false;
            if (batchedPoints.Length != n)
            {
                vao.resizeBuffer(0, batchedPoints.Length * PointParticle.SIZE_BYTES);
            }
            batchedPoints[pointsItterator++] = p;
            return true;
        }
        public override void updateBuffers()
        {
            vao.updateBuffer(0, batchedPoints, pointsItterator * PointParticle.SIZE_BYTES);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
            batchShader.setUniformVec3F("fogColor", thePlanet.getFogColor());
            batchShader.setUniform1F("fogStart", thePlanet.getFogStart());
            batchShader.setUniform1F("fogEnd", thePlanet.getFogEnd());
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            batchShader.use();
            batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, pointsItterator);
            vao.unBind();
        }
    }
}
