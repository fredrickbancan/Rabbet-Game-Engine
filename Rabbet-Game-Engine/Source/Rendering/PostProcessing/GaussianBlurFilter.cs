using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    public class GaussianBlurFilter : IPostFilter
    {
        private HdrFrameBuffer[] ppfbos;//Ping pong frame buffers for blurring
        private Shader gBlurShader;

        public void delete()
        {
            for (int i = 0; i < 2; i++)
            {
                ppfbos[i].delete();
            }
        }

        public void init()
        {
            ppfbos = new HdrFrameBuffer[2];
            for (int i = 0; i < 2; i++)
            {
                ppfbos[i] = new HdrFrameBuffer(1024, 1024, TextureMinFilter.Linear, TextureMagFilter.Linear, false);
            }
            ShaderUtil.tryGetShader(ShaderUtil.filterGBlurName, out gBlurShader);
        }

        private void resizeBuf(int index, int size)
        {
            ppfbos[index].resize(size, size);
        }

        private void doBlur(int layer)
        {
            //1024px * 16it * 5krn , 256px * 8it * 11krn, 64px * 4it * 21krn

            int size = 1024 >> (layer * 2);//decrease size for each layer for bigger kernel and less itterations

            GL.Viewport(0, 0, size, size);
            bool highLayer = System.Convert.ToBoolean(layer);//if the layer is more than 0 true

            bool firstIter = true;
            int evenItterBool = 0;//1 if itteration is even else 0
            int itterations = 16 >> layer;
            gBlurShader.use();
            gBlurShader.setUniform1I("layer", 0);

            for (int i = 0; i < itterations; i++)
            {
                evenItterBool = i & 1;
                gBlurShader.setUniform1I("verticalPass", evenItterBool);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, blurPPFBOs[evenItterBool]);//target framebuffer
                GL.BindTexture(TextureTarget.Texture2D, firstIter ? srcTex : blurPPColorBuffers[1 - evenItterBool]);//source texture

                if (highLayer && i < 2)//If not layer 0, resize the target buffer. Do only twice so both buffers get resized.
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, size, size, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, blurPPColorBuffers[evenItterBool], 0);
                }

                GL.DrawElements(PrimitiveType.Triangles, screenQuad.indices.Length, DrawElementsType.UnsignedInt, 0);
                Renderer.totalDraws++;
                firstIter = false;
            }
        }
        public int processImage(int textureID)
        {
            FullScreenQuad.bindVao();
            for(int i = 0; i < 3; i++)
            {
                doBlur(i);
            }

            return textureID;
        }
    }
}
