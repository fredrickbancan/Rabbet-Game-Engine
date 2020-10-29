using RabbetGameEngine.Models;

namespace RabbetGameEngine.SubRendering
{
    public static class PointCombiner
    {

        public static PointParticle[] interlacePointArraysByCount(PointParticle[] arrayA, PointParticle[] arrayB, int count)
        {
            int combinedLength = count*2;

            PointParticle[] result = new PointParticle[combinedLength];

            //interlace
            for (int i = 0; i < combinedLength; i += 2)
            {
                result[i] = arrayA[i / 2];
                result[i + 1] = arrayB[i / 2];
            }

            return result;
        }

        public static PointParticle[] interlacePointArraysOfSameLength(PointParticle[] arrayA, PointParticle[] arrayB)
        {
            int combinedLength = arrayA.Length + arrayB.Length;

            PointParticle[] result = new PointParticle[combinedLength];

            //interlace
            for (int i = 0; i < combinedLength; i += 2)
            {
                result[i] = arrayA[i / 2];
                result[i + 1] = arrayB[i / 2];
            }

            return result;
        }

        public static PointParticle[] concatPoints(PointParticle[] points, PointParticle[] newPoints)
        {
            int l = points.Length + newPoints.Length;
            PointParticle[] result = new PointParticle[l];
            for (int i = 0; i < points.Length; ++i)
            {
                result[i] = points[i];
            }
            for (int i = points.Length; i < l; ++i)
            {
                result[i] = newPoints[i - points.Length];
            }
            return result;
        }

        public static PointParticle[] concatPoint(PointParticle[] points, PointParticle newPoint)
        {
            PointParticle[] result = new PointParticle[points.Length + 1];
            for (int i = 0; i < points.Length; ++i)
            {
                result[i] = points[i];
            }
            result[points.Length] = newPoint;
            return result;
        }

        public static PointCloudModel concatData(PointCloudModel inputPointCloud, PointCloudModel newPointCloud)
        {
            int totalPoints = inputPointCloud.points.Length + newPointCloud.points.Length;

            PointParticle[] resultPoints = new PointParticle[totalPoints];
            PointParticle[] resultPrevPoints = new PointParticle[totalPoints];

            //fill resultpoints
            for (int i = 0; i < inputPointCloud.points.Length; i++)
            {
                resultPoints[i] = inputPointCloud.points[i];
            }
            for (int i = inputPointCloud.points.Length; i < totalPoints; i++)
            {
                resultPoints[inputPointCloud.points.Length + i] = newPointCloud.points[i];
            }

            //fill resultPrevPoints
            for (int i = 0; i < inputPointCloud.prevPoints.Length; i++)
            {
                resultPrevPoints[i] = inputPointCloud.prevPoints[i];
            }
            for (int i = inputPointCloud.prevPoints.Length; i < totalPoints; i++)
            {
                resultPrevPoints[inputPointCloud.prevPoints.Length + i] = newPointCloud.prevPoints[i];
            }
            inputPointCloud.points = resultPoints;
            inputPointCloud.prevPoints = resultPrevPoints;
            return inputPointCloud;
        }
    }
}
