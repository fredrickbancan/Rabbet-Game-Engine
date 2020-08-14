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
            this.minBounds = Vector3D.zero;
            this.maxBounds = Vector3D.zero;
            this.parent = parent;
            setBounds(minBounds, maxBounds, parent);
        }

        /*sets the bounds for this aabb. if the parent entity is provided and is not null, the bounds will be relative to the parent entity as a center origin
          otherwise the bounds will be absolutely world space orientated.*/
        public AABBCollider setBounds(Vector3D minBounds, Vector3D maxBounds, Entity parent = null)
        {
            
            if(parent != null)
            {
                this.minBounds = parent.getPosition() + minBounds;
                this.maxBounds = parent.getPosition() + maxBounds;
            }
            else
            {
                this.minBounds = minBounds;
                this.maxBounds = maxBounds;
            }
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        public CollisionDirection getCollisionResultForColliderType(ICollider colliderToTest, out bool touching, out double overlap)
        {
            if(colliderToTest is AABBCollider aabb)
            {
                return this.getCollisionResultAABB(aabb, out  touching, out  overlap);
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

        public CollisionDirection getCollisionResultAABB(AABBCollider box, out bool touching, out double overlap)
        {
            if ((touching = !areBoxesNotTouching(this, box)))
            {
               
            }
            overlap = 0;
            return CollisionDirection.none;
        }

        public CollisionDirection getCollisionResultPoint(Vector3D point, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
            return CollisionDirection.none;
        }

        public CollisionDirection getCollisionResultSphere(SphereCollider sphere, out bool touching, out double overlap)
        {
            overlap = 0;
            touching = false;
            return CollisionDirection.none;
        }

        public void onTick()
        {
            double currentMinExtentX = minExtentX;
            double currentMinExtentY = minExtentY;
            double currentMinExtentZ = minExtentZ;

            double currentMaxExtentX = maxExtentX;
            double currentMaxExtentY = maxExtentY;
            double currentMaxExtentZ = maxExtentZ;

            Vector3D pos = parent.getPosition();

            minX = pos.x - currentMinExtentX;
            minY = pos.y - currentMinExtentY;
            minZ = pos.z - currentMinExtentZ;
                   
            maxX = pos.x - currentMaxExtentX;
            maxY = pos.y - currentMaxExtentY;
            maxZ = pos.z - currentMaxExtentZ;
        }

        public bool getHasParent()
        {
            return parent != null;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            AABBCollider result = new AABBCollider(minBounds, maxBounds, parent);
            double currentMinExtentX = minExtentX;
            double currentMinExtentY = minExtentY;
            double currentMinExtentZ = minExtentZ;

            double currentMaxExtentX = maxExtentX;
            double currentMaxExtentY = maxExtentY;
            double currentMaxExtentZ = maxExtentZ;

            Vector3D nextPos = parent.getPredictedNextTickPos();

            result.minX = nextPos.x - currentMinExtentX;
            result.minY = nextPos.y - currentMinExtentY;
            result.minZ = nextPos.z - currentMinExtentZ;
                         
            result.maxX = nextPos.x - currentMaxExtentX;
            result.maxY = nextPos.y - currentMaxExtentY;
            result.maxZ = nextPos.z - currentMaxExtentZ;

            return result;
        }

        

        public double minX { get => minBounds.x; set => minBounds.x = value; }
        public double minY { get => minBounds.y; set => minBounds.y = value; }
        public double minZ { get => minBounds.z; set => minBounds.z = value; }

        public double maxX { get => maxBounds.x; set => maxBounds.x = value; }
        public double maxY { get => maxBounds.y; set => maxBounds.y = value; }
        public double maxZ { get => maxBounds.z; set => maxBounds.z = value; }


        //extent is how much the aabb extends from its center 
        public double minExtentX { get => (maxBounds.x - minBounds.x) / 2D; }
        public double minExtentY { get => (maxBounds.y - minBounds.y) / 2D; }
        public double minExtentZ { get => (maxBounds.z - minBounds.z) / 2D; }

        public double maxExtentX { get => (minBounds.x + maxBounds.x) / 2D; }
        public double maxExtentY { get => (minBounds.y + maxBounds.y) / 2D; }
        public double maxExtentZ { get => (minBounds.z + maxBounds.z) / 2D; }
    }
}
