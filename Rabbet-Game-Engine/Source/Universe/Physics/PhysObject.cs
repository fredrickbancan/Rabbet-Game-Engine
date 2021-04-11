using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class PhysObject : PositionalObject
    {
        protected float mass;
        protected bool isStatic = false;

        public void applyForce(Vector3 force)
        {
            if (isStatic) return;

            velocity += force / mass;
        }

        public bool getIsStatic()
        {
            return isStatic;
        }
    }
}
