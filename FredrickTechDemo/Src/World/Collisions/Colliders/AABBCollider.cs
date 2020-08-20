using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*A struct for doing axis aligned bounding box collisions*/
    public struct AABBCollider : ICollider
    {
        public Entity parent;
        public Vector3D minBounds;
        public Vector3D maxBounds;

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

        public Entity getParent()
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

        //vector from center of aabb to one of the corners, can negate for bottom corners. Vector is relative to aabb, not world
        public Vector3D maxXmaxYmaxZ { get => maxBounds; }
        public Vector3D minXmaxYmaxZ { get => new Vector3D(minBounds.x, maxBounds.y, maxBounds.z); }
        public Vector3D maxXmaxYminZ { get => new Vector3D(maxBounds.x, maxBounds.y, minBounds.z); }
    }
}
