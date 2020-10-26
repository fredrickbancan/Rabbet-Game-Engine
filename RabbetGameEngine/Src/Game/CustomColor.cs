using OpenTK;
using System.Drawing;

namespace RabbetGameEngine
{
    /*Simple color wrapper struct for the default System.Drawing color struct. Allows custom functionality.*/
    public struct CustomColor
    {
        /*some custom presets*/
        public static CustomColor lightBlue = new CustomColor(135, 135, 255);
        public static CustomColor blue = new CustomColor(0, 0, 255);
        public static CustomColor darkBlue = new CustomColor(0, 0, 100);
        public static CustomColor lightRed = new CustomColor(255, 135, 135);
        public static CustomColor red = new CustomColor(255, 0, 0);
        public static CustomColor darkRed = new CustomColor(100, 0, 0);
        public static CustomColor lightGreen = new CustomColor(135, 255, 135);
        public static CustomColor green = new CustomColor(0, 255, 0);
        public static CustomColor darkGreen = new CustomColor(0, 100, 0);
        public static CustomColor white = new CustomColor(255, 255, 255);
        public static CustomColor black = new CustomColor(0, 0, 0);
        public static CustomColor lightGrey = new CustomColor(175, 175, 175);
        public static CustomColor grey = new CustomColor(125, 125, 125);
        public static CustomColor darkGrey = new CustomColor(72, 72, 72);
        public static CustomColor lightYellow = new CustomColor(255, 255, 135);
        public static CustomColor yellow = new CustomColor(255, 255, 0);
        public static CustomColor darkYellow = new CustomColor(153, 153, 0);
        public static CustomColor lightOrange = new CustomColor(255, 175, 85);
        public static CustomColor orange = new CustomColor(255, 135, 0);
        public static CustomColor darkOrange = new CustomColor(175, 110, 0);
        public static CustomColor ember = new CustomColor(255, 100, 0);
        public static CustomColor flame = new CustomColor(255, 175, 0);
        public static CustomColor facility = new CustomColor(70, 120, 90);
        public static CustomColor steelBlue = new CustomColor(70, 100, 120);
        public static CustomColor lightBlossom = new CustomColor(255, 222, 236);
        public static CustomColor blossom = new CustomColor(255, 150, 190);
        public static CustomColor darkBlossom = new CustomColor(255, 72, 135);
        public static CustomColor lightSkyBlue = new CustomColor(165, 192, 235);
        public static CustomColor skyBlue = new CustomColor(0, 156, 252);
        public static CustomColor aquaPain = new CustomColor(0, 255, 255);
        public static CustomColor hotPink = new CustomColor(255, 105, 180);
        public static CustomColor magenta = new CustomColor(255, 0, 255);

        Color systemColor;//color is stored using system color struct

        public CustomColor(Color setColor)
        {
            systemColor = setColor;
        }
        public CustomColor(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            systemColor = Color.FromArgb(a,r,g,b);
        }

        public CustomColor setColor(Color newColor)
        {
            systemColor = newColor;
            return this;
        }

        /*returns the rgb(a) values of this color as a vector*/
        public Vector3 toVec3()
        {
            return new Vector3(systemColor.R, systemColor.G, systemColor.B);
        }

        public Vector4 toVec4()
        {
            return new Vector4(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

        /*returns the rgb(a) values of this color normalized from 0 to 1 instead of 0 to 255, as a vector*/
        public Vector3 toNormalVec3()
        {
            return new Vector3(MathUtil.normalize(0, 255, systemColor.R), MathUtil.normalize(0, 255, systemColor.G), MathUtil.normalize(0, 255, systemColor.B));
        }

        public Vector4 toNormalVec4()
        {
            return new Vector4(MathUtil.normalize(0, 255, systemColor.R), MathUtil.normalize(0, 255, systemColor.G), MathUtil.normalize(0, 255, systemColor.B), MathUtil.normalize(0, 255, systemColor.A));
        }

        /*Returns a vector 4 with the values of the color normalized from 0 to 1, format: RGBA*/
        public static Vector4 colorToNormalVec4(Color color)
        {
            return new Vector4(MathUtil.normalize(0, 255, color.R), MathUtil.normalize(0, 255, color.G), MathUtil.normalize(0, 255, color.B), MathUtil.normalize(0, 255, color.A));
        }

        /*Returns a vector 3 with the values of the color normalized from 0 to 1, format: RGB*/
        public static Vector3 colorToNormalVec3(Color color)
        {
            return new Vector3(MathUtil.normalize(0, 255, color.R), MathUtil.normalize(0, 255, color.G), MathUtil.normalize(0, 255, color.B));
        }

        /*returns the provided color with its saturation decreased by the provided percentage (0.0 to 1.0)*/
        public CustomColor reduceSaturation(float percentage)
        {
            if (percentage <= 0)
                return this;
            if (percentage > 1F)
                percentage = 1F;

            float newRed = (float)systemColor.R;
            float newGreen = (float)systemColor.G;
            float newBlue = (float)systemColor.B;

            MathUtil.smooth3(ref newRed, ref newGreen, ref newBlue, percentage);
            return new CustomColor((byte)newRed, (byte)newGreen, (byte)newBlue, systemColor.A);
        }

        public Vector3 reduceSaturationVec3(float percentage)
        {
            if (percentage <= 0)
                return this.toVec3();
            if (percentage > 1F)
                percentage = 1F;

            Vector3 result = this.toVec3();

            MathUtil.smooth3(ref result.X, ref result.Y, ref result.Z, percentage);

            return result;
        }

        public Vector4 reduceSaturationVec4(float percentage)
        {
            if (percentage <= 0)
                return this.toVec4();
            if (percentage > 1F)
                percentage = 1F;

            Vector4 result = this.toVec4();

            MathUtil.smooth3(ref result.X, ref result.Y, ref result.Z, percentage);

            return result;
        }

        /*returns the provided color with its saturation decreased by the provided percentage (0.0 to 1.0)*/
        public static CustomColor reduceSaturation(Color color, float percentage)
        {
            if (percentage <= 0)
                return new CustomColor(color);
            if (percentage > 1F)
                percentage = 1F;

            float newRed = (float)color.R;
            float newGreen = (float)color.G;
            float newBlue = (float)color.B;

            MathUtil.smooth3(ref newRed, ref newGreen, ref newBlue, percentage);
            return new CustomColor(Color.FromArgb(color.A, (int)newRed, (int)newGreen, (int)newBlue));
        }

        public static Vector3 reduceSaturation(Vector3 color, float percentage)
        {
            if (percentage <= 0)
                return color;
            if (percentage > 1F)
                percentage = 1F;

            MathUtil.smooth3(ref color.X, ref color.Y, ref color.Z, percentage);

            return color;
        }

        /*changes the brightness of this color by the provided percentage (1.0F is 100%)*/
        public CustomColor setBrightPercent(float percentage)
        {
            systemColor = Color.FromArgb(systemColor.A, (int)(systemColor.R * percentage), (int)(systemColor.G * percentage), (int)(systemColor.B * percentage));
            return this;
        }

        public CustomColor setAlphaPercent(float percentage)
        {
            percentage = MathUtil.clamp(percentage, 0, 1);
            systemColor = Color.FromArgb((int)(systemColor.A * percentage),systemColor.R , systemColor.G , systemColor.B );
            return this;
        }
        
        public CustomColor setAlphaF(float a)
        {
            a = MathUtil.clamp(a, 0, 1);
            systemColor = Color.FromArgb((int)(255 * a), systemColor.R, systemColor.G, systemColor.B);
            return this;
        }

        public byte r { get => systemColor.R; set { systemColor = Color.FromArgb(systemColor.A, value, systemColor.G, systemColor.B); } }
        public byte g { get => systemColor.G; set { systemColor = Color.FromArgb(systemColor.A, systemColor.R, value, systemColor.B); } }
        public byte b { get => systemColor.B; set { systemColor = Color.FromArgb(systemColor.A, systemColor.R, systemColor.G, value); } }
        public byte a { get => systemColor.A; set { systemColor = Color.FromArgb(value, systemColor.R, systemColor.G, systemColor.B); } }
    }
}
