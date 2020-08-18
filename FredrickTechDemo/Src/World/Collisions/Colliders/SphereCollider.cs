using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct SphereCollider : ICollider
    {
        public Entity parent;
        public Vector3D pos;
        public double radius;

        public SphereCollider(Vector3D pos, double radius, Entity parent = null)
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

        public void onTick()
        {
            if (parent != null)
            {
                pos = parent.getPosition();
            }
        }
    }
}
