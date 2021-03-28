using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;

namespace RabbetGameEngine.SubRendering
{
    public class BatchGuiText : Batch
    {
        public BatchGuiText(int renderLayer = 0) : base(RenderType.guiLines, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.text2DName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            VertexBufferLayout l1 = new VertexBufferLayout();
            Vertex.configureLayout(l1);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l1);
            vao.addIndicesBufferDynamic(indices.Length);
            vao.updateIndices(indices, indices.Length);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod)
        {
            int n = vertices.Length;
            if (!BatchUtil.canFitOrResize(ref vertices, mod.vertices.Length, requestedVerticesCount, maxVertexCount)) return false;
            int i = indices.Length;
            if (BatchUtil.canResizeQuadIndicesIfNeeded(ref indices, requestedVerticesCount + mod.vertices.Length, maxIndiciesCount))
            {
                if (i != indices.Length)
                {
                    vao.resizeIndices(indices.Length);
                    vao.updateIndices(indices, indices.Length);
                }
            }
            else return false;

            if (vertices.Length != n)
            {
                vao.resizeBuffer(0, vertices.Length * Vertex.SIZE_BYTES);
            }

            System.Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);
            requestedVerticesCount += mod.vertices.Length;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            batchShader.use();
            GL.DepthMask(false);
            GL.DepthRange(0, 0.005F);
            GL.DrawElements(PrimitiveType.Triangles, requestedVerticesCount + (requestedVerticesCount / 2), DrawElementsType.UnsignedInt, 0);
            GL.DepthRange(0, 1);
            GL.DepthMask(true);
            vao.unBind();
        }
    }
}
