using FredrickTechDemo.FredsMath;
namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics, inventory and/or non-batched draw call is an entity.*/

    public class Entity
    {
        protected Vector3F previousTickPos;
        protected Vector3F pos;
        protected Vector3F velocity;
        public static readonly float defaultAirResistance = 0.9F;
        public static readonly float defaultGravity = 0.05F;
        protected float airResistance = defaultAirResistance;
        protected float gravity = defaultGravity;
        protected bool isFlying = false;
        protected bool isGrounded = false;
        protected float pitch;
        protected float yaw;
        protected float roll;
        protected bool hasDoneFirstUpdate = false;
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
            /*do this first.*///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            previousTickPos = pos;
            if (yaw > 360.0F) {  yaw = 0.0F; }
            if (yaw < -360.0F) { yaw = 0.0F; }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            /*decelerate velocity by air resistance (not accurate to real life)*/
            velocity *= airResistance;
            //decrease entity y velocity by gravity, will not spiral out of control due to terminal velocity.
            if(!isFlying && !isGrounded) velocity.y -= gravity;


            /*do this last*///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            pos += velocity;
            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }
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

        public float getYaw()
        {
            return yaw;
        }
        public Vector3F getPosition()
        {
            return this.pos;
        }
        public Vector3F getVelocity()
        {
            return this.velocity;
        }
        public bool getIsFlying()
        {
            return isFlying;
        }

        public void setFlying(bool flag)
        {
            this.isFlying = flag;
        }

        public void toggleFlying()
        {
            if(!isFlying)
            {
                isFlying = true;
            }
            else
            {
                isFlying = false;
            }
        }
        public Vector3F getLerpPos()
        {
            return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
        }

    }
}
