using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    public class StaticRenderObject
    {
        private VertexArrayObject VAO = null;
        private Texture tex = null;
        private Shader shader = null;
        private int renderLength = 0;
        private RenderType type;

        private StaticRenderObject(string texture, Model data, RenderType drawType)
        {
            TextureUtil.tryGetTexture(texture, out tex);
            type = drawType;

            switch(type)
            {
                case RenderType.triangles:
                    {
                        ShaderUtil.tryGetShader(ShaderUtil.trianglesName, out shader);
                        VAO = new VertexArrayObject();
                        VAO.beginBuilding();
                        VertexBufferLayout l = new VertexBufferLayout();
                        Vertex.configureLayout(l);
                        VAO.addBuffer(data.vertices, Vertex.SIZE_BYTES, l);
                        VAO.addIndicesBuffer(data.indices);
                        VAO.finishBuilding();
                    }
                    break;
                case RenderType.lines:
                    {
                        ShaderUtil.tryGetShader(ShaderUtil.linesName, out shader);
                        VAO = new VertexArrayObject();
                        VAO.beginBuilding();
                        VertexBufferLayout l = new VertexBufferLayout();
                        Vertex.configureLayout(l);
                        VAO.addBuffer(data.vertices, Vertex.SIZE_BYTES, l);
                        VAO.addIndicesBuffer(data.indices);
                        VAO.finishBuilding();
                    }
                    break;
            }

            if(data.indices != null)
            {
                renderLength = data.indices.Length;
            }
            else
            {
                renderLength = data.vertices.Length;
            }
        }
        private StaticRenderObject(PointParticle[] data, bool transparency)
        {
            if (transparency)
            {
                type = RenderType.iSpheresTransparent;
                ShaderUtil.tryGetShader(ShaderUtil.iSpheresTransparentName, out this.shader);
                VAO = new VertexArrayObject();
                VAO.beginBuilding();
                VertexBufferLayout l = new VertexBufferLayout();
                PointParticle.configureLayout(l);
                VAO.addBuffer(data, PointParticle.SIZE_BYTES, l);
                VAO.finishBuilding();
            }
            else
            {
                type = RenderType.iSpheres;
                ShaderUtil.tryGetShader(ShaderUtil.iSpheresName, out this.shader);
                VAO = new VertexArrayObject();
                VAO.beginBuilding();
                VertexBufferLayout l = new VertexBufferLayout();
                PointParticle.configureLayout(l);
                VAO.addBuffer(data, PointParticle.SIZE_BYTES, l);
                VAO.finishBuilding();
            }
            this.renderLength = data.Length;
        }

        public static StaticRenderObject createSROTriangles(string texture, Model data)
        {
            return new StaticRenderObject(texture, data, RenderType.triangles);
        }

        public static StaticRenderObject createSROLines(Model data)
        {
            return new StaticRenderObject("none",  data, RenderType.lines);
        }

        public static StaticRenderObject createSROPoints(PointParticle[] data, bool transparency)
        {
            return new StaticRenderObject(data, transparency);
        }

        public void draw(Matrix4 viewMatrix, Vector3 fogColor)
        {
            VAO.bind();
            shader.use();
            if (tex != null)
            {
                tex.use();
            }
            shader.setUniformMat4F("projectionMatrix", Renderer.projMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniform1F("fogStart", GameInstance.get.currentPlanet.getFogStart());
            shader.setUniform1F("fogEnd", GameInstance.get.currentPlanet.getFogEnd());
            shader.setUniform1F("percentageToNextTick", TicksAndFrames.getPercentageToNextTick());

            switch(type)
            {
                case RenderType.none:
                    return;
                case RenderType.triangles:
                    GL.DrawElements(PrimitiveType.Triangles, renderLength, DrawElementsType.UnsignedInt, 0);
                    return;
                case RenderType.lines:
                    return;

            }
        }

        public void delete()
        {
            VAO.delete();
        }
    }
}
