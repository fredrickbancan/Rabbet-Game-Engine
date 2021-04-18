using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Rendering;
namespace RabbetGameEngine
{
    public class BatchSpriteCylinder : Batch
    {
        public BatchSpriteCylinder(int renderLayer = 0) : base(RenderType.spriteCylinder, renderLayer)
        {
        }

        protected override void buildBatch()
        {
            ShaderUtil.tryGetShader(ShaderUtil.spriteCylinderName, out batchShader);
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            sprites3D = new Sprite3D[RenderConstants.INIT_BATCH_ARRAY_SIZE];
            VertexBufferLayout l18 = new VertexBufferLayout();
            Sprite3D.configureLayout(l18);
            l18.instancedData = true;
            vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Sprite3D.sizeInBytes, l18);
            VertexBufferLayout instl2 = new VertexBufferLayout();
            instl2.add(VertexAttribPointerType.Float, 2);
            vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl2);
            vao.drawType = PrimitiveType.TriangleStrip;
        }

        public override bool tryToFitInBatchSprite3D(Sprite3D s, Texture tex = null)
        {
            if (!tryToAddSpriteTexAndApplyIndex(s, tex)) return false;

            int n = sprites3D.Length;
            if (!BatchUtil.canFitOrResize(ref sprites3D, 1, spriteItterator, maxSprite3DCount)) return false;

            if (sprites3D.Length != n)
            {
                vao.resizeBuffer(0, sprites3D.Length * Sprite3D.sizeInBytes);
            }
            sprites3D[spriteItterator++] = s;
            hasBeenUsed = true;
            return true;
        }

        public override void updateBuffers()
        {
            vao.updateBuffer(0, sprites3D, spriteItterator * Sprite3D.sizeInBytes);
        }

        public override void updateUniforms(World thePlanet)
        {
            batchShader.use();
            batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
        }

        public override void drawBatch(World thePlanet)
        {
            vao.bind();
            bindAllTextures();
            batchShader.use();
            batchShader.setUniformMat4F("viewMatrix", Renderer.viewMatrix);
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, spriteItterator);
            vao.unBind();
        }
    }
}
