using OpenTK.Mathematics;

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
        public static CustomColor flame = new CustomColor(255, 183, 0);
        public static CustomColor facility = new CustomColor(70, 120, 90);
        public static CustomColor steelBlue = new CustomColor(70, 100, 120);
        public static CustomColor lightBlossom = new CustomColor(255, 222, 236);
        public static CustomColor blossom = new CustomColor(255, 150, 190);
        public static CustomColor darkBlossom = new CustomColor(255, 72, 135);
        public static CustomColor dusk = new CustomColor(255, 72, 100);
        public static CustomColor lightSkyBlue = new CustomColor(165, 192, 235);
        public static CustomColor skyBlue = new CustomColor(0, 156, 252);
        public static CustomColor aquaPain = new CustomColor(0, 255, 255);
        public static CustomColor hotPink = new CustomColor(255, 105, 180);
        public static CustomColor magenta = new CustomColor(255, 0, 255);

        Color4 baseColor;//color is stored using system color struct

        public CustomColor(Color4 setColor)
        {
            baseColor = setColor;
        }
        public CustomColor(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            baseColor = new Color4(r,g,b,a);
        }
        public CustomColor(float r, float g, float b, float a = 255.0F)
        {
            baseColor = new Color4(r, g, b, a);
        }

        public CustomColor setColor(Color4 newColor)
        {
            baseColor = newColor;
            return this;
        }

        /*returns the rgb(a) values of this color normalized from 0 to 1 instead of 0 to 255, as a vector*/
        public Vector3 toNormalVec3()
        {
            return new Vector3(baseColor.R, baseColor.G, baseColor.B);
        }

        public Vector4 toNormalVec4()
        {
            return new Vector4(baseColor.R, baseColor.G, baseColor.B, baseColor.A);
        }

        /*returns the provided color with its vibrancy decreased by the provided percentage (0.0 to 1.0)*/
        public CustomColor reduceVibrancy(float percentage)
        {
            float newRed = baseColor.R;
            float newGreen = baseColor.G;
            float newBlue = baseColor.B;

            MathUtil.smooth3(ref newRed, ref newGreen, ref newBlue, percentage);
            return new CustomColor(newRed, newGreen, newBlue, baseColor.A);
        }
        /*changes the brightness of this color by the provided percentage (1.0F is 100%)*/
        public CustomColor setBrightPercent(float percentage)
        {
            baseColor =new Color4(baseColor.R * percentage, baseColor.G * percentage, baseColor.B * percentage, baseColor.A);
            return this;
        }

        public CustomColor mix(CustomColor other, float ratio)
        {
            return new CustomColor(MathUtil.lerp(r, other.r, ratio), MathUtil.lerp(g, other.g, ratio), MathUtil.lerp(b, other.b, ratio), a);
        }

        public CustomColor setAlphaPercent(float percentage)
        {
            percentage = MathUtil.clamp(percentage, 0, 1);
            baseColor.A *= percentage;
            return this;
        }
        
        public CustomColor setAlphaF(float a)
        {
            a = MathUtil.clamp(a, 0, 1);
            baseColor.A = a;
            return this;
        }

        public CustomColor copy()
        {
            return new CustomColor(baseColor);
        }

        public float r { get => baseColor.R; set { baseColor.R = value; } }
        public float g { get => baseColor.G; set { baseColor.G = value; } }
        public float b { get => baseColor.B; set { baseColor.B = value; } }
        public float a { get => baseColor.A; set { baseColor.A = value; } }
    }
}
