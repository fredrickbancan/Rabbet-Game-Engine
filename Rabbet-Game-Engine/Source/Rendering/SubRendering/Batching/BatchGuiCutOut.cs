using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
using System;

namespace RabbetGameEngine.SubRendering
{
    public class BatchGuiCutOut :  Batch
    {
        public BatchGuiCutOut(int renderLayer = 0) : base(RenderType.guiCutout, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            base.buildBatch();
            ShaderUtil.tryGetShader(ShaderUtil.guiCutoutName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
            maxBufferSizeBytes /= 2;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            VAO.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l);
            VAO.addIndicesBufferDynamic(indices.Length);
            VAO.updateIndices(indices, indices.Length);
            VAO.drawType = PrimitiveType.Triangles;
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
                    VAO.resizeIndices(indices.Length);
                    VAO.updateIndices(indices, indices.Length);
                }
            }
            else return false;

            if (vertices.Length != n)
            {
                VAO.resizeBuffer(0, vertices.Length * Vertex.SIZE_BYTES);
            }

            Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);
            requestedVerticesCount += mod.vertices.Length;
            return true;
        }

        public override void updateBuffers()
        {
            VAO.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
        }

        public override void updateUniforms()
        {
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
        }

        public override void drawBatch(Planet thePlanet)
        {
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", GameInstance.get.thePlayer.getViewMatrix());
            batchShader.setUniformVec3F("fogColor", thePlanet.getFogColor());
            batchShader.setUniform1F("fogStart", thePlanet.getFogStart());
            batchShader.setUniform1F("fogEnd", thePlanet.getFogEnd());
            GL.DepthMask(false);
            GL.DepthRange(0, 0.005F);
            GL.DrawElements(PrimitiveType.Triangles,requestedVerticesCount + (requestedVerticesCount / 2), DrawElementsType.UnsignedInt, 0);
            GL.DepthRange(0, 1);
            GL.DepthMask(true);
        }
    }
}
