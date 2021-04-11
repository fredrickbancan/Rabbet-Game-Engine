using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchLerpQuads : Batch
    {
        public BatchLerpQuads(int renderLayer = 0) : base(RenderType.lerpQuads, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.quadsName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 3;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l11 = new VertexBufferLayout();
            Vertex.configureLayout(l11);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l11);
            VertexBufferLayout matl4 = new VertexBufferLayout();
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            matl4.add(VertexAttribPointerType.Float, 4);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl4);
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            vao.drawType = PrimitiveType.Triangles;
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
            bindAllTextures();
            batchShader.use();
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, System.IntPtr.Zero, requestedObjectItterator, sizeof(uint));
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
