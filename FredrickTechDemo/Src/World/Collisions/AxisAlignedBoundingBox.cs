using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*A struct for doing basic collision detection*/
    public struct AxisAlignedBoundingBox
    {
        Vector3D minBounds;
        Vector3D maxBounds;

        public AxisAlignedBoundingBox(Vector3D minBounds, Vector3D maxBounds)
        {
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
        }

        public AxisAlignedBoundingBox setBounds(Vector3D minBounds, Vector3D maxBounds)
        {
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            return this;
        }

        /*returns true if two bounding boxes are touching in any way*/
        public static bool areBoxesTouching(AxisAlignedBoundingBox boxA, AxisAlignedBoundingBox boxB)
        {
            return false;
        }
    }
}
