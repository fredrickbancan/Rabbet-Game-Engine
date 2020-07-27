using FredrickTechDemo.FredsMath;
namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics, inventory and/or non-batched draw call is an entity.*/

    class Entity
    {
        protected Vector3F previousTickPos;
        protected Vector3F pos;
        protected Vector3F velocity;
        protected float airResistanceFactor = 0.93572F;

        protected float pitch;
        protected float yaw;
        protected float roll;
        public Entity()
        {
            this.pos = new Vector3F();
            previousTickPos = pos;
            yaw = -90.0F;
        }
        
        public Entity(Vector3F spawnPosition)
        {
            this.pos = spawnPosition;
            previousTickPos = pos;
            yaw = -90.0F;
        }

        /*Called every tick*/
        public virtual void onTick()
        {
            /*do this first. Used for interpolation.*/
            previousTickPos = pos;
            /*decelerate velocity by air resistance (not accurate to real life)*/
            velocity *= airResistanceFactor;

            /*do this last*/
            pos += velocity;
        }

        public void rotateYaw(float amount)
        {
            yaw += amount;
        }

        public void rotatePitch(float amount)
        {
            pitch += amount;
        }

        public void setYaw(float amount)
        {
            yaw = amount;
        }

        public void setPitch(float amount)
        {
            pitch = amount;
        }

        public Vector3F getPosition()
        {
            return this.pos;
        }

        public Vector3F getLerpPos()
        {
            return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
        }

    }
}
