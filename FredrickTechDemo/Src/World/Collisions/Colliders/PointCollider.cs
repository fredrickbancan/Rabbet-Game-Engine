using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct PointCollider : ICollider
    {
        public Vector3D pointPos;
        public Entity parent;

        public PointCollider(Vector3D pos, Entity parent = null)
        {
            pointPos = pos;
            this.parent = parent;
        }
        public CollisionDirection getCollisionResultForColliderType(ICollider colliderToTest, out bool touching, out double overlap)
        {
            if (colliderToTest is AABBCollider aabb)
            {
                return this.getCollisionResultAABB(aabb, out touching, out overlap);
            }

            if (colliderToTest is PointCollider point)
            {
                return this.getCollisionResultPoint(point.pointPos, out touching, out overlap);
            }

            if (colliderToTest is SphereCollider sphere)
            {
                return this.getCollisionResultSphere(sphere, out touching, out overlap);
            }

            touching = false;
            overlap = 0;
            return CollisionDirection.none;
        }
        public CollisionDirection getCollisionResultAABB(AABBCollider boxToTest, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
            return CollisionDirection.none;
        }

        public CollisionDirection getCollisionResultPoint(Vector3D pointToTest, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
            return CollisionDirection.none;//points cannot collide with points period
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
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
