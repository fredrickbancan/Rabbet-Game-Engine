﻿namespace RabbetGameEngine
{
    public class GUITransparentRectangle : GUIComponent
    {
        public GUITransparentRectangle(float posX, float posY, float sizeX, float sizeY, Color color, ComponentAnchor anchor, int renderLayer = 0, bool dpiRelative = true, string texture = "white") : base(posX, posY, renderLayer)
        {
            base.anchor = anchor;
            setModel(QuadPrefab.copyModel().setColor(color.toNormalVec4()));
            setSize(sizeX, sizeY, dpiRelative);
            componentTexture = TextureUtil.getTexture(texture);
        }

        public override void requestRender(bool isFrameUpdate)
        {
            base.requestRender(isFrameUpdate);
            if (!hidden)
            {
                Renderer.requestRender(RenderType.guiTransparent, componentTexture, componentQuadModel.copyModel().transformVertices(translationAndScale), renderLayer, isFrameUpdate);
            }
        }
    }
}
