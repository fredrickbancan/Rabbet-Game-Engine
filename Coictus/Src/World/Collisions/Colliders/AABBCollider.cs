using OpenTK;
namespace Coictus
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABBCollider : ICollider
    {
        public PositionalObject parent;
        public Vector3d minBounds;
        public Vector3d maxBounds;

        /*these vectors are in aabb relative space. They point from
          the center of the aabb to the direction specified.
          They can be used for collision detection. If the aabb
          is resized, these must be recalculated.*/
        public Vector3d vecToBackRight;
        public Vector3d vecToBackLeft;
        public Vector3d vecToFrontRight;

        public AABBCollider(Vector3d minBounds, Vector3d maxBounds, PositionalObject parent = null)
        {
            this.minBounds = Vector3d.Zero;
            this.maxBounds = Vector3d.Zero;
            vecToBackRight = Vector3d.Zero;
            vecToBackLeft = Vector3d.Zero;
            vecToFrontRight = Vector3d.Zero;
            this.parent = parent;
            setBounds(minBounds, maxBounds, parent);
        }

        /*sets the bounds for this aabb. if the parent entity is provided and is not null, the bounds will be relative to the parent entity as a center origin
          otherwise the bounds will be absolutely world space orientated.*/
        public AABBCollider setBounds(Vector3d minBounds, Vector3d maxBounds, PositionalObject parent = null)
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
            vecToBackLeft = new Vector3d(-(maxBounds.X - minBounds.X) / 2D, (maxBounds.Y - minBounds.Y) / 2D, (maxBounds.Z - minBounds.Z) / 2D);
            vecToFrontRight = new Vector3d((maxBounds.X - minBounds.X) / 2D, (maxBounds.Y - minBounds.Y) / 2D, -(maxBounds.Z - minBounds.Z) / 2D);
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        /*returns true if a vector is NOT within a box's bounds*/
        public static bool isPositionNotInsideBox(AABBCollider box, Vector3d position)
        {
            return position.X > box.maxX || position.X < box.minX
                || position.Y > box.maxY || position.Y < box.minY
                || position.Z > box.maxZ || position.Z < box.minZ;
        }
        public void onTick()
        {
            if (this.getHasParent())
            {
                Vector3d boundDistFromCenter = (maxBounds - minBounds) / 2D;
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
                Vector3d boundDistFromCenter = (maxBounds - minBounds) / 2D;
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

        public double minX { get => minBounds.X; set => minBounds.X = value; }
        public double minY { get => minBounds.Y; set => minBounds.Y = value; }
        public double minZ { get => minBounds.Z; set => minBounds.Z = value; }

        public double maxX { get => maxBounds.X; set => maxBounds.X = value; }
        public double maxY { get => maxBounds.Y; set => maxBounds.Y = value; }
        public double maxZ { get => maxBounds.Z; set => maxBounds.Z = value; }


        //extent is how much the aabb extends from its center 
        public double extentX { get => (maxBounds.X - minBounds.X) / 2D; }
        public double extentY { get => (maxBounds.Y - minBounds.Y) / 2D; }
        public double extentZ { get => (maxBounds.Z - minBounds.Z) / 2D; }
        public Vector3d centerVec { get => minBounds + ((maxBounds - minBounds) / 2D); }//relative to world
    }
}
