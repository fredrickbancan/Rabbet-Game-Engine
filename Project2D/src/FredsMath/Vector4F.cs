using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredsMath
{
    public class Vector4F
    {
        public float x, y, z, w;

        //constructors

        public Vector4F()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        public Vector4F(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4F(float j)
        {
            this.x = this.y = this.z = this.w = j; 
        }

        public Vector4F(Vector4F copyVector)
        {
            this.x = copyVector.x;
            this.y = copyVector.y;
            this.z = copyVector.z;
            this.w = copyVector.w;
        }

        //matrix vector operators
        public static Vector4F operator *(Matrix4F mat, Vector4F vec) // column major vector multiplication
        {
            return new Vector4F(
                    /*X*/mat.m1 * vec.x + mat.m5 * vec.y + mat.m9 * vec.z + mat.m13 * vec.w,
                    /*Y*/mat.m2 * vec.x + mat.m6 * vec.y + mat.m10 * vec.z + mat.m14 * vec.w,
                    /*Z*/mat.m3 * vec.x + mat.m7 * vec.y + mat.m11 * vec.z + mat.m15 * vec.w,
                    /*W*/mat.m4 * vec.x + mat.m8 * vec.y + mat.m12 * vec.z + mat.m16 * vec.w);
        }

        //vector vector operators
        public static Vector4F operator + (Vector4F a, Vector4F b)
        {
            return new Vector4F(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4F operator - (Vector4F a, Vector4F b)
        {
            return new Vector4F(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4F operator * (Vector4F a, Vector4F b)
        {
            return new Vector4F(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        //Float operators, post
        public static Vector4F operator + (Vector4F a, float b)
        {
            return new Vector4F(a.x + b, a.y + b, a.z + b, a.w + b);
        }

        public static Vector4F operator - (Vector4F a, float b)
        {
            return new Vector4F(a.x - b, a.y - b, a.z - b, a.w - b);
        }

        public static Vector4F operator * (Vector4F a, float b)
        {
            return new Vector4F(a.x * b, a.y * b, a.z * b, a.w * b);
        }

        //Float operators, pre
        public static Vector4F operator + (float b, Vector4F a)
        {
            return new Vector4F(a.x + b, a.y + b, a.z + b, a.w + b);
        }

        public static Vector4F operator - (float b, Vector4F a)
        {
            return new Vector4F(a.x - b, a.y - b, a.z - b, a.w - b);
        }

        public static Vector4F operator * (float b, Vector4F a)
        {
            return new Vector4F(a.x * b, a.y * b, a.z * b, a.w * b);
        }

        //funcs
        public float Dot(Vector4F vec)
        {
            return this.x * vec.x + this.y * vec.y + this.z * vec.z + this.w * vec.w;
        }
        public float Magnitude()
        {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
        }

        public void Normalize()
        {
            float length = this.Magnitude();
            if (length != 0) //cannot divide by zero
            {
                this.x /= length;
                this.y /= length;
                this.z /= length;
                this.w /= length;
            }
            else
            {
                this.x = this.y = this.z = this.w = 0;
            }
        }
        public Vector4F Cross(Vector4F vec)
        {
            return new Vector4F(this.y * vec.z - this.z * vec.y, this.z * vec.x - this.x * vec.z, this.x * vec.y - this.y * vec.x, 0);
        }
    }
}
