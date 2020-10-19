using OpenTK;
namespace RabbetGameEngine.Physics
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
         void setParent(PositionalObject parent)
        {
            
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

        public Vector3 getCenterVec()
        {
            return Vector3.Zero;
        }

        public void offset(Vector3 vec)
        {
            normal += vec;
            normal.Normalize();
        }

        public ColliderType getType()
        {
            return ColliderType.plane;
        }
    }
}
