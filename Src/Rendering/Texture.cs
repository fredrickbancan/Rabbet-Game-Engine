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
        public Texture(string path, bool enableFiltering)
        {
            if (path != "none")
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
            for(int i = 0; i < res; i++)
            {
                for(int j = 0; j < res; j++)
                {//exlusive or
                    bitmap.SetPixel(i, j, i % 2 == 0 ^ j % 2 != 0 ? Color.Magenta : Color.Black);//creating black and magenta checker board
                }
            }

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            id = tex;
        }


        public void use()
        {
            if(!isNone)
                GL.BindTexture(TextureTarget.Texture2D, id);
        }
        

        public int loadTexture(string file, TextureMinFilter minfilter, TextureMagFilter magfilter)
        {   
            if(GL.IsTexture(id))//checks if this texture has already been loaded, if so, will replace it with a new one
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
            catch(Exception e)
            {
                Application.error("Could not load texture: " + file + "\nException: " + e.Message);
                bitmap = new Bitmap(16, 16);//creating default error texture
                width = 16;
                height = 16;
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {//exlusive or
                        bitmap.SetPixel(i, j, i % 2 == 0 ^ j % 2 != 0 ? Color.Magenta : Color.Black);//creating black and magenta checker board
                    }
                }
            }
            
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);// flipping vertically because opengl starts from bottom left corner 

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
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
