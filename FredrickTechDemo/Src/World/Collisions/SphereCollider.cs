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

        public CollisionDirection getCollisionResultAABB(AABBCollider boxToTest, out bool touching)
        {
            throw new System.NotImplementedException();
        }

        public CollisionDirection getCollisionResultPoint(Vector3D pointToTest, out bool touching)
        {
            return CollisionDirection.none;//points can not collide with spheres with direction in an axis aligned only context
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphereToTest, out bool touching)
        {
            return CollisionDirection.none;//spheres can not collide with spheres with direction in an axis aligned only context
        }

        public bool getHasParent()
        {
            return parent != null;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            SphereCollider result = new SphereCollider(pos, radius, parent);
            if (parent != null)
            {
                result.pos = parent.getPredictedNextTickPos();
            }
            return result;
        }

        public void onTick()
        {
            if (parent != null)
            {
                pos = parent.getPosition();
            }
        }
    }
}
