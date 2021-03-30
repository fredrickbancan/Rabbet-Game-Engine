using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchQuads : Batch
    { //Not implemented
        public BatchQuads(int renderLayer = 0) : base(RenderType.quads, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.quadsName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l5 = new VertexBufferLayout();
            Vertex.configureLayout(l5);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l5);
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
            batchShader.setUniformVec3F("fogColor", thePlanet.getFogColor());
            batchShader.setUniform1F("fogStart", thePlanet.getFogStart());
            batchShader.setUniform1F("fogEnd", thePlanet.getFogEnd());
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            bindAllTextures();
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", thePlanet.getViewMatrix());
            GL.DrawArrays(PrimitiveType.Triangles, 0, requestedVerticesCount);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
