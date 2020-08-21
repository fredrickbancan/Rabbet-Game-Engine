using System;

namespace FredrickTechDemo.FredsMath
{
    static class MathUtil
    {
        public static float normalize(float min, float max, float val)//returns a float in between 0 and 1 representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0.5
        {
            return (val - min) / (max - min);
        }

        public static float normalizeCustom(float mapMin, float mapMax, float min, float max, float val)//returns a float in between mapMin and mapMax representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0
        {
            return (mapMax - mapMin) * normalize(min, max, val) + mapMin;
        }

        public static double clamp(double val, double min, double max)
        {
            if (val < min) val = min;
            if (val > max) val = max;
            return val;
        }

        public static double radians(double degrees)
        {
            return degrees * 0.01745329251994329576923690768489D;
        }

        /*gives the dot product of the two provided coordinates*/
        public static float dotFloats(float xA, float yA, float zA, float xB, float yB, float zB)
        {
            return xA * xB + yA * yB + zA * zB;
        }

        public static double dotDoubles(double xA, double yA, double zA, double xB, double yB, double zB)
        {
            return xA * xB + yA * yB + zA * zB;
        }

        public static double hypotenuse(double a, double b)
        {
            return Math.Sqrt(a*a + b*b);
        }

        public static double max3(double a, double b, double c)
        {
            if(a >= b && a >= c)
            {
                return a;
            }
            if(b >= a && b >= c)
            {
                return b;
            }
            return c;
        }
        public static double min3(double a, double b, double c)
        {
            if (a <= b && a <= c)
            {
                return a;
            }
            if (b <= a && b <= c)
            {
                return b;
            }
            return c;
        }

        public static double max6(double a, double b, double c, double d, double e, double f)
        {
            if (a >= b && a >= c && a >= d && a >= e && a >= f)
            {
                return a;
            }
            if (b >= a && b >= c && b >= d && b >= e && b >= f)
            {
                return b;
            }
            if (c >= a && c >= b && c >= d && c >= e && c >= f)
            {
                return c;
            }
            if (d >= a && d >= b && d >= c && d >= e && d >= f)
            {
                return d;
            }
            if (e >= a && e >= b && e >= c && e >= d && e >= f)
            {
                return e;
            }
            return f;
        }
    
    }
}
