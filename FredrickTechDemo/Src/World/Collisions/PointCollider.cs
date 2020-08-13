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
        public CollisionDirection getCollisionResultAABB(AABBCollider boxToTest, out bool touching)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultPoint(Vector3D pointToTest, out bool touching)
        {
            touching = false;
            return CollisionDirection.none;//points cannot collide with points period
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching)
        {
            return CollisionDirection.none;//points cannot collide with spheres with direction in an axis aligned only context
        }

        public bool getHasParent()
        {
            return parent != null;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            throw new System.NotImplementedException();
        }

        public void onTick()
        {
            throw new System.NotImplementedException();
        }
    }
}
