using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FredrickTechDemo
{
    /*this class provides a handle on any image the game wants to load as a texture.*/
    public class Texture : IDisposable
    {
        private bool disposed = false;
        private int id;
        public Texture(String path, bool enableFiltering)
        {
            if (!enableFiltering)
            {
                id = loadTexture(path, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            }
            else
            {
                id = loadTexture(path, TextureMinFilter.Linear, TextureMagFilter.Linear);
            }
        }

        public Texture()//constructs default texture
        {
            Bitmap bitmap = new Bitmap(16, 16);//creating default error texture
            for(int i = 0; i < 16; i++)
            {
                for(int j = 0; j < 16; j++)
                {//exlusive or
                    bitmap.SetPixel(i, j, i % 2 == 0 ^ j % 2 != 0 ? Color.Magenta : Color.Black);//creating black and magenta checker board
                }
            }

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            id = tex;
        }

        public void use()
        {
            GL.BindTexture(TextureTarget.Texture2D, id);
        }
        

        public int loadTexture(String file, TextureMinFilter minfilter, TextureMagFilter magfilter)
        {   
            if(GL.IsTexture(id))//checks if this texture has already been loaded, if so, will replace it with a new one
            {
                GL.DeleteTexture(id);
            }
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(file);
            }
            catch(Exception e)
            {
                Application.error("Could not load texture: " + file + "\nException: " + e.Message);
                bitmap = new Bitmap(2,2);//creating default error texture
                bitmap.SetPixel(0,0, Color.Black);
                bitmap.SetPixel(0,1, Color.Magenta);
                bitmap.SetPixel(1,0, Color.Magenta);
                bitmap.SetPixel(1,1, Color.Black);
            }
            
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);// flipping vertically because opengl starts from bottom left corner 

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minfilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magfilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteTexture(id);

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
