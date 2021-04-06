using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    //TODO: add onvideosettingschanged func
    public static class SkyboxRenderer
    {
        private static Texture ditherTex = null;
        private static Texture moonsTex = null;
        private static World skyboxToDraw = null;
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
            skyVAO.addBuffer(skyboxModel.vertices, Vertex.SIZE_BYTES, l);
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
            shroudVAO.addBuffer(shroudModel.vertices, Vertex.SIZE_BYTES, l);
            shroudVAO.addIndicesBuffer(shroudModel.indices);
            shroudVAO.finishBuilding();

            ShaderUtil.tryGetShader(ShaderUtil.skyboxName, out skyboxShader);
            skyboxShader.use();
            skyboxShader.setUniform1I("ditherTex", 1);
            skyboxShader.setUniform1F("minSkyLuminosity", World.minSkyLuminosity);
            skyboxShader.setUniform1F("maxSkyLuminosity", World.maxSkyLuminosity);
            ShaderUtil.tryGetShader(ShaderUtil.sunName, out sunShader);
            sunShader.use();

            ShaderUtil.tryGetShader(ShaderUtil.skyboxShroudName, out horizonShroudShader);
            ShaderUtil.tryGetShader(ShaderUtil.starsName, out starsShader);
            ShaderUtil.tryGetShader(ShaderUtil.moonsName, out moonsShader);
            TextureUtil.tryGetTexture("dither", out ditherTex);
            TextureUtil.tryGetTexture("moons", out moonsTex);
            moonsTex.use();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        }

        public static void setSkyboxToDraw(World p)
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
            starsVAO.addBuffer(p.getStars().points, PointParticle.SIZE_BYTES, sl);
            starsVAO.finishBuilding();

            if (moonsVAO != null)
            {
                moonsVAO.delete();
            }

            moonBuffer = new Sprite3D[p.totalMoons];
            SkyMoon[] m = p.getMoons();
            for (int i = 0; i < p.totalMoons; i++)
            {
                moonBuffer[(p.totalMoons - 1) - i] = m[i].sprite;//reverse order to prevent alpha blending of overlapping moons
            }

            Vector2[] axies = new Vector2[p.totalMoons];

            for (int i = 0; i < p.totalMoons; i++)
            {
                Vector2 dir = m[i].orbitDirection;
                Vector2 axis = new Vector2(dir.Y, dir.X);
                axies[p.totalMoons-1-i] = axis;
            }
            moonsVAO = new VertexArrayObject();
            moonsVAO.beginBuilding();
            VertexBufferLayout ml = new VertexBufferLayout();
            Sprite3D.configureLayout(ml);
            ml.instancedData = true;
            moonsVAO.addBufferDynamic(p.totalMoons * Sprite3D.sizeInBytes, ml);
            moonsVAO.updateBuffer(0, moonBuffer, p.totalMoons * Sprite3D.sizeInBytes);
            VertexBufferLayout al = new VertexBufferLayout();
            al.add(VertexAttribPointerType.Float, 2);
            al.instancedData = true;
            moonsVAO.addBuffer(axies, sizeof(float) * 2, al);
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
            Matrix4 proj = Renderer.projMatrix;
            Matrix4 view = viewMatrix.ClearTranslation();

            //drawing horizon shroud
            shroudVAO.bind();
            horizonShroudShader.use();
            horizonShroudShader.setUniformMat4F("projectionMatrix", proj);
            horizonShroudShader.setUniformMat4F("viewMatrix", view);
            GL.DepthRange(0.99995f, 0.99995f);
            GL.DrawElements(PrimitiveType.Triangles, shroudModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            Renderer.totalDraws++;

            //drawing skybox
            skyVAO.bind();
            skyboxShader.use();
            skyboxShader.setUniformMat4F("projectionMatrix", proj);
            skyboxShader.setUniformMat4F("viewMatrix", view);
            GL.DepthRange(0.999995f, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            ditherTex.use();
            GL.DrawElements(PrimitiveType.Triangles, skyboxModel.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            Renderer.totalDraws++;


            //drawing moons
            moonsVAO.bind();
            moonsTex.use();
            moonsShader.use();
            moonsShader.setUniformMat4F("projectionMatrix", proj);
            moonsShader.setUniformMat4F("viewMatrix", view);
            GL.DepthRange(0.99996f, 0.999995f);
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, skyboxToDraw.totalMoons);
            Renderer.totalDraws++;

            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);//additive blending
            //drawing stars
            starsVAO.bind(); 
            starsShader.use();
            starsShader.setUniformMat4F("projectionMatrix", proj);
            starsShader.setUniformMat4F("viewMatrix", view);
            GL.DepthRange(0.999996f, 0.999996f);
            GL.DrawArrays(PrimitiveType.Points, 0, skyboxToDraw.totalStars);
            Renderer.totalDraws++;

            //TODO: Make sun drawn as a quad not a point. will help to reduce distortion.
            //drawing sun
            moonsVAO.bind();
            sunShader.use();
            sunShader.setUniformMat4F("projectionMatrix", proj);
            sunShader.setUniformMat4F("viewMatrix", view);
            GL.DepthRange(0.999995f, 0.9999951f);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            Renderer.totalDraws++;

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthMask(true);
            GL.DepthRange(0, 1);
        }


        public static void onUpdate()
        {
            if (skyboxToDraw == null) return;

            skyboxShader.use();
            skyboxShader.setUniformVec3F("skyColor", skyboxToDraw.getSkyColor());
            skyboxShader.setUniform1F("skyLuminosity", skyboxToDraw.getSkyLuminosity());//not realy used
            skyboxShader.setUniformVec3F("skyAmbient", skyboxToDraw.getSkyAmbientColor());
            skyboxShader.setUniformVec3F("skyHorizon", skyboxToDraw.getHorizonColor());
            skyboxShader.setUniformVec3F("skyHorizonAmbient", skyboxToDraw.getHorizonAmbientColor());
            skyboxShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());
            skyboxShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());

            horizonShroudShader.use();
            horizonShroudShader.setUniformVec3F("fogColor", skyboxToDraw.getFogColor());

            sunShader.use();
            sunShader.setUniformVec3F("sunPos", skyboxToDraw.getSunDirection());
            sunShader.setUniformVec3F("sunColor", skyboxToDraw.getSunColor());

            starsShader.use();
            starsShader.setUniformMat4F("modelMatrix", MathUtil.dirVectorToRotationNoFlip(skyboxToDraw.getSunDirection()));
            starsShader.setUniformVec2F("viewPortSize", Renderer.viewPortSize);
            starsShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());

            SkyMoon[] m = skyboxToDraw.getMoons();
            for (int i = 0; i < skyboxToDraw.totalMoons; i++)
            {
                moonBuffer[(skyboxToDraw.totalMoons - 1) - i] = m[i].sprite;//reverse order to prevent alpha blending of overlapping moons
            }
            moonsVAO.bind();
            moonsVAO.updateBuffer(0, moonBuffer, skyboxToDraw.totalMoons * Sprite3D.sizeInBytes);

            moonsShader.use();
            moonsShader.setUniformVec3F("sunDir", skyboxToDraw.getSunDirection());

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
