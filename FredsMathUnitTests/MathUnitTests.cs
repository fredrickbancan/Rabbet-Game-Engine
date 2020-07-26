using System;
using FredsMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTestProject
{
    [TestClass]
    public class MathUnitTests
    {
        const float DEFAULT_TOLERANCE = 0.0001f;

        bool compare(float a, float b, float tolerance = DEFAULT_TOLERANCE)
        {
            if (Math.Abs(a - b) > tolerance)
                return false;
            return true;
        }

        bool compare(Vector3F a, Vector3F b, float tolerance = DEFAULT_TOLERANCE)
        {
            if (Math.Abs(a.x - b.x) > tolerance ||
                Math.Abs(a.y - b.y) > tolerance ||
                Math.Abs(a.z - b.z) > tolerance)
                return false;
            return true;
        }

        bool compare(Vector4F a, Vector4F b, float tolerance = DEFAULT_TOLERANCE)
        {
            if (Math.Abs(a.x - b.x) > tolerance ||
                Math.Abs(a.y - b.y) > tolerance ||
                Math.Abs(a.z - b.z) > tolerance ||
                Math.Abs(a.w - b.w) > tolerance)
                return false;
            return true;
        }

        bool compare(Matrix3F a, Matrix3F b, float tolerance = DEFAULT_TOLERANCE)
        {
            if (Math.Abs(a.m1 - b.m1) > tolerance || Math.Abs(a.m2 - b.m2) > tolerance || Math.Abs(a.m3 - b.m3) > tolerance ||
                Math.Abs(a.m4 - b.m4) > tolerance || Math.Abs(a.m5 - b.m5) > tolerance || Math.Abs(a.m6 - b.m6) > tolerance ||
                Math.Abs(a.m7 - b.m7) > tolerance || Math.Abs(a.m8 - b.m8) > tolerance || Math.Abs(a.m9 - b.m9) > tolerance)
                return false;
            return true;
        }

        bool compare(Matrix4F a, Matrix4F b, float tolerance = DEFAULT_TOLERANCE)
        {
            if (Math.Abs(a.m1 - b.m1) > tolerance || Math.Abs(a.m2 - b.m2) > tolerance || Math.Abs(a.m3 - b.m3) > tolerance || Math.Abs(a.m4 - b.m4) > tolerance ||
                Math.Abs(a.m5 - b.m5) > tolerance || Math.Abs(a.m6 - b.m6) > tolerance || Math.Abs(a.m7 - b.m7) > tolerance || Math.Abs(a.m8 - b.m8) > tolerance ||
                Math.Abs(a.m9 - b.m9) > tolerance || Math.Abs(a.m10 - b.m10) > tolerance || Math.Abs(a.m11 - b.m11) > tolerance || Math.Abs(a.m12 - b.m12) > tolerance ||
                Math.Abs(a.m13 - b.m13) > tolerance || Math.Abs(a.m14 - b.m14) > tolerance || Math.Abs(a.m15 - b.m15) > tolerance || Math.Abs(a.m16 - b.m16) > tolerance)
                return false;
            return true;
        }

        [TestMethod]
        public void Vector3FAddition()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3b = new Vector3F(5, 3.99f, -12);
            Vector3F v3c = v3a + v3b;

            Assert.IsTrue(compare(new Vector3F(18.5f, -44.24f, 850), v3c));
        }

        [TestMethod]
        public void Vector4FAddition()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = new Vector4F(5, 3.99f, -12, 1);
            Vector4F v4c = v4a + v4b;

            Assert.IsTrue(compare(new Vector4F(18.5f, -44.24f, 850, 1), v4c));
        }

        [TestMethod]
        public void Vector3FSubtraction()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3b = new Vector3F(5, 3.99f, -12);
            Vector3F v3c = v3a - v3b;

            Assert.IsTrue(compare(new Vector3F(8.5f, -52.22f, 874), v3c));
        }

        [TestMethod]
        public void Vector4FSubtraction()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = new Vector4F(5, 3.99f, -12, 1);
            Vector4F v4c = v4a - v4b;

            Assert.IsTrue(compare(new Vector4F(8.5f, -52.22f, 874, -1), v4c));
        }

        [TestMethod]
        public void Vector3FPostScale()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3c = v3a * 0.256f;

            Assert.IsTrue(compare(new Vector3F(3.45600008965f, -12.3468809128f, 220.672012329f), v3c));
        }

        [TestMethod]
        public void Vector4FPostScale()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4c = v4a * 4.89f;

            Assert.IsTrue(compare(new Vector4F(66.0149993896f, -235.844696045f, 4215.1796875f, 0), v4c));
        }

        [TestMethod]
        public void Vector3FPreScale()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3c = 0.256f * v3a;

            Assert.IsTrue(compare(new Vector3F(3.45600008965f, -12.3468809128f, 220.672012329f), v3c));
        }

        [TestMethod]
        public void Vector4FPreScale()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4c = 4.89f * v4a;

            Assert.IsTrue(compare(new Vector4F(66.0149993896f, -235.844696045f, 4215.1796875f, 0), v4c));
        }

        [TestMethod]
        public void Vector3FDot()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3b = new Vector3F(5, 3.99f, -12);
            float dot3 = v3a.Dot(v3b);

            Assert.AreEqual(dot3, -10468.9375f, DEFAULT_TOLERANCE);
        }

        [TestMethod]
        public void Vector4FDot()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = new Vector4F(5, 3.99f, -12, 1);
            float dot4 = v4a.Dot(v4b);

            Assert.AreEqual(dot4, -10468.9375f, DEFAULT_TOLERANCE);
        }

        [TestMethod]
        public void Vector3FCross()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3b = new Vector3F(5, 3.99f, -12);
            Vector3F v3c = v3a.Cross(v3b);

            Assert.IsTrue(compare(v3c, new Vector3F(-2860.62011719f, 4472.00000000f, 295.01498413f)));
        }

        [TestMethod]
        public void Vector4FCross()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = new Vector4F(5, 3.99f, -12, 1);
            Vector4F v4c = v4a.Cross(v4b);

            Assert.IsTrue(compare(v4c, new Vector4F(-2860.62011719f, 4472.00000000f, 295.01498413f, 0)));
        }

        [TestMethod]
        public void Vector3FMagnitude()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            float mag3 = v3a.Magnitude();

            Assert.AreEqual(mag3, 863.453735352f, DEFAULT_TOLERANCE);
        }

        [TestMethod]
        public void Vector4FMagnitude()
        {
            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            float mag4 = v4a.Magnitude();

            Assert.AreEqual(mag4, 863.453735352f, DEFAULT_TOLERANCE);
        }

        [TestMethod]
        public void Vector3FNormalise()
        {
            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            v3a.Normalize();

            Assert.IsTrue(compare(v3a, new Vector3F(0.0156349f, -0.0558571f, 0.998316f)));
        }

        [TestMethod]
        public void Vector4FNormalise()
        {
            Vector4F v4a = new Vector4F(243, -48.23f, 862, 0);
            v4a.Normalize();

            Assert.IsTrue(compare(v4a, new Vector4F(0.270935f, -0.0537745f, 0.961094f, 0)));
        }

        [TestMethod]
        public void Matrix3FSetRotateX()
        {
            Matrix3F m3a = new Matrix3F();
            m3a.SetRotateX(3.98f);

            Assert.IsTrue(compare(m3a,
                new Matrix3F(1, 0, 0, 0, -0.668648f, -0.743579f, 0, 0.743579f, -0.668648f)));
        }

        [TestMethod]
        public void Matrix4FSetRotateX()
        {
            Matrix4F m4a = new Matrix4F();
            m4a.SetRotateX(4.5f);

            Assert.IsTrue(compare(m4a,
                new Matrix4F(1, 0, 0, 0, 0, -0.210796f, -0.97753f, 0, 0, 0.97753f, -0.210796f, 0, 0, 0, 0, 1)));
        }

        [TestMethod]
        public void Matrix3FSetRotateY()
        {
            Matrix3F m3b = new Matrix3F();
            m3b.SetRotateY(1.76f);

            Assert.IsTrue(compare(m3b,
                new Matrix3F(-0.188077f, 0, -0.982154f, 0, 1, 0, 0.982154f, 0, -0.188077f)));
        }

        [TestMethod]
        public void Matrix4FSetRotateY()
        {
            Matrix4F m4b = new Matrix4F();
            m4b.SetRotateY(-2.6f);

            Assert.IsTrue(compare(m4b,
                new Matrix4F(-0.856889f, 0, 0.515501f, 0, 0, 1, 0, 0, -0.515501f, 0, -0.856889f, 0, 0, 0, 0, 1)));
        }

        [TestMethod]
        public void Matrix3FSetRotateZ()
        {
            Matrix3F m3c = new Matrix3F();
            m3c.SetRotateZ(9.62f);

            Assert.IsTrue(compare(m3c,
                new Matrix3F(-0.981005f, -0.193984f, 0, 0.193984f, -0.981005f, 0, 0, 0, 1)));
        }

        [TestMethod]
        public void Matrix4FSetRotateZ()
        {
            Matrix4F m4c = new Matrix4F();
            m4c.SetRotateZ(0.72f);

            Assert.IsTrue(compare(m4c,
                new Matrix4F(0.751806f, 0.659385f, 0, 0, -0.659385f, 0.751806f, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)));
        }

        [TestMethod]
        public void Vector3FMatrixTransform()
        {
            Matrix3F m3b = new Matrix3F();
            m3b.SetRotateY(1.76f);

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3b = m3b * v3a;

            Assert.IsTrue(compare(v3b,
                new Vector3F(844.077941895f, -48.2299995422f, -175.38130188f)));
        }

        [TestMethod]
        public void Vector3FMatrixTransform2()
        {
            Matrix3F m3c = new Matrix3F();
            m3c.SetRotateZ(9.62f);

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 862);
            Vector3F v3c = m3c * v3a;

            Assert.IsTrue(compare(v3c,
                new Vector3F(-22.5994224548f, 44.6950683594f, 862)));
        }

        [TestMethod]
        public void Vector4FMatrixTransform()
        {
            Matrix4F m4b = new Matrix4F();
            m4b.SetRotateY(-2.6f);

            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = m4b * v4a;

            Assert.IsTrue(compare(v4b,
                new Vector4F(-455.930236816f, -48.2299995422f, -731.678771973f, 0)));
        }

        [TestMethod]
        public void Vector4FMatrixTransform2()
        {
            Matrix4F m4c = new Matrix4F();
            m4c.SetRotateZ(0.72f);

            Vector4F v4a = new Vector4F(13.5f, -48.23f, 862, 0);
            Vector4F v4b = m4c * v4a;

            Assert.IsTrue(compare(v4b,
                new Vector4F(41.951499939f, -27.3578968048f, 862, 0)));
        }

        [TestMethod]
        public void Matrix3FMultiply()
        {
            Matrix3F m3a = new Matrix3F();
            m3a.SetRotateX(3.98f);

            Matrix3F m3c = new Matrix3F();
            m3c.SetRotateZ(9.62f);

            Matrix3F m3d = m3a & m3c; // using custom row oder multiplication function for this unit test only

            Assert.IsTrue(compare(m3d,
                new Matrix3F(-0.981004655361f, 0.129707172513f, 0.14424264431f, 0.193984255195f, 0.655946731567f, 0.729454636574f, 0, 0.743579149246f, -0.668647944927f)));
        }

        [TestMethod]
        public void Matrix4FMultiply()
        {
            Matrix4F m4b = new Matrix4F();
            m4b.SetRotateY(-2.6f);

            Matrix4F m4c = new Matrix4F();
            m4c.SetRotateZ(0.72f);

            Matrix4F m4d = m4c & m4b;// using custom row oder multiplication function for this unit test only

            Assert.IsTrue(compare(m4d,
                new Matrix4F(-0.644213855267f, -0.565019249916f, 0.515501439571f, 0, -0.659384667873f, 0.751805722713f, 0, 0, -0.387556940317f, -0.339913755655f, -0.856888711452f, 0, 0, 0, 0, 1)));
        }

        [TestMethod]
        public void Vector3FMatrixTranslation()
        {
            // homogeneous point translation
            Matrix3F m3b = new Matrix3F(1, 0, 0,
                                      0, 1, 0,
                                      55, 44, 1);

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 1);

            Vector3F v3b = m3b * v3a;

            Assert.IsTrue(compare(v3b, new Vector3F(68.5f, -4.23f, 1)));
        }

        [TestMethod]
        public void Vector3FMatrixTranslation2()
        {
            // homogeneous point translation
            Matrix3F m3c = new Matrix3F();
            m3c.SetRotateZ(2.2f);
            m3c.m7 = 55; m3c.m8 = 44; m3c.m9 = 1;

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 1);

            Vector3F v3c = m3c * v3a;

            Assert.IsTrue(compare(v3c, new Vector3F(86.0490112305f, 83.2981109619f, 1)));
        }

        [TestMethod]
        public void Vector4FMatrixTranslation()
        {
            // homogeneous point translation
            Matrix4F m4b = new Matrix4F(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      55, 44, 99, 1);

            Vector4F v4a = new Vector4F(13.5f, -48.23f, -54, 1);

            Vector4F v4c = m4b * v4a;
            Assert.IsTrue(compare(v4c, new Vector4F(68.5f, -4.23f, 45, 1)));
        }

        [TestMethod]
        public void Vector4FMatrixTranslation2()
        {
            // homogeneous point translation
            Matrix4F m4c = new Matrix4F();
            m4c.SetRotateZ(2.2f);
            m4c.m13 = 55; m4c.m14 = 44; m4c.m15 = 99; m4c.m16 = 1;

            Vector4F v4a = new Vector4F(13.5f, -48.23f, -54, 1);

            Vector4F v4c = m4c * v4a;
            Assert.IsTrue(compare(v4c, new Vector4F(86.0490112305f, 83.2981109619f, 45, 1)));
        }

        [TestMethod]
        public void Vector3FMatrixTranslation3()
        {
            // homogeneous point translation
            Matrix3F m3b = new Matrix3F(1, 0, 0,
                                      0, 1, 0,
                                      55, 44, 1);

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 0);

            Vector3F v3b = m3b * v3a;

            Assert.IsTrue(compare(v3b, new Vector3F(13.5f, -48.23f, 0)));
        }

        [TestMethod]
        public void Vector3FMatrixTranslation4()
        {
            // homogeneous point translation
            Matrix3F m3c = new Matrix3F();
            m3c.SetRotateZ(2.2f);
            m3c.m7 = 55; m3c.m8 = 44; m3c.m9 = 1;

            Vector3F v3a = new Vector3F(13.5f, -48.23f, 0);

            Vector3F v3c = m3c * v3a;

            Assert.IsTrue(compare(v3c, new Vector3F(31.0490131378f, 39.2981109619f, 0)));
        }

        [TestMethod]
        public void Vector4FMatrixTranslation3()
        {
            // homogeneous point translation
            Matrix4F m4b = new Matrix4F(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      55, 44, 99, 1);

            Vector4F v4a = new Vector4F(13.5f, -48.23f, -54, 0);

            Vector4F v4c = m4b * v4a;
            Assert.IsTrue(compare(v4c, new Vector4F(13.5f, -48.23f, -54, 0)));
        }

        [TestMethod]
        public void Vector4FMatrixTranslation4()
        {
            // homogeneous point translation
            Matrix4F m4c = new Matrix4F();
            m4c.SetRotateZ(2.2f);
            m4c.m13 = 55; m4c.m14 = 44; m4c.m15 = 99; m4c.m16 = 1;

            Vector4F v4a = new Vector4F(13.5f, -48.23f, -54, 0);

            Vector4F v4c = m4c * v4a;
            Assert.IsTrue(compare(v4c, new Vector4F(31.0490131378f, 39.2981109619f, -54, 0)));
        }

        [TestMethod]
        public void ColourConstructor()
        {
            // homogeneous point translation
            Colour c = new Colour(0x12, 0x34, 0x56, 0x78);

            Assert.AreEqual<UInt32>(c.colour, 0x12345678);
        }

        [TestMethod]
        public void ColourGetRed()
        {
            // homogeneous point translation
            Colour c = new Colour(0x12, 0x34, 0x56, 0x78);

            Assert.AreEqual<byte>(c.GetRed(), 0x12);
        }

        [TestMethod]
        public void ColourGetGreen()
        {
            // homogeneous point translation
            Colour c = new Colour(0x12, 0x34, 0x56, 0x78);

            Assert.AreEqual<byte>(c.GetGreen(), 0x34);
        }


        [TestMethod]
        public void ColourGetBlue()
        {
            // homogeneous point translation
            Colour c = new Colour(0x12, 0x34, 0x56, 0x78);

            Assert.AreEqual<byte>(c.GetBlue(), 0x56);
        }

        [TestMethod]
        public void ColourGetAlpha()
        {
            // homogeneous point translation
            Colour c = new Colour(0x12, 0x34, 0x56, 0x78);

            Assert.AreEqual<byte>(c.GetAlpha(), 0x78);
        }

        [TestMethod]
        public void ColourSetRed()
        {
            // homogeneous point translation
            Colour c = new Colour();
            c.SetRed(0x12);

            Assert.AreEqual<UInt32>(c.colour, 0x12000000);
        }

        [TestMethod]
        public void ColourSetGreen()
        {
            // homogeneous point translation
            Colour c = new Colour();
            c.SetGreen(0x34);

            Assert.AreEqual<UInt32>(c.colour, 0x00340000);
        }

        [TestMethod]
        public void ColourSetBlue()
        {
            // homogeneous point translation
            Colour c = new Colour();
            c.SetBlue(0x56);

            Assert.AreEqual<UInt32>(c.colour, 0x00005600);
        }

        [TestMethod]
        public void ColourSetAlpha()
        {
            // homogeneous point translation
            Colour c = new Colour();
            c.SetAlpha(0x78);

            Assert.AreEqual<UInt32>(c.colour, 0x00000078);
        }
    }
}