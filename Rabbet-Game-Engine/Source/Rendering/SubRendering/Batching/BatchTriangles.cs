using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchTriangles : Batch
    {
        public BatchTriangles(int renderLayer = 0) : base(RenderType.triangles, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 3;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l4 = new VertexBufferLayout();
            Vertex.configureLayout(l4);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l4);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod)
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
            GL.DrawElements(PrimitiveType.Triangles, requestedIndicesCount, DrawElementsType.UnsignedInt, indices);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
