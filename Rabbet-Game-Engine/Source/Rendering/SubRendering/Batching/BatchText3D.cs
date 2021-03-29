using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchText3D : Batch
    {
        public BatchText3D(int renderLayer = 0) : base(RenderType.text3D, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.text3DName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 4;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
            positions = new Vector3[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            drawCommands = new DrawCommand[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l2 = new VertexBufferLayout();
            Vertex.configureLayout(l2);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l2);
            VertexBufferLayout posl = new VertexBufferLayout();
            posl.add(VertexAttribPointerType.Float, 3);
            posl.instancedData = true;
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 3, posl);//positions
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            vao.addIndicesBufferDynamic(indices.Length);
            vao.updateIndices(indices, indices.Length);
            vao.drawType = PrimitiveType.Triangles;
        }

        public override bool tryToFitInBatchModel(Model mod)
        {
            int n = vertices.Length;
            if (!BatchUtil.canFitOrResize(ref vertices, mod.vertices.Length, requestedVerticesCount, maxVertexCount)) return false;
            int p = positions.Length;
            if (!BatchUtil.canFitOrResize(ref positions, 1, requestedObjectItterator, maxPositionCount)) return false;
            int i = indices.Length;
            if (BatchUtil.canResizeQuadIndicesIfNeeded(ref indices, mod.vertices.Length + mod.vertices.Length / 2, maxIndiciesCount))
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

            if (positions.Length != p)
            {
                vao.resizeBuffer(1, positions.Length * sizeof(float) * 3);
            }

            System.Array.Copy(mod.vertices, 0, vertices, requestedVerticesCount, mod.vertices.Length);
            positions[requestedObjectItterator] = mod.worldPos;
            configureDrawCommandsForCurrentObject((n = mod.vertices.Length + mod.vertices.Length / 2), true);
            requestedObjectItterator++;
            requestedVerticesCount += mod.vertices.Length;
            requestedIndicesCount += n;
            hasBeenUsed = true;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, vertices, requestedVerticesCount * Vertex.SIZE_BYTES);
            vao.updateBuffer(1, positions, requestedObjectItterator * sizeof(float) * 3);
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
            GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, System.IntPtr.Zero, requestedObjectItterator, 0);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
