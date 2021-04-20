using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    /*Simple color wrapper struct for the default System.Drawing color struct. Allows custom functionality.*/
    public struct Color
    {
        /*some custom presets*/
        public static readonly Color blue = new Color(0, 0, 255);
        public static readonly Color lightBlue = new Color(135, 135, 255);
        public static readonly Color darkBlue = new Color(0, 0, 100);
        public static readonly Color lightRed = new Color(255, 135, 135);
        public static readonly Color red = new Color(255, 0, 0);
        public static readonly Color darkRed = new Color(100, 0, 0);
        public static readonly Color lightGreen = new Color(135, 255, 135);
        public static readonly Color green = new Color(0, 255, 0);
        public static readonly Color darkGreen = new Color(0, 100, 0);
        public static readonly Color white = new Color(255, 255, 255);
        public static readonly Color black = new Color(0, 0, 0);
        public static readonly Color lightGrey = new Color(175, 175, 175);
        public static readonly Color grey = new Color(125, 125, 125);
        public static readonly Color darkGrey = new Color(72, 72, 72);
        public static readonly Color lightYellow = new Color(255, 255, 135);
        public static readonly Color yellow = new Color(255, 255, 0);
        public static readonly Color darkYellow = new Color(153, 153, 0);
        public static readonly Color lightOrange = new Color(255, 160, 54);
        public static readonly Color orange = new Color(255, 135, 0);
        public static readonly Color darkOrange = new Color(175, 110, 0);
        public static readonly Color ember = new Color(255, 100, 0);
        public static readonly Color flame = new Color(255, 183, 0);
        public static readonly Color facility = new Color(70, 120, 90);
        public static readonly Color steelBlue = new Color(70, 100, 120);
        public static readonly Color lightBlossom = new Color(255, 222, 236);
        public static readonly Color blossom = new Color(255, 150, 190);
        public static readonly Color darkBlossom = new Color(255, 72, 135);
        public static readonly Color dusk = new Color(255, 72, 100);
        public static readonly Color lightSkyBlue = new Color(165, 192, 235);
        public static readonly Color skyBlue = new Color(0, 156, 252);
        public static readonly Color aquaPain = new Color(0, 255, 255);
        public static readonly Color hotPink = new Color(255, 105, 180);
        public static readonly Color magenta = new Color(255, 0, 255);

        Color4 baseColor;//color is stored using system color struct

        public Color(Color4 setColor)
        {
            baseColor = setColor;
        }
        public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            baseColor = new Color4(r, g, b, a);
        }
        public Color(float r, float g, float b, float a = 255.0F)
        {
            baseColor = new Color4(r, g, b, a);
        }

        public Color setColor(Color4 newColor)
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
        public Color reduceVibrancy(float percentage)
        {
            float newRed = baseColor.R;
            float newGreen = baseColor.G;
            float newBlue = baseColor.B;

            MathUtil.smooth3(ref newRed, ref newGreen, ref newBlue, percentage);
            return new Color(newRed, newGreen, newBlue, baseColor.A);
        }
        /*changes the brightness of this color by the provided percentage (1.0F is 100%)*/
        public Color setBrightPercent(float percentage)
        {
            return new Color(new Color4(baseColor.R * percentage, baseColor.G * percentage, baseColor.B * percentage, baseColor.A));
        }

        public Color mix(Color other, float ratio)
        {
            return new Color(MathUtil.lerp(r, other.r, ratio), MathUtil.lerp(g, other.g, ratio), MathUtil.lerp(b, other.b, ratio), a);
        }
        public static Color mix(Color colorA, Color colorB, float ratio)
        {
            return new Color(MathUtil.lerp(colorA.r, colorB.r, ratio), MathUtil.lerp(colorA.g, colorB.g, ratio), MathUtil.lerp(colorA.b, colorB.b, ratio), MathUtil.lerp(colorA.a, colorB.a, ratio));
        }

        public Color setAlphaPercent(float percentage)
        {
            percentage = MathUtil.clamp(percentage, 0, 1);
            baseColor.A *= percentage;
            return this;
        }

        public Color setAlphaF(float a)
        {
            a = MathUtil.clamp(a, 0, 1);
            baseColor.A = a;
            return this;
        }

        public Color copy()
        {
            return new Color(baseColor);
        }

        public float r { get => baseColor.R; set { baseColor.R = value; } }
        public float g { get => baseColor.G; set { baseColor.G = value; } }
        public float b { get => baseColor.B; set { baseColor.B = value; } }
        public float a { get => baseColor.A; set { baseColor.A = value; } }
    }
}
