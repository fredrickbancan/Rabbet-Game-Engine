using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Rendering;
using System;

namespace RabbetGameEngine
{
    public class BatchGuiTransparent : Batch
    {
        public BatchGuiTransparent(int renderLayer = 0) : base(RenderType.guiTransparent, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.guiTransparentName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
            batchShader.setUniformIArray("uTextures", getUniformTextureSamplerArrayInts(RenderConstants.MAX_BATCH_TEXTURES));
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            VertexBufferLayout lt = new VertexBufferLayout();
            Vertex.configureLayout(lt);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, lt);
            vao.addIndicesBufferDynamic(indices.Length);
            vao.updateIndices(indices, indices.Length);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod, Texture tex = null)
        {
            if (!tryAddModelTexAndApplyIndex(mod, tex)) return false;

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

            Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);
            requestedVerticesCount += mod.vertices.Length;
            hasBeenUsed = true;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            bindAllTextures();
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
