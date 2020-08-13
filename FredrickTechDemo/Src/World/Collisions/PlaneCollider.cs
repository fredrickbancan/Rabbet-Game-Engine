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

        public CollisionDirection getCollisionResultAABB(AABBCollider boxToTest)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultPoint(Vector3D pointToTest)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest)
        {
            throw new System.NotImplementedException();
        }

        public bool getHasParent()
        {
            throw new System.NotImplementedException();
        }

        public void onTick()
        {
         
        }
    }
}
