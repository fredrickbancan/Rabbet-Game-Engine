using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RabbetGameEngine
{
    /*this class provides a handle on any image the game wants to load as a texture.*/
    public class Texture : IDisposable
    {
        private bool isNone = false;//is true if texture is null
        private bool disposed = false;
        private int id, width, height;
        public Texture(string path, bool filterMin = false, bool filterMag = false, bool trilinear = false)
        {
            if (path == "dither")
            {
                id = loadDitheringTexture();
                Application.checkGLErrors();
            }
            else if (path == "white")
            {
                id = loadWhiteTexture();
                Application.checkGLErrors();
            }
            else if (path != "none")
            {
                TextureMinFilter minfilt = filterMin ? TextureMinFilter.Linear : TextureMinFilter.Nearest;
                TextureMagFilter magfilt = filterMag ? TextureMagFilter.Linear : TextureMagFilter.Nearest;

                id = loadTexture(path, minfilt, magfilt, trilinear);
                Application.checkGLErrors();
            }
            else
            {
                isNone = true;
            }
        }

        public Texture(int res = 16)//constructs default texture
        {
            Bitmap bitmap = new Bitmap(res, res);//creating default error texture
            width = res;
            height = res;
            for (int i = 0; i < res; i++)
            {
                for (int j = 0; j < res; j++)
                {//exlusive or
                    bitmap.SetPixel(i, j, i % 2 == 0 ^ j % 2 != 0 ? System.Drawing.Color.Magenta : System.Drawing.Color.Black);//creating black and magenta checker board
                }
            }

            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            id = tex;
        }


        public void bind(int index = 0)
        {
            if (isNone) return;
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, id);
        }

        private int loadWhiteTexture()
        {
            Bitmap bitmap = new Bitmap(4, 4);//creating default error texture
            width = 4;
            height = 4;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {//exlusive or
                    bitmap.SetPixel(i, j, System.Drawing.Color.White);//creating black and magenta checker board
                }
            }

            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            return tex;
        }

        private int loadDitheringTexture()
        {
            byte[] pattern = new byte[]{
              0, 32,  8, 40,  2, 34, 10, 42,   /* 8x8 Bayer ordered dithering  */
             48, 16, 56, 24, 50, 18, 58, 26,  /* pattern.  Each input pixel   */
             12, 44,  4, 36, 14, 46,  6, 38,  /* is scaled to the 0..63 range */
             60, 28, 52, 20, 62, 30, 54, 22,  /* before looking in this table */
              3, 35, 11, 43,  1, 33,  9, 41,   /* to determine the action.     */
             51, 19, 59, 27, 49, 17, 57, 25,
              15, 47,  7, 39, 13, 45,  5, 37,
             63, 31, 55, 23, 61, 29, 53, 21 };
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, 8, 8, 0, OpenTK.Graphics.OpenGL.PixelFormat.Red, PixelType.UnsignedByte, pattern);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            return id;
        }

        public int loadTexture(string file, TextureMinFilter minfilter, TextureMagFilter magfilter, bool trilinear = false)
        {
            if (GL.IsTexture(id))//checks if this texture has already been loaded, if so, will replace it with a new one
            {
                GL.DeleteTexture(id);
            }
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(file);
                width = bitmap.Width;
                height = bitmap.Height;
            }
            catch (Exception e)
            {
                Application.error("Could not load texture: " + file + "\nException: " + e.Message);
                bitmap = new Bitmap(16, 16);//creating default error texture
                width = 16;
                height = 16;
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {//exlusive or
                        bitmap.SetPixel(i, j, i % 2 == 0 ^ j % 2 != 0 ? System.Drawing.Color.Magenta : System.Drawing.Color.Black);//creating black and magenta checker board
                    }
                }
            }

            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);// flipping vertically because opengl starts from bottom left corner 

            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (trilinear)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minfilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magfilter);
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            bitmap.Dispose();
            return tex;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (!isNone)
                    GL.DeleteTexture(id);

                disposed = true;
            }
        }
        public bool getIsNone()
        {
            return isNone;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool equals(Texture other)
        {
            return this.id == other.getID();
        }

        public int getID()
        {
            return this.id;
        }
        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }
    }
}
