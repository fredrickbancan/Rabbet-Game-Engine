using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    public static class SkyboxRenderer
    {
        private static Texture ditherTex = null;
        private static Texture moonsTex = null;
        private static Planet skyboxToDraw = null;
        private static Model skyboxModel = null;
        private static Model shroudModel = null;
        private static Shader skyboxShader = null;
        private static Shader sunShader = null;
        private static Shader horizonShroudShader = null;
        private static Shader starsShader = null;
        private static Shader moonsShader = null;
        private static VertexArrayObject skyVAO = null;
        private static VertexArrayObject shroudVAO = null;
        private static VertexArrayObject starsVAO = null;
        private static VertexArrayObject moonsVAO = null;
        private static Sprite3D[] moonBuffer = null;
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
            ShaderUtil.tryGetShader(ShaderUtil.starsName, out starsShader);
            ShaderUtil.tryGetShader(ShaderUtil.moonsName, out moonsShader);
            TextureUtil.tryGetTexture("dither", out ditherTex);
            TextureUtil.tryGetTexture("moons", out moonsTex);
        }

        public static void setSkyboxToDraw(Planet p)
        {
            skyboxToDraw = p;
            if(starsVAO != null)
            {
                starsVAO.delete();
            }
            starsVAO = new VertexArrayObject();
            starsVAO.beginBuilding();
            VertexBufferLayout sl = new VertexBufferLayout();
            PointParticle.configureLayout(sl);
            starsVAO.addBuffer(p.getStars().points, PointParticle.pParticleByteSize, sl);
            starsVAO.finishBuilding();

            if (moonsVAO != null)
            {
                moonsVAO.delete();
            }

            moonBuffer = new Sprite3D[p.totalMoons];
            for(int i = 0; i < p.totalMoons; i++)
            {
                moonBuffer[i] = p.getMoons()[i].sprite;
            }

            moonsVAO = new VertexArrayObject();
            moonsVAO.beginBuilding();
            VertexBufferLayout ml = new VertexBufferLayout();
            Sprite3D.configureLayout(ml);
            ml.instancedData = true;
            moonsVAO.addBufferDynamic(p.totalMoons * Sprite3D.sizeInBytes, ml);
            moonsVAO.updateBuffer(0, moonBuffer, p.totalMoons * Sprite3D.sizeInBytes);
            VertexBufferLayout il = new VertexBufferLayout();
            il.add(VertexAttribPointerType.Float, 2);
            moonsVAO.addInstanceBuffer(QuadPrefab.quadVertexPositions2D, sizeof(float)*2, il);
            moonsVAO.finishBuilding();
        }

        public static void drawSkybox(Matrix4 viewMatrix)
        {
            if(skyboxToDraw == null)
            {
                return;
            }

            //drawing skybox
            skyVAO.bind();
            ditherTex.use();
            skyboxShader.use();
            skyboxShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            skyboxShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            GL.DepthRange(0.999999f, 1);
            GL.DrawElements(PrimitiveType.Triangles, skyboxModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;

            //drawing moons
            if (moonsVAO != null)
            {
                moonsVAO.bind();
                moonsTex.use();
                moonsShader.use();
                moonsShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                moonsShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
                GL.DepthRange(0.9999900f, 0.999901f);
                GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, skyboxToDraw.totalMoons);
                Renderer.totalDraws++;
            }

            //drawing horizon shroud
            shroudVAO.bind();
            horizonShroudShader.use();
            horizonShroudShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            horizonShroudShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            GL.DepthRange(0.9999800f, 0.999801f);
            GL.DrawElements(PrimitiveType.Triangles, shroudModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;

            //drawing stars
            if (starsVAO != null)
            {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha);//additive blending
                starsVAO.bind();
                starsShader.use();
                starsShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
                starsShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
                GL.DepthRange(0.9999960f, 0.999961f);
                GL.DrawArrays(PrimitiveType.Points, 0, skyboxToDraw.totalStars);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Renderer.totalDraws++;
            }

            //drawing sun
            sunShader.use();
            ditherTex.use();
            sunShader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            sunShader.setUniformMat4F("viewMatrix", viewMatrix.ClearTranslation());
            GL.DepthRange(0.9999940f, 0.999941f);
            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            Renderer.totalDraws++;

            
                //TODO: add glow effect to moons
                //TODO: Fix stars and moon glow overlap problem
                //TODO: Look into proper billboard quads for moons so they can go upside down and dont follow camera.(will require instancing)



            GL.DepthRange(0, 1);
        }


        public static void onTick()
        {
            skyboxShader.use();
            skyboxShader.setUniformVec3F("skyTop", skyboxToDraw.getSkyColor());
            skyboxShader.setUniformVec3F("skyAmbient", skyboxToDraw.getSkyAmbientColor());
            skyboxShader.setUniformVec3F("skyHorizon", skyboxToDraw.getHorizonColor());
            skyboxShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());
            skyboxShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());

            horizonShroudShader.use();
            horizonShroudShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());

            sunShader.use();
            sunShader.setUniformVec3F("sunPos", skyboxToDraw.getSunDirection());
            sunShader.setUniformVec3F("sunColor", skyboxToDraw.getSunColor());
            sunShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));

            if (starsVAO != null)
            {
                starsShader.use();
                starsShader.setUniformMat4F("modelMatrix", MathUtil.dirVectorToRotationNoFlip(skyboxToDraw.getSunDirection()));
                starsShader.setUniformVec2F("viewPortSize", new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight));
                starsShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());
            }

            if (moonsVAO != null)
            {
                SkyMoon[] m = skyboxToDraw.getMoons();
                for (int i = 0; i < skyboxToDraw.totalMoons; i++)
                {
                    moonBuffer[i] = m[i].sprite;
                }
                moonsVAO.bind();
                moonsVAO.updateBuffer(0, moonBuffer, skyboxToDraw.totalMoons * Sprite3D.sizeInBytes);
                moonsShader.use();
                moonsShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());
            }
        }

        public static void deleteVAO()
        {
            skyVAO.delete();
            shroudVAO.delete();
            if(starsVAO != null)
            {
                starsVAO.delete();
            }

            if(moonsVAO != null)
            {
                moonsVAO.delete();
            }
        }
    }
}
