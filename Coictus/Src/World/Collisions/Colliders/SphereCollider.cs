using OpenTK;

namespace Coictus
{
    public struct SphereCollider : ICollider
    {
        public PositionalObject parent;
        public Vector3 pos;
        public float radius;

        public SphereCollider(Vector3 pos, float radius, PositionalObject parent = null)
        {
            this.pos = pos;
            this.radius = radius;
            this.parent = parent;
        }

        public bool getHasParent()
        {
            return parent != null;
        }

        public ICollider getNextTickPredictedHitbox()
        {
            if (this.getHasParent())
            {
                SphereCollider result = new SphereCollider(pos, radius, parent);
                if (parent != null)
                {
                    result.pos = parent.getPredictedNextTickPos();
                }
                return result;
            }
            return this;
        }

        public PositionalObject getParent()
        {
            return parent;
        }

        public int getCollisionWeight()
        {
            if (getHasParent())
            {
                return parent.getCollisionWeight();
            }

            return 0;
        }

        public void onTick()
        {
            if (parent != null)
            {
                pos = parent.getPosition();
            }
        }
        public ColliderType getType()
        {
            return ColliderType.sphere;
        }
    }
}
