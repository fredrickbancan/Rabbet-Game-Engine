using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct SphereCollider : ICollider
    {
        Entity parent;
        Vector3D pos;
        double radius;

        public SphereCollider(Vector3D pos, double radius, Entity parent = null)
        {
            this.pos = pos;
            this.radius = radius;
            this.parent = parent;
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
            throw new System.NotImplementedException();
        }
    }
}
