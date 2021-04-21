using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public struct Plane
    {
        public Vector3 normal;
        public float scalar;

        public Plane(Vector3 normal, float scalar)
        {
            this.normal = Vector3.Normalize(normal);
            this.scalar = scalar;
        }

        public static float vectorDistanceFromPlane(Plane plane, Vector3 vec)
        {
            return Vector3.Dot(vec, plane.normal) - plane.scalar;
        }
        
        public static bool isBoxInfrontOfPlane(AABB box, Plane plane)
        {
            float r = Vector3.Dot(box.extents, MathUtil.abs(plane.normal));
            return vectorDistanceFromPlane(plane, box.pos) >= -r;
        }

        public static bool isBoxBehindPlane(AABB box, Plane plane)
        {
            float r = Vector3.Dot(box.extents, MathUtil.abs(plane.normal));
            return vectorDistanceFromPlane(plane, box.pos) < -r;
        }
    }
}