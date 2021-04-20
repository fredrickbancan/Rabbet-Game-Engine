using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RabbetGameEngine
{
    public static class IconLoader
    {
        public static void getIcon(string name, out int width, out int height, out byte[] data)
        {
            string dir = ResourceUtil.getIconFileDir(name + ".ico");
            Bitmap bitmap = null;
            BitmapData bmpData = null;
            Icon ico = null;
            try
            {
                ico = new Icon(dir);
                bitmap = ico.ToBitmap();
                width = bitmap.Width;
                height = bitmap.Height;
                bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numBytes = bmpData.Stride * bitmap.Height;
                data = new byte[numBytes];
                IntPtr ptr = bmpData.Scan0;
                Marshal.Copy(ptr, data, 0, numBytes);
                //swapping blue and red bits
                for (int i = 0; i < data.Length / 4; i++)
                {
                    byte s = data[i * 4];
                    data[i * 4] = data[i * 4 + 2];
                    data[i * 4 + 2] = s;
                }
            }
            catch (Exception e)
            {
                Application.warn("Could not load icon from directory: " + dir + "\nException: " + e.Message);
                width = 1;
                height = 1;
                data = new byte[1] { 0 };
            }
            finally
            {
                if (bmpData != null)
                {
                    if (bitmap != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bitmap.Dispose();
                    }
                }
                else if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                if (ico != null)
                {
                    ico.Dispose();
                }
            }


        }
    }
}
