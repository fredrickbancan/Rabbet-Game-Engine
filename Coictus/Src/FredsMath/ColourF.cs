using System;

namespace Coictus.FredsMath
{
    public struct ColourF
    {
        #region pre-set colours;
        public static ColourF lightBlue = new ColourF(135, 135, 255);
        public static ColourF blue = new ColourF(0, 0, 255);
        public static ColourF darkBlue = new ColourF(0, 0, 100);
        public static ColourF lightRed = new ColourF(255, 135, 135);
        public static ColourF red = new ColourF(255, 0, 0);
        public static ColourF darkRed = new ColourF(100, 0, 0);
        public static ColourF lightGreen = new ColourF(135, 255, 135);
        public static ColourF green = new ColourF(0, 255, 0);
        public static ColourF darkGreen = new ColourF(0, 100, 0);
        public static ColourF white = new ColourF(255, 255, 255);
        public static ColourF black = new ColourF(0, 0, 0);
        public static ColourF lightGrey = new ColourF(175, 175, 175);
        public static ColourF grey = new ColourF(125, 125, 125);
        public static ColourF darkGrey = new ColourF(72, 72, 72);
        public static ColourF lightYellow = new ColourF(255, 255, 135);
        public static ColourF yellow = new ColourF(255, 255, 0);
        public static ColourF darkYellow = new ColourF(153, 153, 0);
        public static ColourF lightOrange = new ColourF(255, 175, 85);
        public static ColourF orange = new ColourF(255, 135, 0);
        public static ColourF darkOrange = new ColourF(175, 110, 0);
        public static ColourF ember = new ColourF(255, 100, 0);
        public static ColourF flame = new ColourF(255, 175, 0);
        public static ColourF facility = new ColourF(70, 120, 90);
        public static ColourF steelBlue = new ColourF(70, 100, 120);
        public static ColourF lightBlossom = new ColourF(255, 222, 236);
        public static ColourF blossom = new ColourF(255, 150, 190);
        public static ColourF darkBlossom = new ColourF(255, 72, 135);
        public static ColourF lightSkyBlue = new ColourF(165, 192, 235);
        public static ColourF skyBlue = new ColourF(0, 156, 252);
        public static ColourF aquaPain = new ColourF(0, 255, 255);
        public static ColourF hotPink = new ColourF(255, 105, 180);
        public static ColourF majenta = new ColourF(255, 0, 255);
        #endregion

        public UInt32 colour;

        //constructors
        /*public Colour()
        {
            colour = 0;
        }*/
        public ColourF(UInt32 color)
        {
            this.colour = color;
        }

        public ColourF(byte r, byte g, byte b, byte a = 255)
        {
            colour = (UInt32)(r << 24 | g << 16 | b << 8 | a);
        }


        //get functions
        public Vector4F normalVector4F()
        {
            return new Vector4F(MathUtil.normalize(0, 255, this.GetRed()), MathUtil.normalize(0, 255, this.GetGreen()), MathUtil.normalize(0, 255, this.GetBlue()), MathUtil.normalize(0, 255, this.GetAlpha()));
        }
        public Vector3F normalVector3F()
        {
            return new Vector3F(MathUtil.normalize(0, 255, this.GetRed()), MathUtil.normalize(0, 255, this.GetGreen()), MathUtil.normalize(0, 255, this.GetBlue()));
        }
        public byte GetRed()
        {
            return (byte)(colour >> 24);
        }
        public byte GetGreen()
        {
            return (byte)(colour >> 16);
        }
        public byte GetBlue()
        {
            return (byte)(colour >> 8);
        }
        public byte GetAlpha()
        {
            return (byte)(colour);
        }

        public float[] getNormalisedFloats()//returns colours as an array of floats with values 0 - 1 relative to the current values from 0 to 255, for opengl
        {
            return new float[] {MathUtil.normalize(0, 255, (float)this.GetRed()), MathUtil.normalize(0, 255, (float)this.GetGreen()), MathUtil.normalize(0, 255, (float)this.GetBlue()), MathUtil.normalize(0, 255, (float)this.GetAlpha())};
        }

        //set functions
        public void SetRed(byte red)
        {
            colour |= (UInt32) red << 24;
        }
        public void SetGreen(byte green)
        {
            colour |= (UInt32)green << 16;
        }
        public void SetBlue(byte blue)
        {
            colour |= (UInt32)blue << 8;
        }
        public ColourF SetAlpha(byte alpha)
        {
            colour |= (UInt32)alpha;
            return new ColourF(this.GetRed(), this.GetGreen(), this.GetBlue(), this.GetAlpha());
        }
    }
}
