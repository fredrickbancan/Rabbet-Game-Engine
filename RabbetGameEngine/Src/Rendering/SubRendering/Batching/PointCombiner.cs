using RabbetGameEngine.Models;

namespace RabbetGameEngine.SubRendering
{
    public static class PointCombiner
    {
        public static PointParticle[] addPointsToArray(PointParticle[] points, PointParticle[] newPoints)
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

        public static PointParticle[] addPointToArray(PointParticle[] points, PointParticle newPoint)
        {
            PointParticle[] result = new PointParticle[points.Length + 1];
            for (int i = 0; i < points.Length; ++i)
            {
                result[i] = points[i];
            }
            result[points.Length] = newPoint;
            return result;
        }

        public static PointCloudModel combineData(PointCloudModel inputPointCloud, PointCloudModel newPointCloud)
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
