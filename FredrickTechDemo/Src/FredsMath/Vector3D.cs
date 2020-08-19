using System;

namespace FredrickTechDemo.FredsMath
{
    /*This is a double precision version of Vector3F*/
    public struct Vector3D
    {
        public static Vector3D zero = new Vector3D();
        public double x, y, z;
        public double r { get => x; set { x = value; } }
        public double g { get => y; set { y = value; } }
        public double b { get => z; set { z = value; } }
        //constructors
        /*public Vector3D()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }*/
        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3D(double w)
        {
            this.x = this.y = this.z = w;
        }
        public Vector3D(Vector3D copyVector)
        {
            this.x = copyVector.x;
            this.y = copyVector.y;
            this.z = copyVector.z;
        }

        //vector vector operators
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector3D operator -(Vector3D vec)
        {
            return new Vector3D(-vec.x, -vec.y, -vec.z);
        }

        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        //double operators, Post
        public static Vector3D operator +(Vector3D a, double b)
        {
            return new Vector3D(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3D operator -(Vector3D a, double b)
        {
            return new Vector3D(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.x * b, a.y * b, a.z * b);
        }
        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.x / b, a.y / b, a.z / b);
        }

        //double operators, Pre
        public static Vector3D operator +(double b, Vector3D a)
        {
            return new Vector3D(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3D operator -(double b, Vector3D a)
        {
            return new Vector3D(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3D operator *(double b, Vector3D a)
        {
            return new Vector3D(a.x * b, a.y * b, a.z * b);
        }

        //matrix vector operators
        public static Vector3D operator *(Matrix3F mat, Vector3D vec)//row major? (originates from aie tests)
        {
            return new Vector3D(
                    /*X*/mat.m1 * vec.x + mat.m4 * vec.y + mat.m7 * vec.z,
                    /*Y*/mat.m2 * vec.x + mat.m5 * vec.y + mat.m8 * vec.z,
                    /*Z*/mat.m3 * vec.x + mat.m6 * vec.y + mat.m9 * vec.z);
        }

        public static Vector3D operator *(Vector3D vec, Matrix4F mat)//row major? (originates from aie tests)
        {
            return new Vector3D(
                    /*X*/mat.m1 * vec.x + mat.m5 * vec.y + mat.m9 * vec.z,
                    /*Y*/mat.m2 * vec.x + mat.m6 * vec.y + mat.m10 * vec.z,
                    /*Z*/mat.m3 * vec.x + mat.m7 * vec.y + mat.m11 * vec.z);
        }
        //funcs
        public double Dot(Vector3D vec)
        {
            return this.x * vec.x + this.y * vec.y + this.z * vec.z;
        }

        public double Magnitude()
        {
            return (double)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public double horizontalMagnitude()
        {
            return (double)Math.Sqrt(this.x * this.x + this.z * this.z);
        }

        public Vector3D normalize()
        {
            double length = this.Magnitude();
            if (length != 0)
            {
                this.x /= length;
                this.y /= length;
                this.z /= length;
            }
            else
            {
                this.x = this.y = this.z = 0;
            }
            return this;
        }

        public static Vector3D clamp(Vector3D vector, Vector3D min, Vector3D max)
        {
            return new Vector3D(MathUtil.clamp(vector.x, min.x, max.x), MathUtil.clamp(vector.y, min.y, max.y), MathUtil.clamp(vector.z, min.z, max.z));
        }

        public static double distance(Vector3D vecA, Vector3D vecB)
        {
            double xsub = vecB.x - vecA.x;
            double ysub = vecB.y - vecA.y;
            double zsub = vecB.z - vecA.z;
            return Math.Sqrt(xsub * xsub + ysub * ysub + zsub * zsub);
        }
        public static double magnitude(Vector3D vec)
        {
            return Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        }
        public Vector3D Cross(Vector3D vec)
        {
            return new Vector3D(this.y * vec.z - this.z * vec.y, this.z * vec.x - this.x * vec.z, this.x * vec.y - this.y * vec.x);
        }
        public static Vector3D normalize(Vector3D vec)
        {
            Vector3D resultVec = vec;
            double length = magnitude(resultVec);
            if (length != 0)
            {
                resultVec.x /= length;
                resultVec.y /= length;
                resultVec.z /= length;
            }
            else
            {
                resultVec.x = resultVec.y = resultVec.z = 0;
            }
            return resultVec;
        }

        public static Vector3D cross(Vector3D vecA, Vector3D vecB)
        {
            return new Vector3D(vecA.y * vecB.z - vecA.z * vecB.y, vecA.z * vecB.x - vecA.x * vecB.z, vecA.x * vecB.y - vecA.y * vecB.x);
        }
        public static double dot(Vector3D vecA, Vector3D vecB)
        {
            return vecA.x * vecB.x + vecA.y * vecB.y + vecA.z * vecB.z;
        }

    }
}
