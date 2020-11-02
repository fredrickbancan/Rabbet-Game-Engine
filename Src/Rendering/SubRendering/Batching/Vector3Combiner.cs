using OpenTK.Mathematics;

namespace RabbetGameEngine.SubRendering
{
    public static class VectorCombiner
    {

        public static Vector3[] interlaceVector3ArraysByCount(Vector3[] a, Vector3[] b, int count)
        {
            int combinedLength = count * 2;

            Vector3[] result = new Vector3[combinedLength];

            //interlacing
            for (int i = 0; i < combinedLength; i += 2)
            {
                result[i] = a[i / 2];
                result[i + 1] = b[i / 2];
            }

            return result;
        }
    }
}
