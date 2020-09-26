using System.Collections.Generic;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType
    {
        triangles,
        trianglesTransparent,
        lerpTriangles,
        lerpTrianglesTransparent,
        quads,
        quadsTransparent,
        lerpQuads,
        lerpQuadsTransparent,
        lerpVertices,
        lerpVerticesTransparent,
        lerpPoints,
        lerpPointsTransparent,
        lerpSinglePoints,
        lerpSinglePointsTransparent,
        lines,
        lerpLines
    };

    /*Class for constructing*/
    public static class BatchManager
    {
        
        private static List<Batch> batches = new List<Batch>();

        //TODO: impliment
    }
}
