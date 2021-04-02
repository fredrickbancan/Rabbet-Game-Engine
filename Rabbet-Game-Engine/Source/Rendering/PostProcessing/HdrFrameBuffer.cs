using OpenTK.Graphics.OpenGL;

namespace RabbetGameEngine
{
    /// <summary>
    /// Object for encapsulating a HDR framebuffer for post processing purposes.
    /// Does not contain textures or shaders or render code.
    /// </summary>
    public class HdrFrameBuffer
    {
        protected int fboID;
        protected int[] texOutputs;
        protected int numTexOutputs;
        protected int fboDepthID;
        protected int width;
        protected int height;
        protected bool hasDepth;
        protected TextureMinFilter minFilter;
        protected TextureMagFilter magFilter;

        /// <summary>
        /// Construct HDR framebuffer (uses float16 for data). size can be changed after.
        /// </summary>
        /// <param name="initialWidth">Inital width of buffer</param>
        /// <param name="initialHeight">Initial height of buffer</param>
        /// <param name="minificationFilger">Filter for minification of this framebuffers textures</param>
        /// <param name="magnificationFilter">Filter for magnification of this framebuffers textures</param>
        /// <param name="depth">Sets wether or not this framebuffer has a depth buffer.</param>
        /// <param name="numOutputs">Number of texture outputs of this framebuffer</param>
        public HdrFrameBuffer(int initialWidth, int initialHeight, TextureMinFilter minificationFilger, TextureMagFilter magnificationFilter, bool depth = true, int numOutputs = 1)
        {
            minFilter = minificationFilger;
            magFilter = magnificationFilter;
            hasDepth = depth;
            width = initialWidth;
            height = initialHeight;
            numTexOutputs = numOutputs;
            initBuffers();
        }

        protected virtual void initBuffers()
        {
            fboID = GL.GenFramebuffer();
            if(hasDepth) fboDepthID = GL.GenRenderbuffer();
            texOutputs = new int[numTexOutputs];
            GL.GenTextures(numTexOutputs, texOutputs);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);
            DrawBuffersEnum[] attachments = new DrawBuffersEnum[numTexOutputs];
            for (int i = 0; i < numTexOutputs; i++) attachments[i] = DrawBuffersEnum.ColorAttachment0 + i;
            GL.DrawBuffers(numTexOutputs, attachments);

            for (int i = 0; i < numTexOutputs; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, texOutputs[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, System.IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, texOutputs[i], 0);
            }
            if (!hasDepth) return;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, fboDepthID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, fboDepthID);
        }

        public virtual void resize(int width, int height)
        {
            this.width = width;
            this.height = height;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);

            for (int i = 0; i < numTexOutputs; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, texOutputs[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, System.IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, texOutputs[i], 0);
            }
            if (!hasDepth) return;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, fboDepthID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, fboDepthID);
        }

        public virtual void use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);
            GL.Viewport(0, 0, width, height);
        }

        public virtual int getOutputTexture(int index = 0)
        {
            return texOutputs[index];
        }

        public virtual void delete()
        {
            GL.DeleteFramebuffer(fboID);
            GL.DeleteTextures(numTexOutputs, texOutputs);
            if (hasDepth) GL.DeleteRenderbuffer(fboDepthID);
        }
    }
}
