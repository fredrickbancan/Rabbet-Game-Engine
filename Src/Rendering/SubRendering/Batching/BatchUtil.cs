using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;

namespace RabbetGameEngine.SubRendering
{


    //TODO: do VAO building here based on batch type.
    //TODO: add method(s) to help with attempting to add new data to a batch that works with different batch types. These methods can resize the vao buffers and batch arrayts, and will return false if this size exceeds the batches max buffer size.
    public static class BatchUtil
    {
        public static void init()
        {
        }

        //TODO: When building a batch which uses instanced data, be sure to build and add instance data vbo to vao.
        //For multi draw indirect types, the indirect buffer will need to be added. Buffers containing matrices for lerping will also need to be added.
        //Text 3D and lerp text 3D will need a seperate built buffer for positions and prev positions.
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
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    VertexBufferLayout l = new VertexBufferLayout();
                    Vertex.configureLayout(l);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.guiText:
                    ShaderUtil.tryGetShader(ShaderUtil.text2DName, out theBatch.batchShader);
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    VertexBufferLayout l1 = new VertexBufferLayout();
                    Vertex.configureLayout(l1);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l1);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.text3D:
                    ShaderUtil.tryGetShader(ShaderUtil.text3DName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    VertexBufferLayout l2 = new VertexBufferLayout();
                    Vertex.configureLayout(l2);
                    VertexBufferLayout posl = new VertexBufferLayout();
                    posl.add(VertexAttribPointerType.Float, 3);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l2);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 3, posl);//positions
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpText3D:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpText3DName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    VertexBufferLayout l3 = new VertexBufferLayout();
                    Vertex.configureLayout(l3);
                    VertexBufferLayout posl1 = new VertexBufferLayout();
                    posl1.add(VertexAttribPointerType.Float, 3);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l3);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 3, posl1);//positions
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.triangles:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l4 = new VertexBufferLayout();
                    Vertex.configureLayout(l4);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l4);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.quads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l5 = new VertexBufferLayout();
                    Vertex.configureLayout(l5);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l5);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lines:
                    ShaderUtil.tryGetShader(ShaderUtil.linesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l6 = new VertexBufferLayout();
                    Vertex.configureLayout(l6);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l6);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Lines;
                    break;

                case RenderType.iSpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out theBatch.batchShader);
                    theBatch.batchedPoints = new PointParticle[Batch.initialArraySize];
                    VertexBufferLayout l7 = new VertexBufferLayout();
                    PointParticle.configureLayout(l7);
                    vao.addBufferDynamic(Batch.initialArraySize * PointParticle.pParticleByteSize, l7);
                    VertexBufferLayout instl = new VertexBufferLayout();
                    instl.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl);
                    vao.drawType = PrimitiveType.TriangleStrip;
                    break;

                case RenderType.iSpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresTransparentName, out theBatch.batchShader);
                    theBatch.batchedPoints = new PointParticle[Batch.initialArraySize];
                    VertexBufferLayout l8 = new VertexBufferLayout();
                    PointParticle.configureLayout(l8);
                    vao.addBufferDynamic(Batch.initialArraySize * PointParticle.pParticleByteSize, l8);
                    VertexBufferLayout instl1 = new VertexBufferLayout();
                    instl1.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl1);
                    vao.drawType = PrimitiveType.TriangleStrip;
                    break;

                case RenderType.lerpISpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresName, out theBatch.batchShader);
                    theBatch.batchedPoints = new PointParticle[Batch.initialArraySize];
                    VertexBufferLayout l9 = new VertexBufferLayout();
                    PointParticle.configureLayout(l9);
                    vao.addBufferDynamic(Batch.initialArraySize * PointParticle.pParticleByteSize, l9);
                    vao.drawType = PrimitiveType.Points;
                    break;

                case RenderType.lerpTriangles:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l10 = new VertexBufferLayout();
                    Vertex.configureLayout(l10);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l10);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    VertexBufferLayout matl3 = new VertexBufferLayout();
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    matl3.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl3);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpQuads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l11 = new VertexBufferLayout();
                    Vertex.configureLayout(l11);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l11);
                    VertexBufferLayout matl4 = new VertexBufferLayout();
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    matl4.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl4);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpLines:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l12 = new VertexBufferLayout();
                    Vertex.configureLayout(l12);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l12);
                    VertexBufferLayout matl5 = new VertexBufferLayout();
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    matl5.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl5);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Lines;
                    break;

                case RenderType.lerpISpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresTransparentName, out theBatch.batchShader);
                    theBatch.batchedPoints = new PointParticle[Batch.initialArraySize];
                    VertexBufferLayout l13 = new VertexBufferLayout();
                    PointParticle.configureLayout(l13);
                    vao.addBufferDynamic(Batch.initialArraySize * PointParticle.pParticleByteSize, l13);
                    vao.drawType = PrimitiveType.Points;
                    break;

                case RenderType.lerpTrianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l14 = new VertexBufferLayout();
                    Vertex.configureLayout(l14);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l14);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    VertexBufferLayout matl6 = new VertexBufferLayout();
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    matl6.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl6);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.lerpQuadsTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpQuadsTransparentName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l15 = new VertexBufferLayout();
                    Vertex.configureLayout(l15);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l15);
                    VertexBufferLayout matl7 = new VertexBufferLayout();
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    matl7.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl7);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.trianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 3;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], new uint[Batch.initialArraySize]);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l16 = new VertexBufferLayout();
                    Vertex.configureLayout(l16);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l16);
                    vao.addIndicesBufferDynamic(Batch.initialArraySize);
                    VertexBufferLayout matl8 = new VertexBufferLayout();
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    matl8.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl8);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.quadsTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsTransparentName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    theBatch.modelMatrices = new Matrix4[Batch.initialArraySize];
                    VertexBufferLayout l17 = new VertexBufferLayout();
                    Vertex.configureLayout(l17);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l17);
                    VertexBufferLayout matl9 = new VertexBufferLayout();
                    matl9.add(VertexAttribPointerType.Float, 4);
                    matl9.add(VertexAttribPointerType.Float, 4);
                    matl9.add(VertexAttribPointerType.Float, 4);
                    matl9.add(VertexAttribPointerType.Float, 4);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 16, matl9);
                    vao.addIndirectBuffer(Batch.initialArraySize);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case RenderType.spriteCylinder:
                    ShaderUtil.tryGetShader(ShaderUtil.spriteCylinderName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    VertexBufferLayout l18 = new VertexBufferLayout();
                    Sprite3D.configureLayout(l18);
                    vao.addBufferDynamic(Batch.initialArraySize * Sprite3D.sizeInBytes, l18);
                    VertexBufferLayout instl2 = new VertexBufferLayout();
                    instl2.add(VertexAttribPointerType.Float, 2);
                    vao.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float) * 2, instl2);
                    VertexBufferLayout sl = new VertexBufferLayout();
                    sl.add(VertexAttribPointerType.Float, 3);
                    vao.addBufferDynamic(Batch.initialArraySize * sizeof(float) * 3, sl);
                    break;
            }

            vao.finishBuilding();
            theBatch.setVAO(vao);
            theBatch.calculateBatchLimitations();
        }

        //For dynamic vertex objects, when submitting data, the residual un-updated data at the end of the buffer does not need to be cleared.
        //use the submittedVerticesCount or something similar with drawElements(count) to only draw the submitted vertices and ignore the residual ones.
        //This is faster than clearing the whole buffer each update.
        public static bool tryToFitInBatchModel(Model mod, Batch theBatch)
        {

            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return false;

                case RenderType.guiCutout:
                    {

                    }
                    return false;

                case RenderType.guiText:
                    return false;

                case RenderType.text3D:
                    return false;

                case RenderType.lerpText3D:
                    return false;

                case RenderType.triangles:
                    return false;

                case RenderType.quads:
                    return false;

                case RenderType.lines:
                    return false;

                case RenderType.lerpTriangles:
                    return false;

                case RenderType.lerpQuads:
                    return false;

                case RenderType.lerpLines:
                    return false;

                case RenderType.lerpTrianglesTransparent:
                    return false;

                case RenderType.lerpQuadsTransparent:
                    return false;

                case RenderType.trianglesTransparent:
                    return false;

                case RenderType.quadsTransparent:
                    return false;

                case RenderType.spriteCylinder:
                    return false;

                default:
                    return false;
            }
        }

        public static bool tryToFitInBatchPoints(PointCloudModel mod, Batch theBatch)
        {

            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return false;

                case RenderType.iSpheres:
                    return false;

                case RenderType.iSpheresTransparent:
                    return false;

                case RenderType.lerpISpheres:
                    return false;

                case RenderType.lerpISpheresTransparent:
                    return false;

                default:
                    return false;
            }
        }

        public static bool tryToFitInBatchSinglePoint(PointParticle p, Batch theBatch)
        {

            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return false;

                case RenderType.iSpheres:
                    return false;

                case RenderType.iSpheresTransparent:
                    return false;
                default:
                    return false;
            }
        }

        public static bool tryToFitInBatchLerpPoint(PointParticle p, PointParticle prevP, Batch theBatch)
        {

            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return false;

                case RenderType.lerpISpheres:
                    return false;

                case RenderType.lerpISpheresTransparent:
                    return false;

                default:
                    return false;
            }
        }

        public static bool tryToFitInBatchSprite3D(Sprite3D s, Batch theBatch)
        {
            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return false;

                case RenderType.spriteCylinder:
                    return false;
                default:
                    return false;
            }
        }
        public static void drawBatch(Batch theBatch, Matrix4 viewMatrix, Vector3 fogColor)
        {
            theBatch.bindVAO();
            if(theBatch.batchTex != null)
            {
                theBatch.batchTex.use();
            }
            theBatch.batchShader.use();
            theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            theBatch.batchShader.setUniformMat4F("viewMatrix", viewMatrix);
            theBatch.batchShader.setUniformVec3F("fogColor", fogColor);
            theBatch.batchShader.setUniform1F("fogStart", GameInstance.get.currentPlanet.getFogStart());
            theBatch.batchShader.setUniform1F("fogEnd", GameInstance.get.currentPlanet.getFogEnd());
            switch (theBatch.getRenderType())
            {
                case RenderType.none:
                    return;

                case RenderType.guiCutout:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    return;

                case RenderType.guiText:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    return;

                case RenderType.text3D:
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, theBatch.drawCommands, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case RenderType.lerpText3D:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, theBatch.drawCommands, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case RenderType.triangles:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case RenderType.quads:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    return;

                case RenderType.lines:
                    GL.DrawElements(PrimitiveType.Lines, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case RenderType.iSpheres:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    return;

                case RenderType.iSpheresTransparent:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    return;

                case RenderType.lerpISpheres:
                    theBatch.batchShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator / 2);
                    return;

                case RenderType.lerpTriangles:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case RenderType.lerpQuads:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case RenderType.lerpLines:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Lines,  DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case RenderType.lerpISpheresTransparent:
                    theBatch.batchShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator/2);
                    return;

                case RenderType.lerpTrianglesTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case RenderType.lerpQuadsTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case RenderType.trianglesTransparent:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case RenderType.quadsTransparent:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, theBatch.requestedVerticesCount);
                    return;

                case RenderType.spriteCylinder:
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.requestedObjectItterator);
                    return;
            }
        }
    }
}
