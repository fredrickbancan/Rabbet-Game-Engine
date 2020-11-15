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
        private static Shader skyboxShader = null;
        private static VertexArrayObject VAO = null;
        /// <summary>
        /// builds the base skybox mesh and prepares for drawing
        /// </summary>
        public static void init()
        {
            Model[] temp = new Model[6];
            temp[0] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 180, 0), new Vector3(0, 0, 0.5F));//posZ
            temp[1] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, -90, 0), new Vector3(-0.5F, 0, 0));//negX
            temp[2] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 90, 0), new Vector3(0.5F, 0, 0));//posX
            temp[3] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(0, 0, 0), new Vector3(0, 0, -0.5F));//negZ
            temp[4] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(-90, 0, 0), new Vector3(0, 0.5F, 0));//top
            temp[5] = QuadPrefab.copyModel().transformVertices(new Vector3(1, 1, 1), new Vector3(90, 0, 0), new Vector3(0, -0.5F, 0));//bottom
            skyboxModel = new Model(QuadCombiner.combineData(temp), QuadCombiner.getIndicesForQuadCount(6));
            VAO = new VertexArrayObject();
            VAO.beginBuilding();
            VertexBufferLayout l = new VertexBufferLayout();
            Vertex.configureLayout(l);
            VAO.addBuffer(skyboxModel.vertices, Vertex.vertexByteSize, l);
            VAO.addIndicesBuffer(skyboxModel.indices);
            VAO.finishBuilding();

            ShaderUtil.tryGetShader(ShaderUtil.skyboxName, out skyboxShader);
        }

        public static void setSkyboxToDraw(Planet p)
        {
            skyboxToDraw = p;
        }

        public static void drawSkybox(Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if(skyboxToDraw == null)
            {
                return;
            }
            VAO.bind();
            skyboxShader.use();
            skyboxShader.setUniformMat4F("projectionMatrix", projectionMatrix);
            skyboxShader.setUniformMat4F("viewMatrix", viewMatrix);
            skyboxShader.setUniformVec3F("skyTop", skyboxToDraw.getSkyColor());
            skyboxShader.setUniformVec3F("skyHorizon", skyboxToDraw.getHorizonColor());
            skyboxShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());
            skyboxShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());
            GL.DrawElements(PrimitiveType.Triangles, skyboxModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;
        }

        public static void deleteVAO()
        {
            VAO.delete();
        }
    }
}
