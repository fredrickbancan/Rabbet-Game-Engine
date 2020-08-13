using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    struct PointCollider : ICollider
    {
        Vector3D pointPos;
        Entity parent;

        public PointCollider(Vector3D pos, Entity parent = null)
        {
            pointPos = pos;
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
