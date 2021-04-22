using System;

namespace RabbetGameEngine
{
    public static class BatchUtil
    {
        public static void init()
        {
        }

        public static Batch getBatchForType(RenderType type, int renderLayer = 0, bool isFrameRenderUpdate = false)
        {
            switch (type)
            {
                case RenderType.guiCutout:
                    return new BatchGuiCutOut(renderLayer).setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.guiLines:
                    return new BatchGuiLines().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.guiText:
                    return new BatchGuiText().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.guiTransparent:
                    return new BatchGuiTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.iSpheresTransparent:
                    return new BatchISpheresTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.trianglesTransparent:
                    return new BatchTrianglesTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.quadsTransparent:
                    return new BatchQuadsTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpISpheresTransparent:
                    return new BatchLerpISpheresTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpTrianglesTransparent:
                    return new BatchLerpTrianglesTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpQuadsTransparent:
                    return new BatchLerpQuadsTransparent().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpText3D:
                    return new BatchLerpText3D().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpISpheres:
                    return new BatchLerpISpheres().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpTriangles:
                    return new BatchLerpTriangles().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpQuads:
                    return new BatchLerpQuads().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lerpLines:
                    return new BatchLerpLines().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.text3D:
                    return new BatchText3D().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.triangles:
                    return new BatchTriangles().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.quads:
                    return new BatchQuads().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.lines:
                    return new BatchLines().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.iSpheres:
                    return new BatchISpheres().setUpdateEachFrame(isFrameRenderUpdate);
                case RenderType.spriteCylinder:
                    return new BatchSpriteCylinder().setUpdateEachFrame(isFrameRenderUpdate);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns true if the dest indices array needs to be resized and is successfully resized to accomidate the number of total vertices for a quad based model based on the number of vertices of the model provided, and the maximum array size provided.
        /// </summary>
        /// <param name="dstIndices">Destination array to be tested and resized</param>
        /// <param name="totalVertices">Number of vertices to be accomidated</param>
        /// <param name="maxDstSize">Maximum allowed size for the array</param>
        public static bool canResizeQuadIndicesIfNeeded(ref uint[] dstIndices, int totalVertices, int maxDstSize)
        {
            int n;
            if ((n = totalVertices + (totalVertices / 2)) >= maxDstSize)
            {
                return false;
            }
            if (n >= dstIndices.Length)
            {
                if ((n *= 2) >= maxDstSize)
                {
                    dstIndices = QuadCombiner.getIndicesForQuadCount(maxDstSize / 6);
                }
                else
                {
                    dstIndices = QuadCombiner.getIndicesForQuadCount(n / 6);
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if an array of length srcSize can fit into the dstArray, or if the dstArray has been resized and can now accept it.
        /// If adding the srcSize goes over maxDstSize, then returns false.
        /// When resizing, simply doubles current dst array size, or sets dst array size to maxDstSize.
        /// int dstFilled is how much of the dstArray is used.
        /// </summary>
        public static bool canFitOrResize<T2>(ref T2[] dstArr, int srcSize, int dstFilled, int maxDstSize) where T2 : struct
        {
            int n;
            if ((n = dstFilled + srcSize) >= maxDstSize) return false;
            if (n >= dstArr.Length)
            {
                if ((n *= 2) >= maxDstSize)
                {
                    Array.Resize<T2>(ref dstArr, maxDstSize);
                }
                else
                {
                    Array.Resize<T2>(ref dstArr, n);
                }
            }
            return true;
        }
    }
}
