using Coictus.FredsMath;

namespace Coictus
{
    public struct PointCollider : ICollider
    {
        public Vector3D pos;
        public PositionalObject parent;

        public PointCollider(Vector3D pos, PositionalObject parent = null)
        {
            this.pos = pos;
            this.parent = parent;
        }
     
        public bool getHasParent()
        {
            return parent != null;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            if (parent != null)
            {
                PointCollider result = new PointCollider(pos);
                result.pos += parent.getPredictedNextTickPos();
                return result;
            }
            return this;
        }

        public int getCollisionWeight()
        {
            if (getHasParent())
            {
                return parent.getCollisionWeight();
            }

            return 0;
        }

        public PositionalObject getParent()
        {
            return parent;
        }

        public void onTick()
        {
            pos = parent.getPosition();
        }

        public ColliderType getType()
        {
            return ColliderType.point;
        }
    }
}
