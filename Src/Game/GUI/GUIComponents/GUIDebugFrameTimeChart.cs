using OpenTK.Mathematics;
using RabbetGameEngine.Models;

namespace RabbetGameEngine
{
    //TODO: Implement working debug frame time chart
    public class GUIDebugFrameTimeChart : GUIComponent
    {
        private Model graphLines;
        private GUITransparentRectangle backGround;
        private Vector4 goodFrameTimeColor = Color.green.toNormalVec4();
        private Vector4 moderateFrameTimeColor = Color.yellow.toNormalVec4();
        private Vector4 badFrameTimeColor = Color.red.toNormalVec4();

        public GUIDebugFrameTimeChart(float posX, float posY, ComponentAnchor anchor, int renderLayer = 0) : base(posX, posY, renderLayer)
        {
            this.anchor = anchor;
            backGround = new GUITransparentRectangle(posX, posY, ((float)GUIManager.guiLineWidth * 200.0F) / (float)GameInstance.gameWindowWidth,250.0F / (float)GameInstance.gameWindowHeight, Color.black, anchor, renderLayer, false);
            graphLines = new Model(new Vertex[400], null);
            for(int i = 0; i < 400; i += 2)
            {
                graphLines.vertices[i].pos.X = i * (int)((float)GUIManager.guiLineWidth * 0.5F);
                graphLines.vertices[i + 1].pos.X = i * (int)((float)GUIManager.guiLineWidth * 0.5F);
                graphLines.vertices[i + 1].pos.Y = graphLines.vertices[i].pos.Y + (int)(100D * GameInstance.rand.NextDouble());
                graphLines.vertices[i].color = Color.black.toNormalVec4();
                graphLines.vertices[i + 1].color = Color.green.toNormalVec4();
            }

            componentTexture = TextureUtil.getTexture("none");
            updateRenderData();
        }

        public override void updateRenderData()
        {
            setSize(((float)GUIManager.guiLineWidth * 200.0F) / (float)GameInstance.gameWindowWidth, 500.0F / (float)GameInstance.gameWindowHeight, false);
            backGround.setSize(((float)GUIManager.guiLineWidth * 200.0F) / (float)GameInstance.gameWindowWidth, 250.0F / (float)GameInstance.gameWindowHeight, false);
            backGround.updateRenderData();
            base.updateRenderData();
        }
    

        public override void onFrame()
        {
            for(int j = 0; j < 400; j += 2)
            {
                graphLines.vertices[j + 1].color *= 0.99F;
                graphLines.vertices[j + 1].color.W = 1;
            }
            double[] frameTimes = TicksAndFrames.getFrameTimes();
            int frameIndex = TicksAndFrames.getFrameIndex();
            int i = TicksAndFrames.getFrameIndex() * 2;
            if (i < 400)
            {
                Vector3 columnBasePos = graphLines.vertices[i].pos;
                graphLines.vertices[i + 1].pos = new Vector3(columnBasePos.X, columnBasePos.Y + (float)(frameTimes[frameIndex] * 5.0D), columnBasePos.Z);
                graphLines.vertices[i + 1].color = frameTimes[frameIndex] < 5.0D ? goodFrameTimeColor : frameTimes[frameIndex] < 10.0D ? moderateFrameTimeColor : badFrameTimeColor;
            }
        }

        public override void requestRender()
        {
            if (!hidden)
            {
                backGround.requestRender();
                Renderer.requestRender(RenderType.guiLines, componentTexture, graphLines.copyModel().translateVertices(translationAndScale.ExtractTranslation() - new Vector3(screenPixelSize.X * 0.5F - (int)((float)GUIManager.guiLineWidth * 0.5F), screenPixelSize.Y * 0.5F, 0)), renderLayer);
            }
        }
    }
    
}
