using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine.SubRendering
{
    public class BatchLerpTrianglesTransparent : Batch
    {
        public BatchLerpTrianglesTransparent(int renderLayer = 0) : base(RenderType.lerpTrianglesTransparent, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            maxBufferSizeBytes /= 3;
            vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l14 = new VertexBufferLayout();
            Vertex.configureLayout(l14);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l14);
            vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
            VertexBufferLayout matl6 = new VertexBufferLayout();
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            matl6.add(VertexAttribPointerType.Float, 4);
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl6);
            vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
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
            batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
            GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, System.IntPtr.Zero, requestedObjectItterator, 0);
            vao.unBind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
