using System;

namespace FredrickTechDemo.FredsMath
{
    public enum Axis// simple enum for chosing axis to rotate on
    {
        y,
        z,
        x
    }
    public struct Matrix4F
    {
        public Vector4F row0;
        public Vector4F row1;
        public Vector4F row2;
        public Vector4F row3;

        #region constructors
        public Matrix4F(float identity)
        {
            this.row0 = new Vector4F(identity, 0, 0, 0);
            this.row1 = new Vector4F(0, identity, 0, 0);
            this.row2 = new Vector4F(0, 0, identity, 0);
            this.row3 = new Vector4F(0, 0, 0, identity);
        }

        public Matrix4F(Vector4F row0, Vector4F row1, Vector4F row2, Vector4F row3)
        {
            this.row0 = row0;
            this.row1 = row1;
            this.row2 = row2;
            this.row3 = row3;
        }

        public Matrix4F(float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float mm4, float m15, float m16)
        {
            this.row0 = new Vector4F(m1,m2,m3,m4);
            this.row1 = new Vector4F(m5,m6,m7,m8);
            this.row2 = new Vector4F(m9,m10,m11,m12);
            this.row3 = new Vector4F(m13,mm4,m15,m16);
        }
        #endregion

        #region get set
        public float m1
        {
            get { return this.row0.x; }
            set { this.row0.x = value; }
        }
        public float m2
        {
            get { return this.row0.y; }
            set { this.row0.y = value; }
        }
        public float m3
        {
            get { return this.row0.z; }
            set { this.row0.z = value; }
        }
        public float m4
        {
            get { return this.row0.w; }
            set { this.row0.w = value; }
        }
        public float m5
        {
            get { return this.row1.x; }
            set { this.row1.x = value; }
        }
        public float m6
        {
            get { return this.row1.y; }
            set { this.row1.y = value; }
        }
        public float m7
        {
            get { return this.row1.z; }
            set { this.row1.z = value; }
        }
        public float m8
        {
            get { return this.row1.w; }
            set { this.row1.w = value; }
        }
        public float m9
        {
            get { return this.row2.x; }
            set { this.row2.x = value; }
        }
        public float m10
        {
            get { return this.row2.y; }
            set { this.row2.y = value; }
        }
        public float m11
        {
            get { return this.row2.z; }
            set { this.row2.z = value; }
        }
        public float m12
        {
            get { return this.row2.w; }
            set { this.row2.w = value; }
        }
        public float m13
        {
            get { return this.row3.x; }
            set { this.row3.x = value; }
        }
        public float m14
        {
            get { return this.row3.y; }
            set { this.row3.y = value; }
        }
        public float m15
        {
            get { return this.row3.z; }
            set { this.row3.z = value; }
        }
        public float m16
        {
            get { return this.row3.w; }
            set { this.row3.w = value; }
        }

       
        #endregion

        #region matrix matrix operators
        public static Matrix4F operator * (Matrix4F matA, Matrix4F matB) // Row left column right (standard)
        {
            Matrix4F result = new Matrix4F();

            result.row0.x = (matA.m1 * matB.m1) + (matA.m2 * matB.m5) + (matA.m3 * matB.m9) + (matA.m4 * matB.m13);
            result.row0.y = (matA.m1 * matB.m2) + (matA.m2 * matB.m6) + (matA.m3 * matB.m10) + (matA.m4 * matB.m14);
            result.row0.z = (matA.m1 * matB.m3) + (matA.m2 * matB.m7) + (matA.m3 * matB.m11) + (matA.m4 * matB.m15);
            result.row0.w = (matA.m1 * matB.m4) + (matA.m2 * matB.m8) + (matA.m3 * matB.m12) + (matA.m4 * matB.m16);
            result.row1.x = (matA.m5 * matB.m1) + (matA.m6 * matB.m5) + (matA.m7 * matB.m9) + (matA.m8 * matB.m13);
            result.row1.y = (matA.m5 * matB.m2) + (matA.m6 * matB.m6) + (matA.m7 * matB.m10) + (matA.m8 * matB.m14);
            result.row1.z = (matA.m5 * matB.m3) + (matA.m6 * matB.m7) + (matA.m7 * matB.m11) + (matA.m8 * matB.m15);
            result.row1.w = (matA.m5 * matB.m4) + (matA.m6 * matB.m8) + (matA.m7 * matB.m12) + (matA.m8 * matB.m16);
            result.row2.x = (matA.m9 * matB.m1) + (matA.m10 * matB.m5) + (matA.m11 * matB.m9) + (matA.m12 * matB.m13);
            result.row2.y = (matA.m9 * matB.m2) + (matA.m10 * matB.m6) + (matA.m11 * matB.m10) + (matA.m12 * matB.m14);
            result.row2.z = (matA.m9 * matB.m3) + (matA.m10 * matB.m7) + (matA.m11 * matB.m11) + (matA.m12 * matB.m15);
            result.row2.w = (matA.m9 * matB.m4) + (matA.m10 * matB.m8) + (matA.m11 * matB.m12) + (matA.m12 * matB.m16);
            result.row3.x = (matA.m13 * matB.m1) + (matA.m14 * matB.m5) + (matA.m15 * matB.m9) + (matA.m16 * matB.m13);
            result.row3.y = (matA.m13 * matB.m2) + (matA.m14 * matB.m6) + (matA.m15 * matB.m10) + (matA.m16 * matB.m14);
            result.row3.z = (matA.m13 * matB.m3) + (matA.m14 * matB.m7) + (matA.m15 * matB.m11) + (matA.m16 * matB.m15);
            result.row3.w = (matA.m13 * matB.m4) + (matA.m14 * matB.m8) + (matA.m15 * matB.m12) + (matA.m16 * matB.m16);

            return result;
        }

        /// <summary>
        ///  Column left row right For use with provided unit tests. using & as symbol because * is used by opengl compatable multiplication function.
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        /// <returns></returns>
        public static Matrix4F operator & (Matrix4F matA, Matrix4F matB) // Column left row right For use with provided unit tests. 
        {
            Matrix4F result = new Matrix4F();
            result.row0.x = (matA.m1 * matB.m1) + (matA.m5 * matB.m2) + (matA.m9  * matB.m3) + (matA.m13 * matB.m4);
            result.row0.y = (matA.m2 * matB.m1) + (matA.m6 * matB.m2) + (matA.m10 * matB.m3) + (matA.m14 * matB.m4);
            result.row0.z = (matA.m3 * matB.m1) + (matA.m7 * matB.m2) + (matA.m11 * matB.m3) + (matA.m15 * matB.m4);
            result.row0.w = (matA.m4 * matB.m1) + (matA.m8 * matB.m2) + (matA.m12 * matB.m3) + (matA.m16 * matB.m4);
            result.row1.x = (matA.m1 * matB.m5) + (matA.m5 * matB.m6) + (matA.m9  * matB.m7) + (matA.m13 * matB.m8);
            result.row1.y = (matA.m2 * matB.m5) + (matA.m6 * matB.m6) + (matA.m10 * matB.m7) + (matA.m14 * matB.m8);
            result.row1.z = (matA.m3 * matB.m5) + (matA.m7 * matB.m6) + (matA.m11 * matB.m7) + (matA.m15 * matB.m8);
            result.row1.w = (matA.m4 * matB.m5) + (matA.m8 * matB.m6) + (matA.m12 * matB.m7) + (matA.m16 * matB.m8);
            result.row2.x = (matA.m1 * matB.m9) + (matA.m5 * matB.m10) + (matA.m9  * matB.m11) + (matA.m13 * matB.m12);
            result.row2.y = (matA.m2 * matB.m9) + (matA.m6 * matB.m10) + (matA.m10 * matB.m11) + (matA.m14 * matB.m12);
            result.row2.z = (matA.m3 * matB.m9) + (matA.m7 * matB.m10) + (matA.m11 * matB.m11) + (matA.m15 * matB.m12);
            result.row2.w = (matA.m4 * matB.m9) + (matA.m8 * matB.m10) + (matA.m12 * matB.m11) + (matA.m16 * matB.m12);
            result.row3.x = (matA.m1 * matB.m13) + (matA.m5 * matB.m14) + (matA.m9  * matB.m15) + (matA.m13 * matB.m16);
            result.row3.y = (matA.m2 * matB.m13) + (matA.m6 * matB.m14) + (matA.m10 * matB.m15) + (matA.m14 * matB.m16);
            result.row3.z = (matA.m3 * matB.m13) + (matA.m7 * matB.m14) + (matA.m11 * matB.m15) + (matA.m15 * matB.m16);
            result.row3.w = (matA.m4 * matB.m13) + (matA.m8 * matB.m14) + (matA.m12 * matB.m15) + (matA.m16 * matB.m16);

            return result;
        }
        public static Matrix4F operator + (Matrix4F matA, Matrix4F matB)
        {
            return new Matrix4F(matA.row0 + matB.row0, matA.row1 + matB.row1, matA.row2 + matB.row2, matA.row3 + matB.row3);
        }
        public static Matrix4F operator - (Matrix4F matA, Matrix4F matB)
        {
            return new Matrix4F(matA.row0 - matB.row0, matA.row1 - matB.row1, matA.row2 - matB.row2, matA.row3 - matB.row3);
        }
        public static Matrix4F operator * (Matrix4F matA, float value)
        {
            return new Matrix4F(matA.row0 * value, matA.row1 * value, matA.row2 * value, matA.row3 * value);
        }
        

        #endregion


        #region functions
        public void SetRotateX(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads);
            this.row1.y = cos;
            this.row1.z = sin;
            this.row2.y = -sin;
            this.row2.z = cos;
        }

        public void SetRotateY(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads);
            this.row0.x = cos;
            this.row0.z = -sin;
            this.row2.x = sin;
            this.row2.z = cos;
        }

        public void SetRotateZ(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads);
            this.row0.x = cos;
            this.row0.y = sin;
            this.row1.x = -sin;
            this.row1.y = cos;
        }

        public static Matrix4F rotate(float degrees, Axis axis)
        {
            float rads = (float)MathUtil.radians(degrees);
            Matrix4F result = new Matrix4F(1.0F);
            if(axis == Axis.x)
            {
                result.SetRotateX(rads);
            }
            else if(axis == Axis.y)
            {
                result.SetRotateY(rads);
            }
            else if(axis == Axis.z)
            {
                result.SetRotateZ(rads);
            }
            return result; 
        }
        public static Matrix4F rotate(Vector3F rotationVector)
        {
            return rotate(rotationVector.x, Axis.x) * rotate(rotationVector.y, Axis.y) * rotate(rotationVector.z, Axis.z);
        }
        public static Matrix4F translate(Vector3F vec)
        {
            Matrix4F result = new Matrix4F(1.0F);
            result.row3 = new Vector4F(vec.x, vec.y, vec.z, 1.0F);
            return result;
        }
        public static Matrix4F scale(Vector3F scaleVec)
        {
            Matrix4F result = new Matrix4F(1.0F);
            result.m1 = scaleVec.x;
            result.m6 = scaleVec.y;
            result.m11 = scaleVec.z;
            return result;
        }
        public static Matrix4F lookAt(Vector3D eye, Vector3D target, Vector3D up)
        {
            return lookAt(new Vector3F((float)eye.x, (float)eye.y, (float)eye.z), new Vector3F((float)target.x, (float)target.y, (float)target.z), new Vector3F((float)up.x, (float)up.y, (float)up.z));
        }
        public static Matrix4F lookAt(Vector3F eye, Vector3F target, Vector3F up)
         {
             Vector3F front = new Vector3F(Vector3F.normalize(eye - target));
             Vector3F side = new Vector3F(Vector3F.cross(up, front));
             Vector3F upVector = new Vector3F(Vector3F.cross(front, side));

             Matrix4F translateToEye = createTranslationMatrix(-eye.x, -eye.y, -eye.z);
             Matrix4F rotateToTarget = new Matrix4F(new Vector4F(side.x, upVector.x, front.x, 0.0F), new Vector4F(side.y, upVector.y, front.y, 0.0F), new Vector4F(side.z, upVector.z, front.z, 0.0F), new Vector4F(0.0F, 0.0F, 0.0F, 1.0F));

             return translateToEye * rotateToTarget;
         }

        public static Matrix4F createTranslationMatrix(float x, float y, float z)
        {
            Matrix4F result = new Matrix4F(1.0F);
            result.m13 = x;
            result.m14 = y;
            result.m15 = z;
            return result;
        }
        public static Matrix4F createTranslationMatrix(Vector3F vec)
        {
            Matrix4F result = new Matrix4F(1.0F);
            result.m13 = vec.x;
            result.m14 = vec.y;
            result.m15 = vec.z;
            return result;
        }

        public static Matrix4F createPerspectiveMatrix(float fov, float aspectRatio, float zNear, float zFar)
        {
            Matrix4F result = new Matrix4F(1.0F);
            float yMax = zNear * (float)Math.Tan(0.5F * fov);
            float yMin = -yMax;
            float xMin = yMin * aspectRatio;
            float xMax = yMax * aspectRatio;
            float x = 2.0F * zNear / (xMax - xMin);
            float y = 2.0F * zNear / (yMax - yMin);
            float a = (xMax + xMin) / (xMax - xMin);
            float b = (yMax + yMin) / (yMax - yMin);
            float c = -(zFar + zNear) / (zFar - zNear);
            float d = -(2.0F * zFar * zNear) / (zFar - zNear);

            result.row0.x = x;
            result.row0.y = 0;
            result.row0.z = 0;
            result.row0.w = 0;
            result.row1.x = 0;
            result.row1.y = y;
            result.row1.z = 0;
            result.row1.w = 0;
            result.row2.x = a;
            result.row2.y = b;
            result.row2.z = c;
            result.row2.w = -1;
            result.row3.x = 0;
            result.row3.y = 0;
            result.row3.z = d;
            result.row3.w = 0;

            return result; 
        }

        public static void translateXYZFloats(Vector3F translation, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            newX = x + translation.x;
            newY = y + translation.y;
            newZ = z + translation.z;
        }
        public static void rotateXYZFloats(Vector3F newRotation, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            Matrix4F rotMat = rotate(newRotation);
            newX = MathUtil.dotFloats(rotMat.m1, rotMat.m2, rotMat.m3, x, y, z) + rotMat.m4;
            newY = MathUtil.dotFloats(rotMat.m5, rotMat.m6, rotMat.m7, x, y, z) + rotMat.m8;
            newZ = MathUtil.dotFloats(rotMat.m9, rotMat.m10, rotMat.m11, x, y, z) + rotMat.m12;
        }
        public static void scaleXYZFloats(Vector3F newScale, float x, float y, float z, out float newX, out float newY, out float newZ)
        {
            newX = x * newScale.x;
            newY = y * newScale.y;
            newZ = z * newScale.z;
        }

        public static void rotateXYZDoubles(Vector3D newRotation, double x, double y, double z, out double newX, out double newY, out double newZ)
        {
            Matrix4F rotMat = rotate(Vector3F.convert(newRotation));
            newX = MathUtil.dotDoubles(rotMat.m1, rotMat.m2, rotMat.m3, x, y, z) + rotMat.m4;
            newY = MathUtil.dotDoubles(rotMat.m5, rotMat.m6, rotMat.m7, x, y, z) + rotMat.m8;
            newZ = MathUtil.dotDoubles(rotMat.m9, rotMat.m10, rotMat.m11, x, y, z) + rotMat.m12;
        }
        #endregion
    }
}
