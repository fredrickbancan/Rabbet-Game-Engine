using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredsMath
{
    static class MathUtil
    {
        public static float normalize(float min, float max, float val)//returns a float in between 0 and 1 representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0.5
        {
            return (val - min/*amount between min and the value*/) / (max - min/*difference between min and max*/);
        }
        public static Vector3F normalize(Vector3F vec)
        {
            Vector3F result = new Vector3F();
            result.Normalize();
            return result;
        }

        public static Vector3F cross(Vector3F vecA, Vector3F vecB)
        {
            Vector3F result = new Vector3F(vecA);
            result.Cross(vecB);
            return result;
        }

        public static float dot(Vector3F vecA, Vector3F vecB)
        {
            Vector3F result = new Vector3F(vecA);
            return result.Dot(vecB);
        }

        public static double radians(double degrees)
        {
            return degrees * 0.01745329251994329576923690768489D;
        }

        public static Matrix4F perspectiveMatrix(float fovRadians, float aspectRatio, float zNear, float zFar)
        {
            float tanHalfFov = (float)Math.Tan((double)(fovRadians / 2.0F));

            Matrix4F result = new Matrix4F();
            result.m1 = 1 / (aspectRatio * tanHalfFov);
            result.m6 = 1 / (tanHalfFov);
            result.m11 = - (zFar + zNear) / (zFar - zNear);
            result.m12 = -1;
            result.m15 = -(2 * zFar * zNear) / (zFar - zNear);
            return result;
        }

        public static Matrix4F lookAt(Vector3F eye, Vector3F center, Vector3F up)
        {
            Vector3F f = new Vector3F(normalize(center - eye));
            Vector3F s = new Vector3F(cross(f, up));
            Vector3F u = new Vector3F(cross(s,f));
            
            Matrix4F result = new Matrix4F();
            result.m1 = s.x;
            result.m5 = s.y;
            result.m9 = s.z;
            result.m2 = u.x;
            result.m6 = u.y;
            result.m10 = u.z;
            result.m3 = -f.x;
            result.m7 = -f.y;
            result.m11 = -f.z;
            result.m13 = -dot(s, eye);
            result.m14 = -dot(u, eye);
            result.m15 = dot(f, eye);

            return result;
        }
    }
}
