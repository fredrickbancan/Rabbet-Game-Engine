using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class BatchLerpQuadsTransparent : Batch
    {
        public BatchLerpQuadsTransparent(int renderLayer = 0) : base(RenderType.lerpQuadsTransparent, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.lerpQuadsTransparentName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 3;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l15 = new VertexBufferLayout();
            Vertex.configureLayout(l15);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l15);
            VertexBufferLayout matl7 = new VertexBufferLayout();
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            matl7.add(VertexAttribPointerType.Float, 4);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl7);
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod, Texture tex = null)
        {
            return true;
        }

        public override void updateBuffers()
        {

        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            bindAllTextures();
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, System.IntPtr.Zero, requestedObjectItterator, sizeof(uint));
            vao.unBind();
        }
    }
}
