using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public struct Sprite3D
    {
        public static int sizeInBytes = 3 * sizeof(float) + 4 * sizeof(float) + 3 * sizeof(float) + 4 * sizeof(float);
        Vector3 position;
        Vector4 color;
        Vector3 scale;
        Vector4 uvMinMax;

        public Sprite3D(Vector3 pos, Vector4 color, Vector3 scale, Vector4 uvMinMax)
        {
            position = pos;
            this.color = color;
            this.scale = scale;
            this.uvMinMax = uvMinMax;
        }

        public static void configureLayout(VertexBufferLayout l)
        {
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 3);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 4);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 3);
            l.add(OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, 4);
        }
    }
}
