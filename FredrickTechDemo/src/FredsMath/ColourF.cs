using System;

namespace FredrickTechDemo.FredsMath
{
    public struct ColourF
    {
        #region pre-set colours;
        public static ColourF blue = new ColourF(0, 0, 255);
        public static ColourF darkBlue = new ColourF(0, 0, 100);
        public static ColourF red = new ColourF(255, 0, 0);
        public static ColourF darkRed = new ColourF(100, 0, 0);
        public static ColourF green = new ColourF(0, 255, 0);
        public static ColourF darkGreen = new ColourF(0, 100, 0);
        public static ColourF white = new ColourF(255, 255, 255);
        public static ColourF black = new ColourF(0, 0, 0);
        public static ColourF lightGrey = new ColourF(100, 100, 100);
        public static ColourF darkGrey = new ColourF(50, 50, 50);
        public static ColourF yellow = new ColourF(255, 255, 0);
        public static ColourF darkYellow = new ColourF(153, 153, 0);
        public static ColourF facility = new ColourF(35, 75, 75);
        public static ColourF steelBlue = new ColourF(70, 130, 180);
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
        public void SetAlpha(byte alpha)
        {
            colour |= (UInt32)alpha;
        }
    }
}
