using OpenTK;
namespace RabbetGameEngine
{
    public struct PlaneCollider : ICollider
    {
        public Vector3 normal;
        public float scalar;

        public PlaneCollider(Vector3 normal, float scalar)
        {
            this.normal = Vector3.Normalize(normal);
            this.scalar = scalar;
        }

        public static float vectorDistanceFromPlane(PlaneCollider plane, Vector3 vec)
        {
            return Vector3.Dot(vec, plane.normal) - plane.scalar;
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
