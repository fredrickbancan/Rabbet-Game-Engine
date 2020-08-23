namespace Coictus.FredsMath
{
    /*Represents a vector that is used to encode three-dimensional physical rotations.*/
    public struct QuaternionF
    {
        public Vector4F values;

        public QuaternionF(Vector4F values)
        {
            this.values = values;
        }

        public QuaternionF(float x, float y, float z, float w)
        {
            this.values = new Vector4F(x,y,z,w);
        }

        public static QuaternionF operator * (QuaternionF quatA, QuaternionF quatB)
        {
            return new QuaternionF();
        }

        public float length()
        {
            return values.Magnitude();
        }

        public static QuaternionF normalize(QuaternionF quat)
        {
            QuaternionF result = new QuaternionF(quat.values);

            result.values.Normalize();

            return result;
        }

        public QuaternionF normalize()
        {
            values.Normalize();
            return this;
        }

        public QuaternionF conjugate()
        {
            values = new Vector4F(-values.x, -values.y, -values.z, values.w);
            return this;
        }
    }
}
