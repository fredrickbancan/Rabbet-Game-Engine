using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RabbetGameEngine
{
    public static class ScreenShotter
    {
        public static void takeScreenshot()
        {
            try
            {
                int width = GameInstance.gameWindowWidth;
                int height = GameInstance.gameWindowHeight;
                int strideFraction = (width * 3) % 4;
                if (strideFraction > 0) strideFraction = 4 - strideFraction;
                byte[] RGBData = new byte[height * (width * 3 + strideFraction)];
                GL.ReadPixels(0, 0, width, height, PixelFormat.Bgr, PixelType.UnsignedByte, RGBData);
                savePNG(ResourceUtil.getScreenShotFileDir(Application.getAppTimeStamp() + ".png"), width, height, RGBData);
            }
            catch (Exception e)
            {
                Application.error("Failed to take screenshot! Error: " + e.Message);
            }
        }

        public static void savePNG(string fileName, int width, int height, byte[] rgbData)
        {
            IntPtr intPtr = Marshal.AllocHGlobal(rgbData.Length);
            Marshal.Copy(rgbData, 0, intPtr, rgbData.Length);

            System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            int strideFraction = (3 * width) % 4;
            if (strideFraction > 0) strideFraction = 4 - strideFraction;
            Bitmap bmp = new Bitmap(width, height, 3 * width + strideFraction, pixelFormat, intPtr);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
        }

    }
}
