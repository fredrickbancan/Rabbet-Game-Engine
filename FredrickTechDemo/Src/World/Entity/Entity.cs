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
        public static readonly double defaultAirResistance = 0.03572F;
        public static readonly double defaultGroundResistance = 0.72F;
        public static readonly double defaultGravity = 0.03572F;
        protected double resistance = defaultAirResistance;
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

            //resist velocity differently depending on state
            if (isGrounded) resistance = defaultGroundResistance; else resistance = defaultAirResistance;

            /*decelerate velocity by air resistance (not accurate to real life)*/
            velocity.x *= (1 - resistance);
            velocity.z *= (1 - resistance);
            velocity.y *= (1 - defaultAirResistance);

            if (pos.y <= 0.0000D) isGrounded = true; else isGrounded = false;//basic ground level collision detection, in this case there is a ground plane collider at 0.0000D

            //decrease entity y velocity by gravity, will not spiral out of control due to terminal velocity.
            if (!isFlying && !isGrounded) velocity.y -= gravity;

            //to prevent the entity from going through  the ground plane, if the next position increased by velocity will place the entity
            //below the ground plane, it will be given a value of 0.0000D -pos.y, so when position is increased by velocity, they cancel out resulting
            //in perfect 0, which stops the entity perfectly on the ground plane.
            if (pos.y + velocity.y < 0.0000D) velocity.y = 0.0000D - pos.y;

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
        public void addYVelocity(double d)
        {
            velocity.y += d;
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
