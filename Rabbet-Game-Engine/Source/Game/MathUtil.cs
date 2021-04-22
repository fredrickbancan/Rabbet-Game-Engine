using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    static class MathUtil
    {
        public static readonly Vector3 up = new Vector3(0, 1, 0);
        public static readonly Vector3 front = new Vector3(0, 0, -1);

        public static float rand(Vector3 xyz)//returns random float using input vector3 as seed
        {
            return fract((float)(System.Math.Tan(Vector2.Distance(xyz.Xy * 1.6180339F, xyz.Xy) * xyz.Z) * xyz.Y));
        }

        public static float fract(float f)//returns the fractional value of the float passed. eg: 1.5F will return 0.5F
        {
            return f - (int)f;
        }

        public static float normalize(float min, float max, float val)//returns a float in between 0 and 1 representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0.5
        {
            return (val - min) / (max - min);
        }
        public static float normalizeClamped(float min, float max, float val)//returns a float in between 0 and 1 representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0.5
        {
            return clamp((val - min) / (max - min), 0, 1);
        }

        /// <summary>
        /// returns a float in between mapMin and mapMax representing the percentage that VAL is from Min to Max.
        /// </summary>
        public static float normalizeCustom(float mapMin, float mapMax, float min, float max, float val)
        {
            return (mapMax - mapMin) * normalize(min, max, val) + mapMin;
        }

        public static float clamp(float val, float min, float max)
        {
            if (val < min) val = min;
            if (val > max) val = max;
            return val;
        }
        public static Vector3 lerp(Vector3 src, Vector3 dest, float factor)
        {
            return src + (dest - src) * factor;
        }

        public static float lerp(float src, float dest, float factor)
        {
            return src + (dest - src) * factor;
        }
        public static float lerpF(float src, float dest, float factor)
        {
            return src + (dest - src) * factor;
        }
        public static float radians(float degrees)
        {
            return degrees * 0.01745329251994329576923690768489F;
        }
        public static Vector3 radians(Vector3 degrees)
        {
            return degrees * 0.01745329251994329576923690768489F;
        }

        /*gives the dot product of the two provided coordinates*/
        public static float dotFloats(float xA, float yA, float zA, float xB, float yB, float zB)
        {
            return xA * xB + yA * yB + zA * zB;
        }


        public static float hypotenuse(float a, float b)
        {
            return (float)System.Math.Sqrt(a * a + b * b);
        }

        /*Takes in 3 floats and a smooth factor. Smooths the 3 values with strength of smooth factor. Smooth factor of 0 does nothing, and 1 makes each value the average.*/
        public static void smooth3(ref float x, ref float y, ref float z, float factor)
        {
            float avg = (x + y + z) / 3F;
            x = lerp(x, avg, factor);
            y = lerp(y, avg, factor);
            z = lerp(z, avg, factor);
        }

        public static float max3(float a, float b, float c)
        {
            if (a >= b && a >= c)
            {
                return a;
            }
            if (b >= a && b >= c)
            {
                return b;
            }
            return c;
        }

        public static float min3(float a, float b, float c)
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

        public static float max6(float a, float b, float c, float d, float e, float f)
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

        public static Vector3i rightShift(Vector3i v, int n)
        {
            return new Vector3i(v.X >> n, v.Y >> n, v.Z >> n);
        }

        public static Vector3i leftShift(Vector3i v, int n)
        {
            return new Vector3i(v.X << n, v.Y << n, v.Z << n);
        }

        public static int manhattanDist(Vector3i a, Vector3i b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }

        //returns the centroid pos vector of the provided triangle
        public static Vector3 getTriangleCentroid(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC)
        {
            return new Vector3((cornerA.X + cornerB.X + cornerC.X) / 3, (cornerA.Y + cornerB.Y + cornerC.Y) / 3, (cornerA.Z + cornerB.Z + cornerC.Z) / 3);
        }

        //returns the Z value of the centroid of the provided triangle, useful for Z sorting when converting points to screenspace
        public static float getTriangleCentroidZ(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC)
        {
            return (cornerA.Z + cornerB.Z + cornerC.Z) / 3;
        }

        public static Vector3 getQuadCentroid(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC, Vector3 cornerD)
        {
            return new Vector3((cornerA.X + cornerB.X + cornerC.X + cornerD.X) / 4, (cornerA.Y + cornerB.Y + cornerC.Y + cornerD.Y) / 4, (cornerA.Z + cornerB.Z + cornerC.Z + cornerD.Z) / 4);
        }

        //returns the Z value of the centroid of the provided quad, useful for Z sorting when converting points to screenspace
        public static float getQuadCentroidZ(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC, Vector3 cornerD)
        {
            return (cornerA.Z + cornerB.Z + cornerC.Z + cornerD.Z) / 4;
        }

        public static Vector3 abs(Vector3 vec)
        {
            return new Vector3(System.Math.Abs(vec.X), System.Math.Abs(vec.Y), System.Math.Abs(vec.Z));
        }

        public static Matrix4 dirVectorToRotation(Vector3 dir)
        {
            Matrix4 result = Matrix4.Identity;

            Vector3 xAxis = Vector3.Cross(up, dir);
            xAxis.NormalizeFast();
            Vector3 yAxis = Vector3.Cross(dir, xAxis);
            yAxis.NormalizeFast();
            result.M11 = xAxis.X;
            result.M21 = yAxis.X;
            result.M31 = dir.X;

            result.M12 = xAxis.Y;
            result.M22 = yAxis.Y;
            result.M32 = dir.Y;

            result.M13 = xAxis.Z;
            result.M23 = yAxis.Z;
            result.M33 = dir.Z;
            return result;
        }

        public static Matrix4 dirVectorToRotationNoFlip(Vector3 dir)
        {
            Matrix4 result = Matrix4.Identity;
            bool b = dir.X < 0 || dir.Z < 0;
            Vector3 xAxis = Vector3.Cross(b ? -up : up, dir);//this method of avoiding rotation only works if rotation is aligned with perfectly with up.
            xAxis.NormalizeFast();
            Vector3 yAxis = Vector3.Cross(dir, xAxis);
            yAxis.NormalizeFast();
            result.M11 = xAxis.X;
            result.M21 = yAxis.X;
            result.M31 = dir.X;

            result.M12 = xAxis.Y;
            result.M22 = yAxis.Y;
            result.M32 = dir.Y;

            result.M13 = xAxis.Z;
            result.M23 = yAxis.Z;
            result.M33 = dir.Z;
            return result;
        }

        public static Matrix4 createRotation(Vector3 rot)
        {
            rot = radians(rot);
            return Matrix4.CreateRotationX(rot.X) * Matrix4.CreateRotationY(rot.Y) * Matrix4.CreateRotationZ(rot.Z);
        }

        public static Matrix4 lerp(Matrix4 src, Matrix4 dest, float factor)
        {
            return src + (dest - src) * factor;
        }

        /*Scales the provided floats the same way a matrix would*/
        public static void scaleXYZFloats(Vector3 newScale, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            newX = x * newScale.X;
            newY = y * newScale.Y;
            newZ = z * newScale.Z;
        }

        /*Rotates the provided floats the same way a matrix would*/
        public static void rotateXYZFloats(Vector3 newRotation, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            Matrix4 rotMat = createRotation(newRotation);
            newX = MathUtil.dotFloats(rotMat.M11, rotMat.M12, rotMat.M13, x, y, z) + rotMat.M14;
            newY = MathUtil.dotFloats(rotMat.M21, rotMat.M22, rotMat.M23, x, y, z) + rotMat.M24;
            newZ = MathUtil.dotFloats(rotMat.M31, rotMat.M32, rotMat.M33, x, y, z) + rotMat.M34;
        }

        /*Translates the provided floats the same way a matrix would*/
        public static void translateXYZFloats(Vector3 translation, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            newX = x + translation.X;
            newY = y + translation.Y;
            newZ = z + translation.Z;
        }
    }
}
