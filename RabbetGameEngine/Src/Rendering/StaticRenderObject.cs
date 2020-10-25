using OpenTK;
using OpenTK.Graphics.OpenGL;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;

namespace RabbetGameEngine
{
    public class StaticRenderObject
    {
        private VertexArrayObject VAO = null;
        private Texture tex = null;
        private Shader shader = null;
        private Model mod = null;
        private PointCloudModel pointMod = null;
        private PrimitiveType type;
        private bool pointBased = false;

        private StaticRenderObject(string texture, string shader, Model data, PrimitiveType drawType)
        {
            TextureUtil.tryGetTexture(texture, out tex);
            ShaderUtil.tryGetShader(shader, out this.shader);
            this.mod = data;
            type = drawType;
            
            switch(type)
            {
                case PrimitiveType.Triangles:
                    VAO = VertexArrayObject.createStaticTriangles(data.vertices, data.indices);
                    break;
                case PrimitiveType.Lines:
                    VAO = VertexArrayObject.createStaticLines(data.vertices, data.indices);
                    break;
            }
        }
        private StaticRenderObject(string shader, PointCloudModel data)
        {
            ShaderUtil.tryGetShader(shader, out this.shader);
            this.pointMod = data;
            type = PrimitiveType.Points;
            pointBased = true;
            VAO = VertexArrayObject.createStaticPoints(pointMod.points);
        }

        public static StaticRenderObject createSROTriangles(string texture, string shader, Model data)
        {
            return new StaticRenderObject(texture, shader, data, PrimitiveType.Triangles);
        }

        public static StaticRenderObject createSROLines(string texture, string shader, Model data)
        {
            return new StaticRenderObject(texture, shader, data, PrimitiveType.Lines);
        }

        public static StaticRenderObject createSROPoints(string shader, PointCloudModel data)
        {
            return new StaticRenderObject(shader, data);
        }

        public void draw(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 fogColor)
        {
            VAO.bindVaoVboIbo();
            shader.use();
            if (tex != null)
            {
                tex.use();
            }
            shader.setUniformMat4F("projectionMatrix", projectionMatrix);
            shader.setUniformMat4F("viewMatrix", viewMatrix);
            shader.setUniformVec3F("fogColor", fogColor);
            shader.setUniform1F("percentageToNextTick", TicksAndFps.getPercentageToNextTick());
            shader.setUniform1I("frame", Renderer.frame);
            shader.setUniformVec2F("viewPortSize", Renderer.useOffScreenBuffer ? new Vector2(OffScreen.getWidth, OffScreen.getHeight) : new Vector2(GameInstance.get.Width, GameInstance.get.Height));

            if(pointBased)
            {
                GL.DrawArrays(PrimitiveType.Points, 0, pointMod.points.Length);
                return;
            }
            else
            {
                GL.DrawElements(type, mod.indices.Length, DrawElementsType.UnsignedInt, 0);
                return;
            }
        }

        public void delete()
        {
            VAO.delete();
        }
    }
}
