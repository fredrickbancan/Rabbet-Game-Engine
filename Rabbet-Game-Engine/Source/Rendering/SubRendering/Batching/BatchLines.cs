using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchLines : Batch
    {
        public BatchLines(int renderLayer = 0) : base(RenderType.guiLines, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.linesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l6 = new VertexBufferLayout();
            Vertex.configureLayout(l6);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l6);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.drawType = PrimitiveType.Lines;
        }

        public override bool tryToFitInBatchModel(Model mod)
        {
            int n = vertices.Length;
            if (!BatchUtil.canFitOrResize(ref vertices, mod.vertices.Length, requestedVerticesCount, maxVertexCount)) return false;
            int i = indices.Length;
            if (!BatchUtil.canFitOrResize(ref indices, mod.indices.Length, requestedIndicesCount, maxIndiciesCount)) return false;
            if (vertices.Length != n)
            {
                vao.resizeBuffer(0, vertices.Length * Vertex.SIZE_BYTES);
            }

            if (indices.Length != i)
            {
                vao.resizeIndices(indices.Length);
            }
            System.Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);

            for (i = 0; i < mod.indices.Length; i++)
            {
                indices[requestedIndicesCount + i] = (uint)(mod.indices[i] + requestedVerticesCount);
            }
            requestedVerticesCount += mod.vertices.Length;
            requestedIndicesCount += mod.indices.Length;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
            vao.updateIndices(indices, requestedIndicesCount);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            batchShader.setUniformVec3F("fogColor", thePlanet.getFogColor());
            batchShader.setUniform1F("fogStart", thePlanet.getFogStart());
            batchShader.setUniform1F("fogEnd", thePlanet.getFogEnd());
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            batchShader.use();
            GL.DrawElements(PrimitiveType.Lines, requestedIndicesCount, DrawElementsType.UnsignedInt, 0);
            vao.unBind();
        }
    }
}
