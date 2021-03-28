using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Rendering;
using System;

namespace RabbetGameEngine.SubRendering
{
    //TODO: Major Rework: this is trash
    public static class BatchUtil
    {
        public static void init()
        {
        }

        public static Batch getBatchForType(RenderType type, int renderLayer = 0)
        {
            switch (type)
            {
                case RenderType.none:
                    return null;
                case RenderType.MARKER_GUI_START:
                    return null;
                case RenderType.guiCutout:
                    return new BatchGuiCutOut(renderLayer);
                case RenderType.MARKER_TRANSPARENT_START:
                    return null;
                case RenderType.guiLines:
                    return null;
                case RenderType.guiText:
                    return null;
                case RenderType.guiTransparent:
                    return null;
                case RenderType.MARKER_GUI_END:
                    return null;
                case RenderType.iSpheresTransparent:
                    return null;
                case RenderType.trianglesTransparent:
                    return null;
                case RenderType.quadsTransparent:
                    return null;
                case RenderType.MARKER_LERP_START:
                    return null;
                case RenderType.lerpISpheresTransparent:
                    return null;
                case RenderType.lerpTrianglesTransparent:
                    return null;
                case RenderType.lerpQuadsTransparent:
                    return null;
                case RenderType.MARKER_TRANSPARENT_END:
                    return null;
                case RenderType.lerpText3D:
                    return null;
                case RenderType.lerpISpheres:
                    return null;
                case RenderType.lerpTriangles:
                    return null;
                case RenderType.lerpQuads:
                    return null;
                case RenderType.lerpLines:
                    return null;
                case RenderType.MARKER_LERP_END:
                    return null;
                case RenderType.text3D:
                    return null;
                case RenderType.triangles:
                    return null;
                case RenderType.quads:
                    return null;
                case RenderType.lines:
                    return null;
                case RenderType.iSpheres:
                    return null;
                case RenderType.spriteCylinder:
                    return null;
                default:
                    return null;
            }
        }

        public static void buildBatch(Batch theBatch)
        {
            VertexArrayObject vao = new VertexArrayObject();
            vao.beginBuilding();

            switch(theBatch.getRenderType())
            {
                case RenderType.none:
                    ShaderUtil.tryGetShader("debug", out theBatch.batchShader);
                    break;

                case RenderType.guiCutout:
                    ShaderUtil.tryGetShader(ShaderUtil.guiCutoutName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    VertexBufferLayout l = new VertexBufferLayout();
                    Vertex.configureLayout(l);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l);
                    vao.addIndicesBufferDynamic(theBatch.indices.Length);
                    vao.updateIndices(theBatch.indices, theBatch.indices.Length);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.guiLines:
                    ShaderUtil.tryGetShader(ShaderUtil.guiLinesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout ll = new VertexBufferLayout();
                    Vertex.configureLayout(ll);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, ll);
                    vao.drawType = PrimitiveType.Lines;
                    break;

                case RenderType.guiTransparent:
                    {
                        ShaderUtil.tryGetShader(ShaderUtil.guiTransparentName, out theBatch.batchShader);
                        theBatch.batchShader.use();
                        theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                        theBatch.maxBufferSizeBytes /= 2;
                        theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                        theBatch.indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                        VertexBufferLayout lt = new VertexBufferLayout();
                        Vertex.configureLayout(lt);
                        vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, lt);
                        vao.addIndicesBufferDynamic(theBatch.indices.Length);
                        vao.updateIndices(theBatch.indices, theBatch.indices.Length);
                        vao.drawType = PrimitiveType.Triangles;
                    }
                    break;

                case RenderType.guiText:
                    ShaderUtil.tryGetShader(ShaderUtil.text2DName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    VertexBufferLayout l1 = new VertexBufferLayout();
                    Vertex.configureLayout(l1);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l1);
                    vao.addIndicesBufferDynamic(theBatch.indices.Length);
                    vao.updateIndices(theBatch.indices, theBatch.indices.Length);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.text3D:
                    ShaderUtil.tryGetShader(ShaderUtil.text3DName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 4;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    theBatch.positions = new Vector3[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.drawCommands = new DrawCommand[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l2 = new VertexBufferLayout();
                    Vertex.configureLayout(l2);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l2);
                    VertexBufferLayout posl = new VertexBufferLayout();
                    posl.add(VertexAttribPointerType.Float, 3);
                    posl.instancedData = true;
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 3, posl);//positions
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.addIndicesBufferDynamic(theBatch.indices.Length);
                    vao.updateIndices(theBatch.indices, theBatch.indices.Length);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpText3D:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpText3DName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 4;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.drawCommands = new DrawCommand[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.positions = new Vector3[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    VertexBufferLayout l3 = new VertexBufferLayout();
                    Vertex.configureLayout(l3);
                    VertexBufferLayout posl1 = new VertexBufferLayout();
                    posl1.add(VertexAttribPointerType.Float, 3);
                    posl1.add(VertexAttribPointerType.Float, 3);
                    posl1.instancedData = true;
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l3);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 3, posl1);//positions
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.addIndicesBufferDynamic(theBatch.indices.Length);
                    vao.updateIndices(theBatch.indices, theBatch.indices.Length);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.triangles:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l4 = new VertexBufferLayout();
                    Vertex.configureLayout(l4);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l4);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.quads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l5 = new VertexBufferLayout();
                    Vertex.configureLayout(l5);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l5);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lines:
                    ShaderUtil.tryGetShader(ShaderUtil.linesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l6 = new VertexBufferLayout();
                    Vertex.configureLayout(l6);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l6);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.drawType = PrimitiveType.Lines;
                    break;

                case RenderType.iSpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l7 = new VertexBufferLayout();
                    PointParticle.configureLayout(l7);
                    l7.instancedData = true;
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l7);
                    VertexBufferLayout instl = new VertexBufferLayout();
                    instl.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl);
                    vao.drawType = PrimitiveType.TriangleStrip;
                    break;

                case RenderType.iSpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresTransparentName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l8 = new VertexBufferLayout();
                    PointParticle.configureLayout(l8);
                    l8.instancedData = true;
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l8);
                    VertexBufferLayout instl1 = new VertexBufferLayout();
                    instl1.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl1);
                    vao.drawType = PrimitiveType.TriangleStrip;
                    break;

                case RenderType.lerpISpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
                    theBatch.batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l9 = new VertexBufferLayout();
                    PointParticle.configureLayout(l9);
                    PointParticle.configureLayout(l9);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l9);
                    vao.drawType = PrimitiveType.Points;
                    break;

                case RenderType.lerpTriangles:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 4;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.drawCommands = new DrawCommand[RenderConstants.INIT_BATCH_ARRAY_SIZE];
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
                    break;

                case RenderType.lerpQuads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l11 = new VertexBufferLayout();
                    Vertex.configureLayout(l11);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l11);
                    VertexBufferLayout matl4 = new VertexBufferLayout();
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl4);
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpLines:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l12 = new VertexBufferLayout();
                    Vertex.configureLayout(l12);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l12);
                    VertexBufferLayout matl5 = new VertexBufferLayout();
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl5);
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.drawType = PrimitiveType.Lines;
                    break;

                case RenderType.lerpISpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresTransparentName, out theBatch.batchShader); 
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
                    theBatch.batchedPoints = new PointParticle[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l13 = new VertexBufferLayout();
                    PointParticle.configureLayout(l13);
                    PointParticle.configureLayout(l13);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * PointParticle.SIZE_BYTES, l13);
                    vao.drawType = PrimitiveType.Points;
                    break;

                case RenderType.lerpTrianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader); 
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
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
                    break;

                case RenderType.lerpQuadsTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpQuadsTransparentName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l15 = new VertexBufferLayout();
                    Vertex.configureLayout(l15);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l15);
                    VertexBufferLayout matl7 = new VertexBufferLayout();
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl7);
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.trianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.indices = new uint[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    theBatch.modelMatrices = new Matrix4[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l16 = new VertexBufferLayout();
                    Vertex.configureLayout(l16);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l16);
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    VertexBufferLayout matl8 = new VertexBufferLayout();
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * sizeof(float) * 16, matl8);
                    vao.addIndirectBuffer(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.quadsTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsTransparentName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.vertices = new Vertex[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l17 = new VertexBufferLayout();
                    Vertex.configureLayout(l17);
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Vertex.SIZE_BYTES, l17); 
                    vao.addIndicesBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE);
                    vao.updateIndices(QuadCombiner.getIndicesForQuadCount(RenderConstants.INIT_BATCH_ARRAY_SIZE / 6), RenderConstants.INIT_BATCH_ARRAY_SIZE / 6);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.spriteCylinder:
                    ShaderUtil.tryGetShader(ShaderUtil.spriteCylinderName, out theBatch.batchShader);
                    theBatch.batchShader.use();
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.sprites3D = new Sprite3D[RenderConstants.INIT_BATCH_ARRAY_SIZE];
                    VertexBufferLayout l18 = new VertexBufferLayout();
                    Sprite3D.configureLayout(l18);
                    l18.instancedData = true;
                    vao.addBufferDynamic(RenderConstants.INIT_BATCH_ARRAY_SIZE * Sprite3D.sizeInBytes, l18);
                    VertexBufferLayout instl2 = new VertexBufferLayout();
                    instl2.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl2);
                    vao.drawType = PrimitiveType.TriangleStrip;
                    break;
            }

            vao.finishBuilding();
            theBatch.setVAO(vao);
            theBatch.calculateBatchLimitations();
            theBatch.hasBeenUsed = true;
        }


        public static bool tryToFitInBatchModel(Model mod, Batch theBatch)
        {
            int n;
            theBatch.hasBeenUsed = true;
            switch (theBatch.getRenderType())
            {
                case RenderType.guiCutout:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int i = theBatch.indices.Length;
                        if (canResizeQuadIndicesIfNeeded(ref theBatch.indices, theBatch.requestedVerticesCount + mod.vertices.Length, theBatch.maxIndiciesCount))
                        {
                            if (i != theBatch.indices.Length)
                            {
                                theBatch.VAO.resizeIndices(theBatch.indices.Length);
                                theBatch.VAO.updateIndices(theBatch.indices, theBatch.indices.Length);
                            }
                        }
                        else return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                    }
                    break;

                case RenderType.guiLines:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                    }
                    break;

                case RenderType.guiTransparent:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int i = theBatch.indices.Length;
                        if (canResizeQuadIndicesIfNeeded(ref theBatch.indices, theBatch.requestedVerticesCount + mod.vertices.Length, theBatch.maxIndiciesCount))
                        {
                            if (i != theBatch.indices.Length)
                            {
                                theBatch.VAO.resizeIndices(theBatch.indices.Length);
                                theBatch.VAO.updateIndices(theBatch.indices, theBatch.indices.Length);
                            }
                        }
                        else return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                    }
                    break;

                case RenderType.guiText:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int i = theBatch.indices.Length;
                        if (canResizeQuadIndicesIfNeeded(ref theBatch.indices, theBatch.requestedVerticesCount + mod.vertices.Length, theBatch.maxIndiciesCount))
                        {
                            if (i != theBatch.indices.Length)
                            {
                                theBatch.VAO.resizeIndices(theBatch.indices.Length);
                                theBatch.VAO.updateIndices(theBatch.indices, theBatch.indices.Length);
                            }
                        }
                        else return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                    }
                    break;

                case RenderType.text3D:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int p = theBatch.positions.Length;
                        if (!canFitOrResize(ref theBatch.positions, 1, theBatch.requestedObjectItterator, theBatch.maxPositionCount)) return false;
                        int i = theBatch.indices.Length;
                        if (canResizeQuadIndicesIfNeeded(ref theBatch.indices, mod.vertices.Length + mod.vertices.Length / 2, theBatch.maxIndiciesCount))
                        {
                            if (i != theBatch.indices.Length)
                            {
                                theBatch.VAO.resizeIndices(theBatch.indices.Length);
                                theBatch.VAO.updateIndices(theBatch.indices, theBatch.indices.Length);
                            }
                        }
                        else return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        if(theBatch.positions.Length != p)
                        {
                            theBatch.VAO.resizeBuffer(1, theBatch.positions.Length * sizeof(float) * 3);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.positions[theBatch.requestedObjectItterator] = mod.worldPos;
                        theBatch.configureDrawCommandsForCurrentObject((n = mod.vertices.Length + mod.vertices.Length / 2), true);
                        theBatch.requestedObjectItterator++;
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                        theBatch.requestedIndicesCount += n;
                    }
                    break;

                case RenderType.lerpText3D:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int p = theBatch.positions.Length;
                        if (!canFitOrResize(ref theBatch.positions, 2, theBatch.positionItterator, theBatch.maxPositionCount)) return false;
                        int i = theBatch.indices.Length;
                        if (canResizeQuadIndicesIfNeeded(ref theBatch.indices, mod.vertices.Length + mod.vertices.Length / 2, theBatch.maxIndiciesCount))
                        {
                            if (i != theBatch.indices.Length)
                            {
                                theBatch.VAO.resizeIndices(theBatch.indices.Length);
                                theBatch.VAO.updateIndices(theBatch.indices, theBatch.indices.Length);
                            }
                        }
                        else return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        if (theBatch.positions.Length != p)
                        {
                            theBatch.VAO.resizeBuffer(1, theBatch.positions.Length * sizeof(float) * 3);
                        }

                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        theBatch.positions[theBatch.positionItterator] = mod.worldPos;
                        theBatch.positions[theBatch.positionItterator + 1] = mod.prevWorldPos;
                        theBatch.positionItterator += 2;
                        theBatch.configureDrawCommandsForCurrentObject((n = mod.vertices.Length + mod.vertices.Length / 2), true);
                        theBatch.requestedObjectItterator++;
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                        theBatch.requestedIndicesCount += n;
                    }
                    break;

                case RenderType.triangles:
                    {
                        return false;
                    }
                case RenderType.quads:
                    {
                        return false;
                    }

                case RenderType.lines:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int i = theBatch.indices.Length;
                        if (!canFitOrResize(ref theBatch.indices, mod.indices.Length, theBatch.requestedIndicesCount, theBatch.maxIndiciesCount)) return false;
                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        if (theBatch.indices.Length != i)
                        {
                            theBatch.VAO.resizeIndices(theBatch.indices.Length);
                        }
                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);

                        for(i = 0; i < mod.indices.Length; i++)
                        {
                            theBatch.indices[theBatch.requestedIndicesCount + i] = (uint)(mod.indices[i] + theBatch.requestedVerticesCount);
                        }
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                        theBatch.requestedIndicesCount += mod.indices.Length;
                    }
                    break;

                case RenderType.lerpTriangles:
                    {
                        n = theBatch.vertices.Length;
                        if (!canFitOrResize(ref theBatch.vertices, mod.vertices.Length, theBatch.requestedVerticesCount, theBatch.maxVertexCount)) return false;
                        int p = theBatch.modelMatrices.Length;
                        if (!canFitOrResize(ref theBatch.modelMatrices, 2, theBatch.matricesItterator, theBatch.maxPositionCount)) return false;
                        int i = theBatch.indices.Length;
                        if (!canFitOrResize(ref theBatch.indices, mod.indices.Length, theBatch.requestedIndicesCount, theBatch.maxIndiciesCount)) return false;

                        if (theBatch.vertices.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.vertices.Length * Vertex.SIZE_BYTES);
                        }

                        if (theBatch.modelMatrices.Length != p)
                        {
                            theBatch.VAO.resizeBuffer(1, theBatch.modelMatrices.Length * sizeof(float) * 16);
                        }

                        if(theBatch.indices.Length != i)
                        {
                            theBatch.VAO.resizeIndices(theBatch.indices.Length);
                        }

                        theBatch.configureDrawCommandsForCurrentObject(mod.indices.Length, false);
                        Array.Copy(mod.vertices, 0, theBatch.vertices, theBatch.requestedVerticesCount, mod.vertices.Length);
                        Array.Copy(mod.indices, 0, theBatch.indices, theBatch.requestedIndicesCount, mod.indices.Length);
                        theBatch.modelMatrices[theBatch.matricesItterator] = mod.modelMatrix;
                        theBatch.modelMatrices[theBatch.matricesItterator + 1] = mod.prevModelMatrix;
                        theBatch.matricesItterator += 2;
                        theBatch.requestedObjectItterator++;
                        theBatch.requestedVerticesCount += mod.vertices.Length;
                        theBatch.requestedIndicesCount += mod.indices.Length;
                    }
                    break;

                case RenderType.lerpQuads:
                    {
                        return false;
                    }

                case RenderType.lerpLines:
                    {
                        return false;
                    }

                case RenderType.lerpTrianglesTransparent:
                    {
                        return false;
                    }

                case RenderType.lerpQuadsTransparent:
                    {
                        return false;
                    }

                case RenderType.trianglesTransparent:
                    {
                        return false;
                    }

                case RenderType.quadsTransparent:
                    {
                        return false;
                    }
            }
            return true;
        }
        
        
        /// <summary>
        /// Returns true if the dest indices array needs to be resized and is successfully resized to accomidate the number of total vertices for a quad based model based on the number of vertices of the model provided, and the maximum array size provided.
        /// </summary>
        /// <param name="dstIndices">Destination array to be tested and resized</param>
        /// <param name="totalVertices">Number of vertices to be accomidated</param>
        /// <param name="maxDstSize">Maximum allowed size for the array</param>
        public static bool canResizeQuadIndicesIfNeeded(ref uint[] dstIndices, int totalVertices, int maxDstSize)
        {
            int n;
            if ((n = totalVertices + (totalVertices / 2)) >= maxDstSize)
            {
                return false;
            }
            if(n >= dstIndices.Length)
            {
                if((n *= 2) >= maxDstSize)
                {
                    dstIndices = QuadCombiner.getIndicesForQuadCount(maxDstSize / 6);
                }
                else
                {
                    dstIndices = QuadCombiner.getIndicesForQuadCount(n / 6);
                }
            }
            return true;
        }

        public static bool tryToFitInBatchPoints(PointCloudModel mod, Batch theBatch)
        {
            int n;
            theBatch.hasBeenUsed = true;

            switch (theBatch.getRenderType())
            {
                case RenderType.iSpheres:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, mod.points.Length, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        Array.Copy(mod.points, 0, theBatch.batchedPoints, theBatch.pointsItterator, mod.points.Length);
                        theBatch.pointsItterator += mod.points.Length;
                    }
                    break;

                case RenderType.iSpheresTransparent:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, mod.points.Length, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        Array.Copy(mod.points, 0, theBatch.batchedPoints, theBatch.pointsItterator, mod.points.Length);
                        theBatch.pointsItterator += mod.points.Length;
                    }
                    break;

                case RenderType.lerpISpheres:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, mod.points.Length * 2, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if(theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        for (n = 0; n < mod.points.Length; n++)
                        {
                            theBatch.batchedPoints[theBatch.pointsItterator] = mod.points[n];
                            theBatch.batchedPoints[theBatch.pointsItterator + 1] = mod.prevPoints[n];
                            theBatch.pointsItterator += 2;
                        }
                    }
                    break;

                case RenderType.lerpISpheresTransparent:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, mod.points.Length * 2, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        for (n = 0; n < mod.points.Length; n++)
                        {
                            theBatch.batchedPoints[theBatch.pointsItterator] = mod.points[n];
                            theBatch.batchedPoints[theBatch.pointsItterator + 1] = mod.prevPoints[n];
                            theBatch.pointsItterator += 2;
                        }
                    }
                    break;
            }
            return true;
        }

        public static bool tryToFitInBatchSinglePoint(PointParticle p, Batch theBatch)
        {
            int n;
            theBatch.hasBeenUsed = true;
            switch (theBatch.getRenderType())
            {
                case RenderType.iSpheres:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, 1, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        theBatch.batchedPoints[theBatch.pointsItterator++] = p;
                    }
                    break;

                case RenderType.iSpheresTransparent:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, 1, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        theBatch.batchedPoints[theBatch.pointsItterator++] = p;
                    }
                    break;
            }
            return true;
        }

        public static bool tryToFitInBatchLerpPoint(PointParticle p, PointParticle prevP, Batch theBatch)
        {
            int n;
            theBatch.hasBeenUsed = true;
            switch (theBatch.getRenderType())
            {
                case RenderType.lerpISpheres:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, 2, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        theBatch.batchedPoints[theBatch.pointsItterator] = p;
                        theBatch.batchedPoints[theBatch.pointsItterator + 1] = prevP;
                        theBatch.pointsItterator += 2;
                    }
                    break;

                case RenderType.lerpISpheresTransparent:
                    {
                        n = theBatch.batchedPoints.Length;
                        if (!canFitOrResize(ref theBatch.batchedPoints, 2, theBatch.pointsItterator, theBatch.maxPointCount)) return false;
                        if (theBatch.batchedPoints.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.batchedPoints.Length * PointParticle.SIZE_BYTES);
                        }
                        theBatch.batchedPoints[theBatch.pointsItterator] = p;
                        theBatch.batchedPoints[theBatch.pointsItterator + 1] = prevP;
                        theBatch.pointsItterator += 2;
                    }
                    break;
            }

            return true;
        }

        public static bool tryToFitInBatchSprite3D(Sprite3D s, Batch theBatch)
        {
            int n;
            theBatch.hasBeenUsed = true;
            switch (theBatch.getRenderType())
            {
                case RenderType.spriteCylinder:
                    {
                        n = theBatch.sprites3D.Length;
                        if (!canFitOrResize(ref theBatch.sprites3D, 1, theBatch.spriteItterator, theBatch.maxSprite3DCount)) return false;

                        if (theBatch.sprites3D.Length != n)
                        {
                            theBatch.VAO.resizeBuffer(0, theBatch.sprites3D.Length * Sprite3D.sizeInBytes);
                        }
                        theBatch.sprites3D[theBatch.spriteItterator++] = s;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Returns true if an array of length srcSize can fit into the dstArray, or if the dstArray has been resized and can now accept it.
        /// If adding the srcSize goes over maxDstSize, then returns false.
        /// When resizing, simply doubles current dst array size, or sets dst array size to maxDstSize.
        /// int dstFilled is how much of the dstArray is used.
        /// </summary>
        public static bool canFitOrResize<T2>(ref T2[] dstArr, int srcSize, int dstFilled, int maxDstSize) where T2 : struct
        {
            int n;
            if ((n = dstFilled + srcSize) >= maxDstSize) return false;
            if (n >= dstArr.Length)
            {
                if ((n *= 2) >= maxDstSize)
                {
                    Array.Resize<T2>(ref dstArr, maxDstSize);
                }
                else
                {
                    Array.Resize<T2>(ref dstArr, n);
                }
            }
            return true;
        }

        public static void updateBuffers(Batch theBatch)
        {
            switch (theBatch.getRenderType())
            {
                case RenderType.guiCutout:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    return;

                case RenderType.guiLines:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    return;

                case RenderType.guiTransparent:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    return;

                case RenderType.guiText:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    return;

                case RenderType.text3D:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    theBatch.VAO.updateBuffer(1, theBatch.positions, theBatch.requestedObjectItterator * sizeof(float) * 3);
                    theBatch.VAO.updateIndirectBuffer(theBatch.drawCommands, theBatch.requestedObjectItterator);
                    return;

                case RenderType.lerpText3D:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    theBatch.VAO.updateBuffer(1, theBatch.positions, theBatch.positionItterator * sizeof(float) * 3);
                    theBatch.VAO.updateIndirectBuffer(theBatch.drawCommands, theBatch.requestedObjectItterator);
                    return;

                case RenderType.triangles:
                    return;

                case RenderType.quads:
                    return;

                case RenderType.lines:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    theBatch.VAO.updateIndices(theBatch.indices, theBatch.requestedIndicesCount);
                    return;

                case RenderType.iSpheres:
                    theBatch.VAO.updateBuffer(0, theBatch.batchedPoints, theBatch.pointsItterator * PointParticle.SIZE_BYTES);
                    return;

                case RenderType.iSpheresTransparent:
                    theBatch.VAO.updateBuffer(0, theBatch.batchedPoints, theBatch.pointsItterator * PointParticle.SIZE_BYTES);
                    return;

                case RenderType.lerpISpheres:
                    theBatch.VAO.updateBuffer(0, theBatch.batchedPoints, theBatch.pointsItterator * PointParticle.SIZE_BYTES);
                    return;

                case RenderType.lerpTriangles:
                    theBatch.VAO.updateBuffer(0, theBatch.vertices, theBatch.requestedVerticesCount * Vertex.SIZE_BYTES);
                    theBatch.VAO.updateBuffer(1, theBatch.modelMatrices, theBatch.matricesItterator * sizeof(float) * 16);
                    theBatch.VAO.updateIndices(theBatch.indices, theBatch.requestedIndicesCount);
                    theBatch.VAO.updateIndirectBuffer(theBatch.drawCommands, theBatch.requestedObjectItterator);
                    return;

                case RenderType.lerpQuads:
                    return;

                case RenderType.lerpLines:
                    return;

                case RenderType.lerpISpheresTransparent:
                    theBatch.VAO.updateBuffer(0, theBatch.batchedPoints, theBatch.pointsItterator * PointParticle.SIZE_BYTES);
                    return;

                case RenderType.lerpTrianglesTransparent:
                    return;

                case RenderType.lerpQuadsTransparent:
                    return;

                case RenderType.trianglesTransparent:
                    return;

                case RenderType.quadsTransparent:
                    return;

                case RenderType.spriteCylinder:
                    theBatch.VAO.updateBuffer(0, theBatch.sprites3D, theBatch.spriteItterator * Sprite3D.sizeInBytes);
                    return;
            }
        }

        //TODO: have current world provided as a paremeter instead of fog and view
        public static void drawBatch(Batch theBatch, Matrix4 viewMatrix, Vector3 fogColor)
        {
            theBatch.bindVAO();
            if (theBatch.batchTex != null)
            {
                theBatch.batchTex.use();
            }
            theBatch.batchShader.use();
            theBatch.batchShader.setUniformMat4F("viewMatrix", viewMatrix);
            theBatch.batchShader.setUniformVec3F("fogColor", fogColor);           
            theBatch.batchShader.setUniform1F("fogStart", GameInstance.get.currentPlanet.getFogStart());
            theBatch.batchShader.setUniform1F("fogEnd", GameInstance.get.currentPlanet.getFogEnd());
            switch (theBatch.getRenderType())
            {
                case RenderType.guiCutout:
                    GL.DepthMask(false);
                    GL.DepthRange(0, 0.005F);
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedVerticesCount + (theBatch.requestedVerticesCount / 2), DrawElementsType.UnsignedInt, 0);
                    GL.DepthRange(0, 1);
                    GL.DepthMask(true);
                    break;

                case RenderType.guiLines:
                    GL.LineWidth(GUIManager.guiLineWidth);
                    GL.DepthMask(false);
                    GL.DepthRange(0, 0.005F);
                    GL.DrawArrays(PrimitiveType.Lines, 0, theBatch.requestedVerticesCount);
                    GL.DepthRange(0, 1);
                    GL.DepthMask(true);
                    GL.LineWidth(Renderer.defaultLineWidthInPixels);
                    break;

                case RenderType.guiTransparent:
                    GL.DepthMask(false);
                    GL.DepthRange(0, 0.005F);
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedVerticesCount + (theBatch.requestedVerticesCount / 2), DrawElementsType.UnsignedInt, 0);
                    GL.DepthRange(0, 1);
                    GL.DepthMask(true);
                    break;

                case RenderType.guiText:
                    GL.DepthMask(false);
                    GL.DepthRange(0,0.005F);
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedVerticesCount + (theBatch.requestedVerticesCount / 2), DrawElementsType.UnsignedInt, 0);
                    GL.DepthRange(0,1);
                    GL.DepthMask(true);
                    break;

                case RenderType.text3D:
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    break;

                case RenderType.lerpText3D:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    break;

                case RenderType.triangles:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.indices);
                    break;

                case RenderType.quads:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    break;

                case RenderType.lines:
                    GL.DrawElements(PrimitiveType.Lines, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt,0);
                    break;

                case RenderType.iSpheres:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    break;

                case RenderType.iSpheresTransparent:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    break;

                case RenderType.lerpISpheres:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator / 2);
                    break;

                case RenderType.lerpTriangles:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    break;

                case RenderType.lerpQuads:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    break;

                case RenderType.lerpLines:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Lines,  DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    break;

                case RenderType.lerpISpheresTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator/2);
                    break;

                case RenderType.lerpTrianglesTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    break;

                case RenderType.lerpQuadsTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    break;

                case RenderType.trianglesTransparent:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.indices);
                    break;

                case RenderType.quadsTransparent:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    break;

                case RenderType.spriteCylinder:
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.spriteItterator);
                    break;
            }
            theBatch.VAO.unBind();
        }

        public static void updateUniformsForBatch(Batch theBatch)
        {
            theBatch.batchShader.use();
            switch (theBatch.getRenderType())
            {
                case RenderType.guiCutout:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    break;
                case RenderType.guiLines:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    break;
                case RenderType.guiText:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    break;
                case RenderType.guiTransparent:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    break;
                case RenderType.iSpheresTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.trianglesTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.quadsTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpISpheresTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
                    break;
                case RenderType.lerpTrianglesTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpQuadsTransparent:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpText3D:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpISpheres:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    theBatch.batchShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
                    break;
                case RenderType.lerpTriangles:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpQuads:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lerpLines:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.text3D:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.triangles:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.quads:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.lines:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.iSpheres:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
                case RenderType.spriteCylinder:
                    theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                    break;
            }
        }

    }
}
