﻿using OpenTK.Mathematics;

namespace RabbetGameEngine.VisualEffects
{
    //temporary class
    public class VFXLogoSprite3D : VFX
    {
        public Vector2 spriteSize;
        public Sprite3D sprite;
        public VFXLogoSprite3D(Vector3 pos, Vector2 size) : base(pos, 1.0F, "icon", null, 10000, RenderType.spriteCylinder)
        {
            spriteSize = size;
            sprite = new Sprite3D(pos, CustomColor.white.toNormalVec4(), new Vector3(size.X, size.Y, 1.0F), new Vector4(0, 0, 1, 1));
        }
        public override void sendRenderRequest()
        {
            Renderer.requestRender(sprite, vfxTexture);
        }
    }
}