using FredrickTechDemo.FredsMath;
namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics, inventory and/or non-batched draw call is an entity.*/

    public class Entity
    {
        protected Vector3D previousTickPos;
        protected Vector3D pos;
        protected Vector3D velocity;
        public static readonly double defaultAirResistance = 0.9F;
        public static readonly double defaultGravity = 0.06F;
        protected double airResistance = defaultAirResistance;
        protected double gravity = defaultGravity;
        protected bool isFlying = false;
        protected bool isGrounded = false;
        protected double pitch;
        protected double yaw;
        protected double roll;
        protected bool hasDoneFirstUpdate = false;
        public Entity()
        {
            this.pos = new Vector3D();
            previousTickPos = pos;
            yaw = -90.0F;
        }
        
        public Entity(Vector3D spawnPosition)
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

        public void rotateYaw(double amount)
        {
            yaw += amount;
        }

        public void rotatePitch(double amount)
        {
            pitch += amount;
        }

        public void setYaw(double amount)
        {
            yaw = amount;
        }

        public void setPitch(double amount)
        {
            pitch = amount;
        }

        public double getYaw()
        {
            return yaw;
        }
        public Vector3D getPosition()
        {
            return this.pos;
        }
        public Vector3D getVelocity()
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

        public void setPosition(Vector3D newPos)
        {
            this.pos = newPos;
        }
        public Vector3D getLerpPos()
        {
            return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
        }

    }
}
