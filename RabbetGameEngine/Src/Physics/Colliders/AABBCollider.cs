using OpenTK;
namespace RabbetGameEngine.Physics
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABBCollider : ICollider
    {
        public PositionalObject parent;
        public Vector3 minBounds;
        public Vector3 maxBounds;

        /*these vectors are in aabb relative space. They point from
          the center of the aabb to the direction specified.
          They can be used for collision detection. If the aabb
          is resized, these must be recalculated.*/
        public Vector3 vecToBackRight;
        public Vector3 vecToBackLeft;
        public Vector3 vecToFrontRight;

        public AABBCollider(Vector3 minBounds, Vector3 maxBounds, PositionalObject parent = null)
        {
            this.minBounds = Vector3.Zero;
            this.maxBounds = Vector3.Zero;
            vecToBackRight = Vector3.Zero;
            vecToBackLeft = Vector3.Zero;
            vecToFrontRight = Vector3.Zero;
            this.parent = parent;
            setBounds(minBounds, maxBounds, parent);
        }

        /*sets the bounds for this aabb. if the parent entity is provided and is not null, the bounds will be relative to the parent entity as a center origin
          otherwise the bounds will be absolutely world space orientated.*/
        public AABBCollider setBounds(Vector3 minBounds, Vector3 maxBounds, PositionalObject parent = null)
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
            
            vecToBackRight = (maxBounds - minBounds) / 2;
            vecToBackLeft = new Vector3(-(maxBounds.X - minBounds.X) / 2, (maxBounds.Y - minBounds.Y) / 2, (maxBounds.Z - minBounds.Z) / 2);
            vecToFrontRight = new Vector3((maxBounds.X - minBounds.X) / 2, (maxBounds.Y - minBounds.Y) / 2, -(maxBounds.Z - minBounds.Z) / 2);
            return this;
        }

        /*returns true if two bounding boxes are NOT touching in any way*/
        public static bool areBoxesNotTouching(AABBCollider boxA, AABBCollider boxB)
        {
            return boxA.minX > boxB.maxX || boxA.minY > boxB.maxY || boxA.minZ > boxB.maxZ
                || boxA.maxX < boxB.minX || boxA.maxY < boxB.minY || boxA.maxZ < boxB.minZ;
        }

        /*returns true if a vector is NOT within a box's bounds*/
        public static bool isPositionNotInsideBox(AABBCollider box, Vector3 position)
        {
            return position.X > box.maxX || position.X < box.minX
                || position.Y > box.maxY || position.Y < box.minY
                || position.Z > box.maxZ || position.Z < box.minZ;
        }
        public void onTick()
        {
            //replaced by new tryToMoveEntity() func which offsets the aabb
           /* if (this.getHasParent())
            {
                Vector3 boundDistFromCenter = (maxBounds - minBounds) / 2;
                maxBounds = parent.getPosition() + boundDistFromCenter;
                minBounds = parent.getPosition() - boundDistFromCenter;
            }*/
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
                Vector3 boundDistFromCenter = (maxBounds - minBounds) / 2;
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

        public void setParent(PositionalObject parent)
        {
            this.parent = parent;
        }

        public Vector3 getCenterVec()
        {
            return centerVec;
        }

        public void offset(Vector3 direction)
        {
            minBounds += direction;
            maxBounds += direction;
        }

        public float minX { get => minBounds.X; set => minBounds.X = value; }
        public float minY { get => minBounds.Y; set => minBounds.Y = value; }
        public float minZ { get => minBounds.Z; set => minBounds.Z = value; }

        public float maxX { get => maxBounds.X; set => maxBounds.X = value; }
        public float maxY { get => maxBounds.Y; set => maxBounds.Y = value; }
        public float maxZ { get => maxBounds.Z; set => maxBounds.Z = value; }


        //extent is how much the aabb extends from its center 
        public float extentX { get => (maxBounds.X - minBounds.X) / 2; }
        public float extentY { get => (maxBounds.Y - minBounds.Y) / 2; }
        public float extentZ { get => (maxBounds.Z - minBounds.Z) / 2; }
        public Vector3 centerVec { get => minBounds + ((maxBounds - minBounds) / 2); }//relative to world
    }
}
