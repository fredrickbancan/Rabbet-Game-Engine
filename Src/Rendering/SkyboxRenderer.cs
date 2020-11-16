using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    public static class SkyboxRenderer
    {
        private static Planet skyboxToDraw = null;
        private static Model skyboxModel = null;
        private static Model shroudModel = null;
        private static Shader skyboxShader = null;
        private static Shader sunShader = null;
        private static Shader horizonShroudShader = null;
        private static VertexArrayObject skyVAO = null;
        private static VertexArrayObject shroudVAO = null;
        /// <summary>
        /// builds the base skybox mesh and prepares for drawing
        /// </summary>
        public static void init()
        {
            Model[] temp = new Model[6];
            temp[0] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 0, 0), new Vector3(0, 0, -0.5F));//negZ
            temp[1] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 180, 0), new Vector3(0, 0, 0.5F));//posZ
            temp[2] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, -90, 0), new Vector3(-0.5F, 0, 0));//negX
            temp[3] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 90, 0), new Vector3(0.5F, 0, 0));//posX
            temp[4] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(-90, 0, 0), new Vector3(0, 0.5F, 0));//top
            temp[5] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(90, 0, 0), new Vector3(0, -0.5F, 0));//bottom
            skyboxModel = new Model(QuadCombiner.combineData(temp), QuadCombiner.getIndicesForQuadCount(6));
            skyVAO = new VertexArrayObject();
            skyVAO.beginBuilding();
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            skyVAO.addBuffer(skyboxModel.vertices, Vertex.vertexByteSize, l);
            skyVAO.addIndicesBuffer(skyboxModel.indices);
            skyVAO.finishBuilding();

            temp = new Model[5];

            temp[0] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 0.5F, 1), new Vector3(0, 0, 0), new Vector3(0, -0.25F, -0.5F));//negZ
            temp[1] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 0.5F, 1), new Vector3(0, 180, 0), new Vector3(0, -0.25F, 0.5F));//posZ
            temp[2] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 0.5F, 1), new Vector3(0, -90, 0), new Vector3(-0.5F, -0.25F, 0));//negX
            temp[3] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 0.5F, 1), new Vector3(0, 90, 0), new Vector3(0.5F, -0.25F, 0));//posX
            temp[4] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(90, 0, 0), new Vector3(0, -0.5F, 0));//bottom

            shroudModel = new Model(QuadCombiner.combineData(temp), QuadCombiner.getIndicesForQuadCount(5));
            shroudVAO = new VertexArrayObject();
            shroudVAO.beginBuilding();
            shroudVAO.addBuffer(shroudModel.vertices, Vertex.vertexByteSize, l);
            shroudVAO.addIndicesBuffer(shroudModel.indices);
            shroudVAO.finishBuilding();

            ShaderUtil.tryGetShader(ShaderUtil.skyboxName, out skyboxShader);
            ShaderUtil.tryGetShader(ShaderUtil.sunName, out sunShader);
            ShaderUtil.tryGetShader(ShaderUtil.skyboxShroudName, out horizonShroudShader);
        }

        public static void setSkyboxToDraw(Planet p)
        {
            skyboxToDraw = p;
        }

        public static void drawSkybox(Matrix4 viewMatrix)
        {
            if(skyboxToDraw == null)
            {
                return;
            }
            //drawing skybox
            skyVAO.bind();
            skyboxShader.use();
            skyboxShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            skyboxShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            skyboxShader.setUniformVec3F("skyTop", skyboxToDraw.getSkyColor());
            skyboxShader.setUniformVec3F("skyHorizon", skyboxToDraw.getHorizonColor());
            skyboxShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());
            skyboxShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());
            GL.DepthRange(0.999999f, 1);
            GL.DrawElements(PrimitiveType.Triangles, skyboxModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;

            //drawing horizon shroud
            shroudVAO.bind();
            horizonShroudShader.use();
            horizonShroudShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            horizonShroudShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            horizonShroudShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());
            GL.DepthRange(0.9999925f, 0.999926f);
            GL.DrawElements(PrimitiveType.Triangles, shroudModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;

            //drawing sun
            sunShader.use();
            sunShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            sunShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            sunShader.setUniformVec3F("sunPos", skyboxToDraw.getSunDirection());
            sunShader.setUniformVec3F("sunColor", skyboxToDraw.getSunColor());
            sunShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
            GL.DepthRange(0.9999945f, 0.999946f);
            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            Renderer.totalDraws++;
            GL.DepthRange(0, 1);
        }
        public static void deleteVAO()
        {
            skyVAO.delete();
        }
    }
}
