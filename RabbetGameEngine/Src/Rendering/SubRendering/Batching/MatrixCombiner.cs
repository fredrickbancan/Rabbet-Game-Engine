using OpenTK;

namespace RabbetGameEngine.SubRendering
{
    public static class MatrixCombiner
    {

        public static Matrix4[] interlaceMatrixArraysByCount(Matrix4[] arrayA, Matrix4[] arrayB, int count)
        {
            int combinedLength = count*2;

            Matrix4[] result = new Matrix4[combinedLength];

            //interlacing
            for (int i = 0; i < combinedLength; i+= 2)
            {
                result[i] = arrayA[i / 2];
                result[i + 1] = arrayB[i / 2];
            }

            return result;
        }
        public static Matrix4[] interlaceMatrixArraysOfSameLength(Matrix4[] arrayA, Matrix4[] arrayB)
        {
            int combinedLength = arrayA.Length + arrayB.Length;

            Matrix4[] result = new Matrix4[combinedLength];

            //interlacing
            for (int i = 0; i < combinedLength; i += 2)
            {
                result[i] = arrayA[i / 2];
                result[i + 1] = arrayB[i / 2];
            }

            return result;
        }
    }
}
