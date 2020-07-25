using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredsMath
{
    public class Matrix4F
    {
        public float m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m14, m15, m16;

        //constructors
        public Matrix4F() //this setup for matrices is WRONG in opengl (this is row order)
        {
            this.m1 = 1; this.m2 = 0; this.m3 = 0; this.m4 = 0;
            this.m5 = 0; this.m6 = 1; this.m7 = 0; this.m8 = 0;
            this.m9 = 0; this.m10= 0; this.m11= 1; this.m12= 0;
            this.m13= 0; this.m14= 0; this.m15= 0; this.m16= 1;
        }
        public Matrix4F(float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float m14, float m15, float m16)
        {
            this.m1 = m1; this.m2 = m2; this.m3 = m3; this.m4 = m4;
            this.m5 = m5; this.m6 = m6; this.m7 = m7; this.m8 = m8;
            this.m9 = m9; this.m10= m10; this.m11= m11; this.m12= m12;
            this.m13= m13; this.m14= m14; this.m15= m15; this.m16= m16;
        }

        //matrix matrix operators
        public static Matrix4F operator * (Matrix4F matA, Matrix4F matB) // 4 columns of A for each row of B TREATING MATRICES IN NEED OF TRANSPOSING
        {
            return new Matrix4F(
 /*m1*/matA.m1 * matB.m1 + matA.m5 * matB.m2 + matA.m9 * matB.m3 + matA.m13 * matB.m4,  /*m2*/matA.m2 * matB.m1 + matA.m6 * matB.m2 + matA.m10 * matB.m3 + matA.m14 * matB.m4,  /*m3*/matA.m3 * matB.m1 + matA.m7 * matB.m2 + matA.m11 * matB.m3 + matA.m15 * matB.m4,  /*m4*/matA.m4 * matB.m1 + matA.m8 * matB.m2 + matA.m12 * matB.m3 + matA.m16 * matB.m4,
 /*m5*/matA.m1 * matB.m5 + matA.m5 * matB.m6 + matA.m9 * matB.m7 + matA.m13 * matB.m8,  /*m6*/matA.m2 * matB.m5 + matA.m6 * matB.m6 + matA.m10 * matB.m7 + matA.m14 * matB.m8,  /*m7*/matA.m3 * matB.m5 + matA.m7 * matB.m6 + matA.m11 * matB.m7 + matA.m15 * matB.m8,  /*m8*/matA.m4 * matB.m5 + matA.m8 * matB.m6 + matA.m12 * matB.m7 + matA.m16 * matB.m8,
 /*m9*/matA.m1 * matB.m9 + matA.m5 * matB.m10+ matA.m9 * matB.m11+ matA.m13 * matB.m12,/*m10*/matA.m2 * matB.m9 + matA.m6 * matB.m10+ matA.m10 * matB.m11+ matA.m14 * matB.m12,/*m11*/matA.m3 * matB.m9 + matA.m7 * matB.m10+ matA.m11 * matB.m11+ matA.m15 * matB.m12,/*m12*/matA.m4 * matB.m9 + matA.m8 * matB.m10+ matA.m12 * matB.m11+ matA.m16 * matB.m12,
/*m13*/matA.m1 * matB.m13+ matA.m5 * matB.m14+ matA.m9 * matB.m15+ matA.m13 * matB.m16,/*m14*/matA.m2 * matB.m13+ matA.m6 * matB.m14+ matA.m10 * matB.m15+ matA.m14 * matB.m16,/*m15*/matA.m3 * matB.m13+ matA.m7 * matB.m14+ matA.m11 * matB.m15+ matA.m15 * matB.m16,/*m16*/matA.m4 * matB.m13+ matA.m8 * matB.m14+ matA.m12 * matB.m15+ matA.m16 * matB.m16);
        }
        //functions
        public void SetRotateX(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads); 

            /*this.m1 = 1; this.m2 = 0; this.m3 = 0; this.m4 = 0;*/
            /*this.m5 = 0;*/ this.m6 = cos; this.m7 = sin; /*this.m8 = 0;*/
            /*this.m9 = 0;*/ this.m10 = -sin; this.m11 = cos;/* this.m12 = 0;*/
            /*this.m13 = 0; this.m14 = 0; this.m15 = 0; this.m16 = 1;*/
        }
        public void SetRotateY(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads);

            this.m1 = cos; /*this.m2 = 0;*/ this.m3 = -sin;/* this.m4 = 0;*/
            /*this.m5 = 0; this.m6 = 1; this.m7 = 0; this.m8 = 0;*/
            this.m9 = sin; /*this.m10 = 0;*/ this.m11 = cos; /*this.m12 = 0;*/
           /* this.m13 = 0; this.m14 = 0; this.m15 = 0; this.m16 = 1;*/
        }

        public void SetRotateZ(float rads)
        {
            float cos = (float)Math.Cos(rads);
            float sin = (float)Math.Sin(rads);

            this.m1 = cos; this.m2 = sin; /*this.m3 = 0; this.m4 = 0;*/
            this.m5 = -sin; this.m6 = cos;/* this.m7 = 0; this.m8 = 0;*/
            /*this.m9 = 0; this.m10 = 0; this.m11 = 1; this.m12 = 0;
            this.m13 = 0; this.m14 = 0; this.m15 = 0; this.m16 = 1;*/
        }
    }
}
