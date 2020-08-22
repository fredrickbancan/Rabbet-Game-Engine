using Coictus.FredsMath;

namespace Coictus
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABBCollider : ICollider
    {
        public PositionalObject parent;
        public Vector3D minBounds;
        public Vector3D maxBounds;

        /*these vectors are in aabb relative space. They point from
          the center of the aabb to the direction specified.
          They can be used for collision detection. If the aabb
          is resized, these must be recalculated.*/
        public Vector3D vecToBackRight;
        public Vector3D vecToBackLeft;
        public Vector3D vecToFrontRight;

        public AABBCollider(Vector3D minBounds, Vector3D maxBounds, PositionalObject parent = null)
        {
            this.minBounds = Vector3D.zero;
            this.maxBounds = Vector3D.zero;
            vecToBackRight = Vector3D.zero;
            vecToBackLeft = Vector3D.zero;
            vecToFrontRight = Vector3D.zero;
            this.parent = parent;
            setBounds(minBounds, maxBounds, parent);
        }

        /*sets the bounds for this aabb. if the parent entity is provided and is not null, the bounds will be relative to the parent entity as a center origin
          otherwise the bounds will be absolutely world space orientated.*/
        public AABBCollider setBounds(Vector3D minBounds, Vector3D maxBounds, PositionalObject parent = null)
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
            
            vecToBackRight = (maxBounds - minBounds) / 2D;
            vecToBackLeft = new Vector3D(-(maxBounds.x - minBounds.x) / 2D, (maxBounds.y - minBounds.y) / 2D, (maxBounds.z - minBounds.z) / 2D);
            vecToFrontRight = new Vector3D((maxBounds.x - minBounds.x) / 2D, (maxBounds.y - minBounds.y) / 2D, -(maxBounds.z - minBounds.z) / 2D);
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        /*returns true if a vector is NOT within a box's bounds*/
        public static bool isPositionNotInsideBox(AABBCollider box, Vector3D position)
        {
            return position.x > box.maxX || position.x < box.minX
                || position.y > box.maxY || position.y < box.minY
                || position.z > box.maxZ || position.z < box.minZ;
        }
        public void onTick()
        {
            if (this.getHasParent())
            {
                Vector3D boundDistFromCenter = (maxBounds - minBounds) / 2D;
                maxBounds = parent.getPosition() + boundDistFromCenter;
                minBounds = parent.getPosition() - boundDistFromCenter;
            }
        }

        public bool getHasParent()
        {
            return parent != null;
        }
        
        public int getCollisionWeight()
        {
            if(getHasParent())
            {
                return parent.getCollisionWeight();
            }

            return 0;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            if(this.getHasParent())
            {
                Vector3D boundDistFromCenter = (maxBounds - minBounds) / 2D;
                AABBCollider result = new AABBCollider(minBounds, maxBounds, parent);
                result.maxBounds = parent.getPredictedNextTickPos() + boundDistFromCenter;
                result.minBounds = parent.getPredictedNextTickPos() - boundDistFromCenter;
                return result;
            }
            return this;
        }

        public ColliderType getType()
        {
            return ColliderType.aabb;
        }

        public PositionalObject getParent()
        {
            return parent;
        }

        public double minX { get => minBounds.x; set => minBounds.x = value; }
        public double minY { get => minBounds.y; set => minBounds.y = value; }
        public double minZ { get => minBounds.z; set => minBounds.z = value; }

        public double maxX { get => maxBounds.x; set => maxBounds.x = value; }
        public double maxY { get => maxBounds.y; set => maxBounds.y = value; }
        public double maxZ { get => maxBounds.z; set => maxBounds.z = value; }


        //extent is how much the aabb extends from its center 
        public double extentX { get => (maxBounds.x - minBounds.x) / 2D; }
        public double extentY { get => (maxBounds.y - minBounds.y) / 2D; }
        public double extentZ { get => (maxBounds.z - minBounds.z) / 2D; }
        public Vector3D centerVec { get => minBounds + ((maxBounds - minBounds) / 2D); }//relative to world
    }
}
