using Coictus.FredsMath;

namespace Coictus
{
    public struct PlaneCollider : ICollider
    {
        public Vector3D normal;
        public double scalar;

        public PlaneCollider(Vector3D normal, double scalar)
        {
            this.normal = Vector3D.normalize(normal);
            this.scalar = scalar;
        }

        public static double vectorDistanceFromPlane(PlaneCollider plane, Vector3D vec)
        {
            return Vector3D.dot(vec, plane.normal) - plane.scalar;
        }
        public bool getHasParent()
        {
            return false;
        }

        public int getCollisionWeight()
        {
            return int.MaxValue;
        }

        public PositionalObject getParent()
        {
            return null;
        }
        public ICollider getNextTickPredictedHitbox()
        {
            return this;
        }

        public void onTick()
        {
         
        }
        public ColliderType getType()
        {
            return ColliderType.plane;
        }
    }
}
