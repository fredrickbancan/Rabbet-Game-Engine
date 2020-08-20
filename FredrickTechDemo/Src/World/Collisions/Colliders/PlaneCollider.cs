using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct PlaneCollider : ICollider
    {
        public Vector3D normal;
        public double scalar;

        public PlaneCollider(Vector3D normal, double scalar)
        {
            this.normal = Vector3D.normalize(normal);
            this.scalar = scalar;
        }
     
        public bool getHasParent()
        {
            return false;
        }

        public Entity getParent()
        {
            return null;
        }
        public ICollider getNextTickPredictedHitbox()
        {
            return this;
        }

        public void onTick()
        {
         
        }
        public ColliderType getType()
        {
            return ColliderType.plane;
        }
    }
}
