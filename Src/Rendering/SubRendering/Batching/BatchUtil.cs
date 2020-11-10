using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine.SubRendering
{
    //TODO: do VAO building here based on batch type.
    //TODO: add method(s) to help with attempting to add new data to a batch that works with different batch types. These methods can resize the vao buffers and batch arrayts, and will return false if this size exceeds the batches max buffer size.
    public static class BatchUtil
    {
        private static Dictionary<BatchType, VertexBufferLayout> layouts;

        /// <summary>
        /// Generates all layouts for each batch type to be used when creating batches.
        /// </summary>
        public static void init()
        {
            layouts = new Dictionary<BatchType, VertexBufferLayout>();
            int count = Enum.GetNames(typeof(BatchType)).Length;

            for(int i = 0; i < count; i++)
            {
                switch ((BatchType)i)
                {
                    case BatchType.none:
                        break;

                    case BatchType.guiCutout:
                        VertexBufferLayout l = new VertexBufferLayout();
                        Vertex.configureLayout(l);
                        layouts.Add(BatchType.guiCutout, l);
                        break;

                    case BatchType.guiText:
                        VertexBufferLayout l1 = new VertexBufferLayout();
                        Vertex.configureLayout(l1);
                        layouts.Add(BatchType.guiText, l1);
                        break;

                    case BatchType.text3D:
                        VertexBufferLayout l2 = new VertexBufferLayout();
                        Vertex.configureLayout(l2);
                        layouts.Add(BatchType.text3D, l2);
                        break;

                    case BatchType.lerpText3D:
                        VertexBufferLayout l3 = new VertexBufferLayout();
                        Vertex.configureLayout(l3);
                        layouts.Add(BatchType.lerpText3D, l3);
                        break;

                    case BatchType.triangles:
                        VertexBufferLayout l4 = new VertexBufferLayout();
                        Vertex.configureLayout(l4);
                        layouts.Add(BatchType.triangles, l4);
                        break;

                    case BatchType.quads:
                        VertexBufferLayout l5 = new VertexBufferLayout();
                        Vertex.configureLayout(l5);
                        layouts.Add(BatchType.quads, l5);
                        break;

                    case BatchType.lines:
                        VertexBufferLayout l6 = new VertexBufferLayout();
                        Vertex.configureLayout(l6);
                        layouts.Add(BatchType.lines, l6);
                        break;

                    case BatchType.iSpheres:
                        VertexBufferLayout l7 = new VertexBufferLayout();
                        PointParticle.configureLayout(l7);//points
                        layouts.Add(BatchType.iSpheres, l7);
                        break;

                    case BatchType.iSpheresTransparent:
                        VertexBufferLayout l8 = new VertexBufferLayout();
                        PointParticle.configureLayout(l8);//points
                        layouts.Add(BatchType.iSpheresTransparent, l8);
                        break;

                    case BatchType.lerpISpheres:
                        VertexBufferLayout l9 = new VertexBufferLayout();
                        PointParticle.configureLayout(l9);//points
                        PointParticle.configureLayout(l9);//prev points
                        layouts.Add(BatchType.lerpISpheres, l9);
                        break;

                    case BatchType.lerpTriangles:
                        VertexBufferLayout l10 = new VertexBufferLayout();
                        Vertex.configureLayout(l10);
                        layouts.Add(BatchType.lerpTriangles, l10);
                        break;

                    case BatchType.lerpQuads:
                        VertexBufferLayout l11 = new VertexBufferLayout();
                        Vertex.configureLayout(l11);
                        layouts.Add(BatchType.lerpQuads, l11);
                        break;

                    case BatchType.lerpLines:
                        VertexBufferLayout l12 = new VertexBufferLayout();
                        Vertex.configureLayout(l12);
                        layouts.Add(BatchType.lerpLines, l12);
                        break;

                    case BatchType.lerpISpheresTransparent:
                        VertexBufferLayout l13 = new VertexBufferLayout();
                        PointParticle.configureLayout(l13);//points
                        PointParticle.configureLayout(l13);//prev points
                        layouts.Add(BatchType.lerpISpheres, l13);
                        break;

                    case BatchType.lerpTrianglesTransparent:
                        VertexBufferLayout l14 = new VertexBufferLayout();
                        Vertex.configureLayout(l14);
                        layouts.Add(BatchType.lerpTrianglesTransparent, l14);
                        break;

                    case BatchType.lerpQuadsTransparent:
                        VertexBufferLayout l15 = new VertexBufferLayout();
                        Vertex.configureLayout(l15);
                        layouts.Add(BatchType.lerpQuadsTransparent, l15);
                        break;

                    case BatchType.trianglesTransparent:
                        VertexBufferLayout l16 = new VertexBufferLayout();
                        Vertex.configureLayout(l16);
                        layouts.Add(BatchType.trianglesTransparent, l16);
                        break;

                    case BatchType.spriteCylinder:
                        VertexBufferLayout l17 = new VertexBufferLayout();
                        Sprite3D.configureLayout(l17);
                        layouts.Add(BatchType.spriteCylinder, l17);
                        break;

                    default:
                        break;
                }
            }
        }

        //TODO: When building a batch which uses instanced data, be sure to build and add instance data vbo to vao.
        //For multi draw indirect types, the indirect buffer will need to be added. Buffers containing matrices for lerping will also need to be added.
        //Text 3D and lerp text 3D will need a seperate built buffer for positions and prev positions.

        public static void buildBatch(Batch theBatch)
        {
            VertexArrayObject vao = new VertexArrayObject();
            vao.beginBuilding();

            switch(theBatch.getBatchType())
            {
                case BatchType.none:
                    ShaderUtil.tryGetShader("debug", out theBatch.batchShader);
                    break;

                case BatchType.guiCutout:
                    ShaderUtil.tryGetShader(ShaderUtil.guiCutoutName, out theBatch.batchShader);
                    theBatch.maxBufferSizeBytes /= 2;
                    theBatch.batchedModel = new Model(new Vertex[Batch.initialArraySize], null);
                    layouts.TryGetValue(BatchType.guiCutout, out VertexBufferLayout l);
                    vao.addBufferDynamic(Batch.initialArraySize * Vertex.vertexByteSize, l);
                    vao.drawType = PrimitiveType.Triangles;
                    break;

                case BatchType.guiText:
                    ShaderUtil.tryGetShader(ShaderUtil.text2DName, out theBatch.batchShader);
                    break;

                case BatchType.text3D:
                    ShaderUtil.tryGetShader(ShaderUtil.text3DName, out theBatch.batchShader);
                    break;

                case BatchType.lerpText3D:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpText3DName, out theBatch.batchShader);
                    break;

                case BatchType.triangles:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    break;

                case BatchType.quads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    break;

                case BatchType.lines:
                    ShaderUtil.tryGetShader(ShaderUtil.linesName, out theBatch.batchShader);
                    break;

                case BatchType.iSpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out theBatch.batchShader);
                    break;

                case BatchType.iSpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.iSpheresTransparentName, out theBatch.batchShader);
                    break;

                case BatchType.lerpISpheres:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresName, out theBatch.batchShader);
                    break;

                case BatchType.lerpTriangles:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader);
                    break;

                case BatchType.lerpQuads:
                    ShaderUtil.tryGetShader(ShaderUtil.quadsName, out theBatch.batchShader);
                    break;

                case BatchType.lerpLines:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpLinesName, out theBatch.batchShader);
                    break;

                case BatchType.lerpISpheresTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpISpheresTransparentName, out theBatch.batchShader);
                    break;

                case BatchType.lerpTrianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpTrianglesName, out theBatch.batchShader);
                    break;

                case BatchType.lerpQuadsTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.lerpQuadsTransparentName, out theBatch.batchShader);
                    break;

                case BatchType.trianglesTransparent:
                    ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out theBatch.batchShader);
                    break;

                case BatchType.spriteCylinder:
                    ShaderUtil.tryGetShader(ShaderUtil.spriteCylinderName, out theBatch.batchShader);
                    break;
            }

            vao.finishBuilding();
            theBatch.setVAO(vao);
            theBatch.calculateBatchLimitations();
        }

        //TODO: Complete draw function for each type
        public static void drawBatch(Batch theBatch, Matrix4 viewMatrix, Vector3 fogColor)
        {
            theBatch.bindVAO();
            theBatch.batchShader.use();
            theBatch.batchShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            theBatch.batchShader.setUniformMat4F("viewMatrix", viewMatrix);
            theBatch.batchShader.setUniformVec3F("fogColor", fogColor);
            theBatch.batchShader.setUniform1F("fogStart", GameInstance.get.currentPlanet.getFogStart());
            theBatch.batchShader.setUniform1F("fogEnd", GameInstance.get.currentPlanet.getFogEnd());
            switch (theBatch.getBatchType())
            {
                case BatchType.none:
                    return;

                case BatchType.guiCutout:
                    theBatch.batchShader.setUniformMat4F("orthoMatrix", Renderer.orthoMatrix);
                    GL.DrawArrays(PrimitiveType.Quads, 0, theBatch.requestedVerticesCount);
                    return;

                case BatchType.guiText:
                    GL.DrawArrays(PrimitiveType.Quads, 0, theBatch.requestedVerticesCount);
                    return;

                case BatchType.text3D:
                    GL.MultiDrawArraysIndirect(PrimitiveType.Quads, theBatch.drawCommands, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case BatchType.lerpText3D:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Quads, theBatch.drawCommands, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case BatchType.triangles:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case BatchType.quads:
                    GL.DrawArrays(PrimitiveType.Quads, 0, theBatch.requestedVerticesCount);
                    return;

                case BatchType.lines:
                    GL.DrawElements(PrimitiveType.Lines, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case BatchType.iSpheres:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    return;

                case BatchType.iSpheresTransparent:
                    theBatch.batchShader.setUniformVec3F("cameraPos", Renderer.camPos);
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.pointsItterator);
                    return;

                case BatchType.lerpISpheres:
                    theBatch.batchShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator / 2);
                    return;

                case BatchType.lerpTriangles:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case BatchType.lerpQuads:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Quads, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case BatchType.lerpLines:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Lines,  DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case BatchType.lerpISpheresTransparent:
                    theBatch.batchShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.DrawArrays(PrimitiveType.Points, 0, theBatch.pointsItterator/2);
                    return;

                case BatchType.lerpTrianglesTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, theBatch.requestedObjectItterator, 0);
                    return;

                case BatchType.lerpQuadsTransparent:
                    theBatch.batchShader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());
                    GL.MultiDrawArraysIndirect(PrimitiveType.Quads, IntPtr.Zero, theBatch.requestedObjectItterator, sizeof(uint));
                    return;

                case BatchType.trianglesTransparent:
                    GL.DrawElements(PrimitiveType.Triangles, theBatch.requestedIndicesCount, DrawElementsType.UnsignedInt, theBatch.batchedModel.indices);
                    return;

                case BatchType.spriteCylinder:
                    GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, theBatch.requestedObjectItterator);
                    return;
            }
        }
    }
}
