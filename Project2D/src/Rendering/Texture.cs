using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace FredrickTechDemo
{
    class Texture
    {
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

        public void use()
        {
            GL.BindTexture(TextureTarget.Texture2D, id);
        }
        

        public int loadTexture(String file, TextureMinFilter minfilter, TextureMagFilter magfilter)
        {
            Bitmap bitmap = new Bitmap(file);
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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return tex;
        }
    }
}
