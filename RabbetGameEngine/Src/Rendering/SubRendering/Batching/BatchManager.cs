using System.Collections.Generic;

namespace RabbetGameEngine.SubRendering
{
    public enum BatchType
    {
        triangles,
        trianglesTransparent,
        quads,
        quadsTransparent,
        lines,
        lerpTrianglesTransparent,
        lerpTriangles,
        lerpQuads,
        lerpQuadsTransparent,
        lerpVertices,
        lerpVerticesTransparent,
        lerpPoints,
        lerpPointsTransparent,
        lerpSinglePoints,
        lerpSinglePointsTransparent,
        lerpLines
    };

    /*Class for constructing*/
    public static class BatchManager
    {
        
        private static List<Batch> batches = new List<Batch>();

        //TODO: impliment
    }
}
