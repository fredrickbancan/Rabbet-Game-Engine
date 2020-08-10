using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics, inventory and/or non-batched draw call is an entity.*/

    public class Entity
    {
        private double groundPlaneHeight = 0.0000D;
        protected Vector3D previousTickPos;
        protected Vector3D pos;
        protected Vector3D velocity;
        protected EntityModel entityModel;
        public static readonly double defaultAirResistance = 0.03572F;
        public static readonly double defaultGroundResistance = 0.72F;
        public static readonly double defaultGravity = 0.03572F;
        protected bool hasModel = false;
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
            yaw = -90.0F; // make entity face -z
        }
        
        public Entity(Vector3D spawnPosition)
        {
            this.pos = spawnPosition;
            previousTickPos = pos;
            yaw = -90.0F;// make entity face -z
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

            if (pos.y <= groundPlaneHeight) isGrounded = true; else isGrounded = false;//basic ground level collision detection, in this case there is a ground plane collider at 0.0000D

            //decrease entity y velocity by gravity, will not spiral out of control due to terminal velocity.
            if (!isFlying && !isGrounded) velocity.y -= gravity;

            //to prevent the entity from going through  the ground plane, if the next position increased by velocity will place the entity
            //below the ground plane, it will be given a value of 0.0000D -pos.y, so when position is increased by velocity, they cancel out resulting
            //in perfect 0, which stops the entity perfectly on the ground plane.
            if (pos.y + velocity.y < groundPlaneHeight) velocity.y = groundPlaneHeight - pos.y;

            if (hasModel)
            {
                updateModel();
            }

            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }

            /*do this last*///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            pos += velocity;
        }

        protected virtual void updateModel()
        {
            entityModel.updateModel();
        }
        public virtual bool getHasModel()
        {
            return hasModel;
        }

        public virtual EntityModel getEntityModel()
        {
            return this.entityModel;
        }

        public virtual void rotateRoll(double amount)
        {
            roll += amount;
        }
        public virtual void rotateYaw(double amount)
        {
            yaw += amount;
        }

        public virtual void rotatePitch(double amount)
        {
            pitch += amount;
        }

        public virtual void setRoll(double amount)
        {
            roll = amount;
        }

        public virtual void setYaw(double amount)
        {
            yaw = amount;
        }
        public virtual void setPitch(double amount)
        {
            pitch = amount;
        }

        public virtual double getYaw()
        {
            return yaw;
        }
        public virtual double getRoll()
        {
            return roll;
        }
        public virtual double getPitch()
        {
            return pitch;
        }
        public virtual Vector3D getPosition()
        {
            return this.pos;
        }
        public virtual Vector3D getVelocity()
        {
            return this.velocity;
        }
        public virtual bool getIsFlying()
        {
            return isFlying;
        }

        public virtual void setFlying(bool flag)
        {
            this.isFlying = flag;
        }

        public virtual void toggleFlying()
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
        public virtual void addYVelocity(double d)
        {
            velocity.y += d;
        }
        public virtual void setPosition(Vector3D newPos)
        {
            this.pos = newPos;
        }
        public virtual Vector3D getLerpPos()
        {
            return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
        }

    }
}
