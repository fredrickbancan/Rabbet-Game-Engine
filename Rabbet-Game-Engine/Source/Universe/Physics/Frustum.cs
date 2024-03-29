﻿using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    //Thank you to github user some rand for this frustum code sample
    /*frustum in world space
    0                                               1
    (1,1,1)-----------------------------------------(-1,1,1)
    |                                               |
    |           4                       5           |
    |       (1,1,-1)----------------(-1,1,-1)       |
    |           |                       |           |
    |           |                       |           |
    |           |                       |           |
    |           |                       |           |
    |           |                       |           |
    |           |                       |           |
    |       (1,-1,-1)----------------(-1,-1,-1)     |
    |           7                       6           |
    |                                               |
    |                                               |
    (1,-1,1)----------------------------------------(-1,-1,1)
    3                                               2
    */

    public struct WorldFrustum
    {
        private static readonly Vector4 nearCornerTopRightClip = new Vector4(-1,1,-1,1);
        private static readonly Vector4 nearCornerTopLeftClip = new Vector4(1,1,-1,1);
        private static readonly Vector4 nearCornerBottomRightClip = new Vector4(-1,-1,-1,1);
        private static readonly Vector4 nearCornerBottomLeftClip = new Vector4(1,-1,-1,1);
                                      
        private static readonly Vector4 farCornerTopRightClip = new Vector4(-1,1,1,1);
        private static readonly Vector4 farCornerTopLeftClip = new Vector4(1,1,1,1);
        private static readonly Vector4 farCornerBottomRightClip = new Vector4(-1,-1,1,1);
        private static readonly Vector4 farCornerBottomLeftClip = new Vector4(1,-1,1,1);

        //all planes point inwards
        public Plane nearPlane;
        public Plane farPlane;
        public Plane leftPlane;
        public Plane rightPlane;
        public Plane topPlane;
        public Plane bottomPlane;

        Vector3 nearCornerTopRightWorld;
        Vector3 nearCornerTopLeftWorld;
        Vector3 nearCornerBottomRightWorld;
        Vector3 nearCornerBottomLeftWorld;

        Vector3 farCornerTopRightWorld;
        Vector3 farCornerTopLeftWorld;
        Vector3 farCornerBottomRightWorld;
        Vector3 farCornerBottomLeftWorld;

        public void transformPlanes(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 camPos)
        {
            Matrix4 clipToWorld = Matrix4.Transpose(Matrix4.Invert(viewMatrix)) * Matrix4.Transpose(Matrix4.Invert(projectionMatrix));

            Vector4 nearCornerTopRightClipWP = clipToWorld * nearCornerTopRightClip;
            Vector4 nearCornerTopLeftClipWP = clipToWorld * nearCornerTopLeftClip;
            Vector4 nearCornerBottomRightClipWP = clipToWorld * nearCornerBottomRightClip;
            Vector4 nearCornerBottomLeftClipWP = clipToWorld * nearCornerBottomLeftClip;

            Vector4 farCornerTopRightClipWP = clipToWorld * farCornerTopRightClip;
            Vector4 farCornerTopLeftClipWP = clipToWorld * farCornerTopLeftClip;
            Vector4 farCornerBottomRightClipWP = clipToWorld * farCornerBottomRightClip;
            Vector4 farCornerBottomLeftClipWP = clipToWorld * farCornerBottomLeftClip;

            nearCornerTopRightWorld = nearCornerTopRightClipWP.Xyz * 1.0F / nearCornerTopRightClipWP.W + camPos;
            nearCornerTopLeftWorld = nearCornerTopLeftClipWP.Xyz * 1.0F / nearCornerTopLeftClipWP.W + camPos;
            nearCornerBottomRightWorld = nearCornerBottomRightClipWP.Xyz * 1.0F / nearCornerBottomRightClipWP.W + camPos;
            nearCornerBottomLeftWorld = nearCornerBottomLeftClipWP.Xyz * 1.0F / nearCornerBottomLeftClipWP.W + camPos;

            farCornerTopRightWorld = farCornerTopRightClipWP.Xyz * 1.0F / farCornerTopRightClipWP.W + camPos;
            farCornerTopLeftWorld = farCornerTopLeftClipWP.Xyz * 1.0F / farCornerTopLeftClipWP.W + camPos;
            farCornerBottomRightWorld = farCornerBottomRightClipWP.Xyz * 1.0F / farCornerBottomRightClipWP.W + camPos;
            farCornerBottomLeftWorld = farCornerBottomLeftClipWP.Xyz * 1.0F / farCornerBottomLeftClipWP.W + camPos;

            Vector3 n, a, b;

            //update near
            a = nearCornerBottomRightWorld - nearCornerBottomLeftWorld;
            b = nearCornerTopLeftWorld - nearCornerBottomLeftWorld;
            n = Vector3.Cross(a, b).Normalized();
            nearPlane.normal = n;
            nearPlane.scalar = Vector3.Dot(n, nearCornerBottomLeftWorld);

            //update far
            a = farCornerBottomLeftWorld - farCornerBottomRightWorld;
            b = farCornerTopRightWorld - farCornerBottomRightWorld;
            n = Vector3.Cross(a, b).Normalized();
            farPlane.normal = n;
            farPlane.scalar = Vector3.Dot(n, farCornerBottomRightWorld);

            //update left
            a = nearCornerBottomLeftWorld - farCornerBottomLeftWorld;
            b = farCornerTopLeftWorld - farCornerBottomLeftWorld;
            n = Vector3.Cross(a, b).Normalized();
            leftPlane.normal = n;
            leftPlane.scalar = Vector3.Dot(n, farCornerBottomLeftWorld);

            //update right
            a = farCornerBottomRightWorld - nearCornerBottomRightWorld;
            b = nearCornerTopRightWorld - nearCornerBottomRightWorld;
            n = Vector3.Cross(a, b).Normalized();
            rightPlane.normal = n;
            rightPlane.scalar = Vector3.Dot(n, nearCornerBottomRightWorld);

            //update top
            a = nearCornerTopLeftWorld - farCornerTopLeftWorld;
            b = farCornerTopRightWorld - farCornerTopLeftWorld;
            n = Vector3.Cross(a, b).Normalized();
            topPlane.normal = n;
            topPlane.scalar = Vector3.Dot(n, farCornerTopLeftWorld);

            //update bottom
            a = nearCornerBottomRightWorld - farCornerBottomRightWorld;
            b = farCornerBottomLeftWorld - farCornerBottomRightWorld;
            n = Vector3.Cross(a, b).Normalized();
            bottomPlane.normal = n;
            bottomPlane.scalar = Vector3.Dot(n, farCornerBottomRightWorld);
        }


        public static bool isBoxNotWithinFrustumRef(in WorldFrustum f,in AABB box)
        {
            return
               Plane.isBoxBehindPlaneRef(in box, in f.farPlane) ||
               Plane.isBoxBehindPlaneRef(in box, in f.topPlane) ||
               Plane.isBoxBehindPlaneRef(in box, in f.rightPlane) ||
               Plane.isBoxBehindPlaneRef(in box, in f.leftPlane) ||
               Plane.isBoxBehindPlaneRef(in box, in f.bottomPlane) ||
               Plane.isBoxBehindPlaneRef(in box, in f.nearPlane);
        }

        public static bool isBoxNotWithinFrustum(WorldFrustum f, AABB box)
        {
            return
               Plane.isBoxBehindPlane(box, f.farPlane) ||
               Plane.isBoxBehindPlane(box, f.topPlane) ||
               Plane.isBoxBehindPlane(box, f.rightPlane) ||
               Plane.isBoxBehindPlane(box, f.leftPlane) ||
               Plane.isBoxBehindPlane(box, f.bottomPlane) ||
               Plane.isBoxBehindPlane(box, f.nearPlane);
        }

    }
}