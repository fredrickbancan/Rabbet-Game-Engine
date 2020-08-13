using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct PlaneCollider : ICollider
    {
        Vector3D normal;
        double scalar;

        public PlaneCollider(Vector3D normal, double scalar)
        {
            this.normal = Vector3D.normalize(normal);
            this.scalar = scalar;
        }

        public CollisionDirection getCollisionResultAABB(AABBCollider boxToTest, out bool touching)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultPoint(Vector3D pointToTest, out bool touching)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching)
        {
            throw new System.NotImplementedException();
        }

        public bool getHasParent()
        {
            return false;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            return this;
        }

        public void onTick()
        {
         
        }
    }
}
