using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class BatchLerpLines : Batch
    { //Not implemented
        public BatchLerpLines(int renderLayer = 0) : base(RenderType.lerpLines, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 3;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l12 = new VertexBufferLayout();
            Vertex.configureLayout(l12);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l12);
            VertexBufferLayout matl5 = new VertexBufferLayout();
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            matl5.add(VertexAttribPointerType.Float, 4);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl5);
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.drawType = PrimitiveType.Lines;
        }

        public override bool tryToFitInBatchModel(Model mod, Texture tex = null)
        {
            return false;
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
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.MultiDrawElementsIndirect(PrimitiveType.Lines, DrawElementsType.UnsignedInt, System.IntPtr.Zero, requestedObjectItterator, 0);
            vao.unBind();
        }
    }
}
