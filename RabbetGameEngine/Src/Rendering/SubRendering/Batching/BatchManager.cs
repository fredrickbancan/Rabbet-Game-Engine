using System.Collections.Generic;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType
    {
        triangles,
        trianglesTransparent,
        lines,
        lerpTrianglesTransparent,
        lerpTriangles,
        lerpPointCloud,
        lerpPointCloudTransparent,
        lerpSinglePoint,
        lerpSinglePointTransparent,
        lerpLines
    };

    /*Class for constructing*/
    public static class BatchManager
    {
        
        private static List<Batch> batches = new List<Batch>();

        //TODO: impliment all required functions
    }
}
