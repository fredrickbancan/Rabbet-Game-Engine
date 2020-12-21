using OpenTK.Mathematics;
using RabbetGameEngine.Models;
using RabbetGameEngine.Text;

namespace RabbetGameEngine
{
    //TODO: Implement working debug frame time chart
    public class GUIDebugFrameTimeChart : GUIComponent
    {
        private Model graphLines;
        private Model measureLines;
        private Model measureNum1;
        private Model measureNum2;
        private Model measureNum3;
        private Model title;
        private FontFace font;
        private GUITransparentRectangle backGround;
        private Vector4 goodFrameTimeColor = Color.green.toNormalVec4();
        private Vector4 moderateFrameTimeColor = Color.yellow.toNormalVec4();
        private Vector4 badFrameTimeColor = Color.red.toNormalVec4();

        public GUIDebugFrameTimeChart(float posX, float posY, ComponentAnchor anchor, int renderLayer = 0) : base(posX, posY, renderLayer)
        {
            this.anchor = anchor;
            backGround = new GUITransparentRectangle(posX, posY, ((float)GUIManager.guiLineWidth * 200.0F + 40.0F) / (float)GameInstance.gameWindowWidth, 200.0F / (float)GameInstance.gameWindowHeight, Color.black, anchor, renderLayer, false);

            TextUtil.tryGetFont("consolas", out font);
            componentTexture = TextureUtil.getTexture("none");
            updateRenderData();

            graphLines = new Model(new Vertex[400], null);
            for (int i = 0; i < 400; i += 2)
            {
                graphLines.vertices[i].pos.X = i / 2 * GUIManager.guiLineWidth;
                graphLines.vertices[i + 1].pos.X = i / 2 * GUIManager.guiLineWidth;
                graphLines.vertices[i + 1].pos.Y = graphLines.vertices[i].pos.Y + (int)(100D * GameInstance.rand.NextDouble());
                graphLines.vertices[i].color = Color.black.toNormalVec4();
                graphLines.vertices[i + 1].color = Color.green.toNormalVec4();
            }

            measureLines = new Model(new Vertex[6], null);
            for (int i = 0; i < 6; i += 2)
            {
                measureLines.vertices[i].color = Color.grey.toNormalVec4();
                measureLines.vertices[i + 1].color = Color.grey.toNormalVec4();
                measureLines.vertices[i].pos.X = -(screenPixelSize.X * 0.5F + 3.0F);
                measureLines.vertices[i + 1].pos.X = screenPixelSize.X * 0.5F + 3.0F;
                measureLines.vertices[i].pos.Y = measureLines.vertices[i + 1].pos.Y = (i / 2 + 1) * 50.0F - screenPixelSize.Y * 0.5F;
            }
        }
    

        public override void updateRenderData()
        {
            setSize(((float)GUIManager.guiLineWidth * 200.0F) / (float)GameInstance.gameWindowWidth, 500.0F / (float)GameInstance.gameWindowHeight, false);
            backGround.setSize(((float)GUIManager.guiLineWidth * 200.0F + 60.0F) / (float)GameInstance.gameWindowWidth, 200.0F / (float)GameInstance.gameWindowHeight, false);
            backGround.updateRenderData();
            base.updateRenderData();
            measureNum1 = TextModelBuilder2D.convertStringToModel("5ms", font, Color.grey.toNormalVec4(), new Vector3(screenPixelPos.X - screenPixelSize.X * 0.5F - 5.0F, screenPixelPos.Y - 200.0F, -0.2F), 0.15F, ComponentAnchor.CENTER_RIGHT);
            measureNum2 = TextModelBuilder2D.convertStringToModel("10ms", font, Color.grey.toNormalVec4(), new Vector3(screenPixelPos.X - screenPixelSize.X * 0.5F - 5.0F, screenPixelPos.Y - 150.0F, -0.2F), 0.15F, ComponentAnchor.CENTER_RIGHT);
            measureNum3 = TextModelBuilder2D.convertStringToModel("15ms", font, Color.grey.toNormalVec4(), new Vector3(screenPixelPos.X - screenPixelSize.X * 0.5F - 5.0F, screenPixelPos.Y - 100.0F, -0.2F), 0.15F, ComponentAnchor.CENTER_RIGHT);
            title = TextModelBuilder2D.convertStringToModel("Frame delta times", font, Color.grey.toNormalVec4(), new Vector3(screenPixelPos.X, screenPixelPos.Y - 75.0F, -0.2F), 0.15F, ComponentAnchor.CENTER);
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
                graphLines.vertices[i + 1].pos = new Vector3(columnBasePos.X, columnBasePos.Y + (float)(frameTimes[frameIndex] * 10.0D), columnBasePos.Z);
                graphLines.vertices[i + 1].color = frameTimes[frameIndex] < 10.0D ? goodFrameTimeColor : frameTimes[frameIndex] < 15.0D ? moderateFrameTimeColor : badFrameTimeColor;
            }
        }

        public override void requestRender()
        {
            if (!hidden)
            {
                backGround.requestRender();
                Renderer.requestRender(RenderType.guiLines, componentTexture, measureLines.copyModel().translateVertices(translationAndScale.ExtractTranslation()), renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, title, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, measureNum1, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, measureNum2, renderLayer);
                Renderer.requestRender(RenderType.guiText, font.texture, measureNum3, renderLayer);
                Renderer.requestRender(RenderType.guiLines, componentTexture, graphLines.copyModel().translateVertices(translationAndScale.ExtractTranslation() - new Vector3(screenPixelSize.X * 0.5F , screenPixelSize.Y * 0.5F, 0)), renderLayer);
            }
        }
    }
    
}
