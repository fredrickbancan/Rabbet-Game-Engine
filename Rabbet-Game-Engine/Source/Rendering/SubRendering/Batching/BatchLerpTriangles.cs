using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchLerpTriangles : Batch
    {
        public BatchLerpTriangles(int renderLayer = 0) : base(RenderType.lerpTriangles, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 4;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            drawCommands = new DrawCommand[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l10 = new VertexBufferLayout();
            Vertex.configureLayout(l10);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l10);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            VertexBufferLayout matl3 = new VertexBufferLayout();
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.add(VertexAttribPointerType.Float, 4);
            matl3.instancedData = true;
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl3);
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod)
        {
            int n = vertices.Length;
            if (!BatchUtil.canFitOrResize(ref vertices, mod.vertices.Length, requestedVerticesCount, maxVertexCount)) return false;
            int p = modelMatrices.Length;
            if (!BatchUtil.canFitOrResize(ref modelMatrices, 2, matricesItterator, maxPositionCount)) return false;
            int i = indices.Length;
            if (!BatchUtil.canFitOrResize(ref indices, mod.indices.Length, requestedIndicesCount, maxIndiciesCount)) return false;

            if (vertices.Length != n)
            {
                vao.resizeBuffer(0, vertices.Length * Vertex.SIZE_BYTES);
            }

            if (modelMatrices.Length != p)
            {
                vao.resizeBuffer(1, modelMatrices.Length * sizeof(float) * 16);
            }

            if (indices.Length != i)
            {
                vao.resizeIndices(indices.Length);
            }

            configureDrawCommandsForCurrentObject(mod.indices.Length, false);

            System.Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);
            System.Array.Copy(mod.indices, 0, indices, requestedIndicesCount, mod.indices.Length);
            modelMatrices[matricesItterator] = mod.modelMatrix;
            modelMatrices[matricesItterator + 1] = mod.prevModelMatrix;
            matricesItterator += 2;
            requestedObjectItterator++;
            requestedVerticesCount += mod.vertices.Length;
            requestedIndicesCount += mod.indices.Length;
            hasBeenUsed = true;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
            vao.updateBuffer(1, modelMatrices, matricesItterator * sizeof(float) * 16);
            vao.updateIndices(indices, requestedIndicesCount);
            vao.updateIndirectBuffer(drawCommands, requestedObjectItterator);
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
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, System.IntPtr.Zero, requestedObjectItterator, 0);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
