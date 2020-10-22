namespace RabbetGameEngine.SubRendering
{
    public static class PointCombiner
    {
        public static Vertex[] addPointsToArray(Vertex[] points, Vertex[] newPoints)
        {
            int l = points.Length + newPoints.Length;
            Vertex[] result = new Vertex[l];
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

        public static Vertex[] addPointToArray(Vertex[] points, Vertex newPoint)
        {
            Vertex[] result = new Vertex[points.Length + 1];
            for (int i = 0; i < points.Length; ++i)
            {
                result[i] = points[i];
            }
            result[points.Length] = newPoint;
            return result;
        }
    }
}
