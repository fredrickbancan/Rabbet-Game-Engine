using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class VFXLogoSprite3D : VFX
    {
        public Vector2 spriteSize;
        public Sprite3D sprite;
        public VFXLogoSprite3D(World w, Vector3 pos, Vector2 size) : base(w, pos, 0, RenderType.spriteCylinder)
        {
            spriteSize = size;
            sprite = new Sprite3D(pos, Color.white.toNormalVec4(), new Vector3(size.X, size.Y, 1.0F), new Vector4(0, 0, 1, 1));
        //    tickable = false;
          //  movable = false;
        }
        public override void sendRenderRequest()
        {
         //   Renderer.requestRender(sprite, vfxTexture);
        }
    }
}
