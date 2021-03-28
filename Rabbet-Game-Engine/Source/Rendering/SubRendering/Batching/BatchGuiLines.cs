using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
using System;

namespace RabbetGameEngine.SubRendering
{
    public class BatchGuiLines : Batch
    {
        public BatchGuiLines(int renderLayer = 0) : base(RenderType.guiLines, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.guiLinesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout ll = new VertexBufferLayout();
            Vertex.configureLayout(ll);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, ll);
            vao.drawType = PrimitiveType.Lines;
        }

        public override bool tryToFitInBatchModel(Model mod)
        {
            int n = vertices.Length;
            if (!BatchUtil.canFitOrResize(ref vertices, mod.vertices.Length, requestedVerticesCount, maxVertexCount)) return false;

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
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", GameInstance.get.thePlayer.getViewMatrix());
            GL.LineWidth(GUIManager.guiLineWidth);
            GL.DepthMask(false);
            GL.DepthRange(0, 0.005F);
            GL.DrawArrays(PrimitiveType.Lines, 0, requestedVerticesCount);
            GL.DepthRange(0, 1);
            GL.DepthMask(true);
            GL.LineWidth(Renderer.defaultLineWidthInPixels);
            vao.unBind();
        }
    }
}
