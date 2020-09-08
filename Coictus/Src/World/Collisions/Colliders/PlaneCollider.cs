using OpenTK;
namespace Coictus
{
    public struct PlaneCollider : ICollider
    {
        public Vector3d normal;
        public double scalar;

        public PlaneCollider(Vector3d normal, double scalar)
        {
            this.normal = Vector3d.Normalize(normal);
            this.scalar = scalar;
        }

        public static double vectorDistanceFromPlane(PlaneCollider plane, Vector3d vec)
        {
            return Vector3d.Dot(vec, plane.normal) - plane.scalar;
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
