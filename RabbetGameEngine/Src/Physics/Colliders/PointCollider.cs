using OpenTK;

namespace RabbetGameEngine.Physics
{
    public struct PointCollider : ICollider
    {
        public Vector3 pos;

        public PointCollider(Vector3 pos)
        {
            this.pos = pos;
        }

        public Vector3 getCenterVec()
        {
            return pos;
        }

        public void offset(Vector3 vec)
        {
            pos += vec;
        }
        public ColliderType getType()
        {
            return ColliderType.point;
        }
    }
}
