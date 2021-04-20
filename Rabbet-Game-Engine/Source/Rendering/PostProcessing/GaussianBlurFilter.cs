using OpenTK.Graphics.OpenGL;


namespace RabbetGameEngine
{
    //TODO: this can be optimized with more fbos for each layer so they dont have to be resized each frame.
    public class GaussianBlurFilter : IPostFilter
    {
        private const int numLayers = 2;
        private const float resolutionFactor = 0.5F;
        private int resWidth;
        private int resHeight;
        private HdrFrameBuffer[] ppfbos;//Ping pong frame buffers for blurring
        private HdrFrameBuffer[] layerFbos;//FrameBuffers for containing the result of each layer to be applied to result
        private HdrFrameBuffer resultFBO;
        private Shader gBlurShader;
        private Shader passThroughShader;
        private Shader combineAverageShader;

        public void delete()
        {
            for (int i = 0; i < 2; i++)
            {
                ppfbos[i].delete();
                layerFbos[i].delete();
                resultFBO.delete();
            }
        }

        public void init(int initialResWidth, int initialResHeight)
        {
            resWidth = (int)((float)initialResWidth * resolutionFactor);
            resHeight = (int)((float)initialResHeight * resolutionFactor);

            if (numLayers >= RenderConstants.MAX_BATCH_TEXTURES)
            {
                Application.error("Gaussian blur filter can not process " + numLayers + " layers because it exceeds the maximum textures!");
            }

            ppfbos = new HdrFrameBuffer[2];
            layerFbos = new HdrFrameBuffer[numLayers];

            for (int i = 0; i < 2; i++)
            {
                ppfbos[i] = new HdrFrameBuffer(resWidth, resHeight, TextureMinFilter.Linear, TextureMagFilter.Linear, false);
            }

            for (int i = 0; i < numLayers; i++)
            {
                layerFbos[i] = new HdrFrameBuffer(resWidth, resHeight, TextureMinFilter.Linear, TextureMagFilter.Linear, false);
            }

            resultFBO = new HdrFrameBuffer(resWidth, resHeight, TextureMinFilter.Linear, TextureMagFilter.Linear, false);

            ShaderUtil.tryGetShader(ShaderUtil.filterGBlurName, out gBlurShader);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferPassThroughName, out passThroughShader);
            ShaderUtil.tryGetShader(ShaderUtil.frameBufferCombineAverageName, out combineAverageShader);
            combineAverageShader.use();
            combineAverageShader.setUniformIArray("renderedTextures", getUniformTextureSamplerArrayInts(numLayers));
            combineAverageShader.setUniform1I("texCount", numLayers);
        }

        private void bindAllLayerTexturesToSlots()
        {
            for (int i = 0; i < numLayers; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                layerFbos[i].bindOutputTexture();
            }
        }

        private int[] getUniformTextureSamplerArrayInts(int count)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = i;
            }
            return result;
        }

        private void resizeBuf(int index, int sizeWidth, int sizeHeight)
        {
            ppfbos[index].resize(sizeWidth, sizeHeight);
        }

        private int doBlur(int srcTex, int layer, int itterations)
        {

            int layerResWidth = resWidth >> layer * 2;//quater size for each layer for bigger blur and less itterations
            int layerResHeight = resHeight >> layer * 2;

            for (int i = 0; i < 2; i++) ppfbos[i].resize(layerResWidth, layerResHeight);
            bool firstIter = true;
            int evenItterBool = 0;//1 if itteration is even else 0

            gBlurShader.use();
            gBlurShader.setUniform1I("layer", layer);

            for (int i = 0; i < itterations; i++)
            {
                evenItterBool = i & 1;

                ppfbos[evenItterBool].use();//target buffer
                GL.BindTexture(TextureTarget.Texture2D, firstIter ? srcTex : ppfbos[1 - evenItterBool].getOutputTexture());//source texture

                gBlurShader.setUniform1I("verticalPass", evenItterBool);
                FrameBufferQuad.draw();
                firstIter = false;
            }

            //store result in a layer frame buffer
            layerFbos[layer].use();
            ppfbos[evenItterBool].bindOutputTexture();
            passThroughShader.use();
            FrameBufferQuad.draw();

            return layerFbos[layer].getOutputTexture();//return id of result texture of this blurr pass
        }

        private int combineLayers()
        {
            bindAllLayerTexturesToSlots();
            combineAverageShader.use();
            resultFBO.use();
            FrameBufferQuad.draw();
            return resultFBO.getOutputTexture();
        }

        public int processImage(int textureID, int width, int height)
        {
            FrameBufferQuad.bindVao();
            int texID = textureID;

            for (int i = 0; i < numLayers; i++)
            {
                texID = doBlur(texID, i, 6);//make blur layers based on previous layer result
            }

            return combineLayers();
        }

        public int getResultWidth()
        {
            return resWidth;
        }

        public int getResultHeight()
        {
            return resHeight;
        }

        public void onResize(int newResWidth, int newResHeight)
        {

        }
    }
}
