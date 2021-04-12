using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine
{
    public class BatchQuadsTransparent : Batch
    { //Not implemented
        public BatchQuadsTransparent(int renderLayer = 0) : base(RenderType.quadsTransparent, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.quadsTransparentName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l17 = new VertexBufferLayout();
            Vertex.configureLayout(l17);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l17);
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, requestedVerticesCount);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
