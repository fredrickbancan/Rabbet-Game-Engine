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

        public static double distanceFromPlane(PlaneCollider plane, Vector3D point)
        {
            return point.Dot(plane.normal) + plane.scalar;
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
            double distance = distanceFromPlane(this, pointToTest);

            if (distance <= 0)//less than 0 means the point is beneath the plane
            {
                overlap = 0;
                touching = true;
            }
            else
            {
                overlap = 0;
                touching = false;
                return CollisionDirection.none;
            }
            overlap = 0;
            return CollisionDirection.none;
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
            return CollisionDirection.none;
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
