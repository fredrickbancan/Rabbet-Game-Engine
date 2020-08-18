using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    public struct PointCollider : ICollider
    {
        public Vector3D pos;
        public Entity parent;

        public PointCollider(Vector3D pos, Entity parent = null)
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
            throw new System.NotImplementedException();
        }

        public void onTick()
        {
            throw new System.NotImplementedException();
        }
    }
}
