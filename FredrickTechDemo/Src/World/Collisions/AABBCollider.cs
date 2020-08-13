using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABBCollider : ICollider
    {
        Entity parent;
        Vector3D minBounds;
        Vector3D maxBounds;

        public AABBCollider(Vector3D minBounds, Vector3D maxBounds, Entity parent = null)
        {
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            this.parent = parent;
        }

        public AABBCollider setBounds(Vector3D minBounds, Vector3D maxBounds)
        {
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return true;
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

        public void onTick()
        {
            throw new System.NotImplementedException();
        }

        public bool getHasParent()
        {
            throw new System.NotImplementedException();
        }
    }
}
