using System;
namespace FredrickTechDemo.FredsMath
{
    public struct Vector2F
    {
        public float x, y;

        public static readonly Vector2F zero = new Vector2F(0.0F, 0.0F);
        //constructors
        public Vector2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2F(float w)
        {
            this.x = this.y  = w;
        }
        public Vector2F(Vector2F copyVector)
        {
            this.x = copyVector.x;
            this.y = copyVector.y;
        }

        //vector vector operators
        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.x + b.x, a.y + b.y);
        }
        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.x - b.x, a.y - b.y);
        }

        public static Vector2F operator *(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.x * b.x, a.y * b.y);
        }

        //Float operators, Post
        public static Vector2F operator +(Vector2F a, float b)
        {
            return new Vector2F(a.x + b, a.y + b);
        }

        public static Vector2F operator -(Vector2F a, float b)
        {
            return new Vector2F(a.x - b, a.y - b);
        }

        public static Vector2F operator *(Vector2F a, float b)
        {
            return new Vector2F(a.x * b, a.y * b);
        }

        //Float operators, Pre
        public static Vector2F operator +(float b, Vector2F a)
        {
            return new Vector2F(a.x + b, a.y + b);
        }

        public static Vector2F operator -(float b, Vector2F a)
        {
            return new Vector2F(a.x - b, a.y - b);
        }

        public static Vector2F operator *(float b, Vector2F a)
        {
            return new Vector2F(a.x * b, a.y * b);
        }


        //funcs

        public float Magnitude()
        {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y);
        }

        public void Normalize()
        {
            float length = this.Magnitude();
            if (length != 0)
            {
                this.x /= length;
                this.y /= length;
            }
            else
            {
                this.x = this.y = 0;
            }
        }

        public static float magnitude(Vector2F vec)
        {
            return (float)Math.Sqrt(vec.x * vec.x + vec.y * vec.y );
        }
        public static Vector2F normalize(Vector2F vec)
        {
            float length = magnitude(vec);
            if (length != 0)
            {
                vec.x /= length;
                vec.y /= length;
            }
            else
            {
                vec.x = vec.y = 0;
            }
            return vec;
        }

        public static void translateFloats(Vector2F translation, float srcX, float srcY, out float newX, out float newY)
        {
            newX = srcX + translation.x;
            newY = srcY + translation.y;
        }

    }
}
