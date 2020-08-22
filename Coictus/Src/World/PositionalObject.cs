using Coictus.FredsMath;

namespace Coictus
{
    /*Abstraction base class of objects with any type of position, rotation, velocity, interpolation.*/
    public class PositionalObject
    {
        protected Vector3D previousTickPos;
        protected Vector3D pos;
        protected Vector3D velocity;
        protected Vector3D acceleration;

        public static readonly double defaultAirResistance = 0.03572F;
        public static readonly double defaultGroundResistance = 0.3572F;
        public static readonly double defaultGravity = 0.03572F;
        protected double airResistance = defaultAirResistance;
        protected double groundResistance = defaultGroundResistance;
        public double gravity = defaultGravity;

        protected double pitch;
        protected double yaw;
        protected double roll;
        protected double prevTickPitch;
        protected double prevTickYaw;
        protected double prevTickRoll;

        protected ICollider collider = null;
        protected bool hasCollider = false;
        protected int collisionWeight; //Objects with more collisionweight than other objects have priority in doing coliisions. If they are equal, they can push eachother.

        protected bool hasDoneFirstUpdate = false;

        public PositionalObject()
        {
            pos = new Vector3D();
            previousTickPos = pos;
            setYaw(-90D);//face neg z
        }
        public PositionalObject(Vector3D initialPosition)
        {
            pos = initialPosition;
            previousTickPos = pos;
            setYaw(-90D);//face neg z
        }

        /*Do this before manipulating any movement of this object*/
        public virtual void preTickMovement()
        {
            previousTickPos = pos;
            prevTickPitch = pitch;
            prevTickYaw = yaw;
            prevTickRoll = roll;
        }


        /*Do this after manipulating any movement of this object*/
        public virtual void postTickMovement()
        {
            velocity += acceleration;
            velocity *= (1 - airResistance);
            pos += velocity;

            if (hasCollider && collider != null)
            {
                collider.onTick();
            }
        }

        /*Called every frame, can be overwritten*/
        public virtual void onFrame()
        {

        }

        /*Done after the entity has ticked, so will correct for overlap AFTER movement. Will change velocity, accelleration and positon.
          Can be overritten.*/
        public virtual void applyCollision(Vector3D direction, double overlap)
        {
            //make sure direction is normal vec
            direction.normalize();

            //correct position
            pos += direction * overlap;

            //clip velocity
            velocity *= direction.oneMinusAbsolute();//TODO: Only works with axis-aligned collision directions. anything else, will still work, but velocity will be slowed in wrong directions.
        }

        /*Apply a force to this entity from the location with the power.*/
        public virtual void applyImpulseFromLocation(Vector3D loc, double power)
        {
            //adding a tiny pos y bias to the impulses
            velocity += Vector3D.normalize((pos + new Vector3D(0, 0.5, 0)) - loc) * power;
        }

        /*used for setting the collider of this object*/
        protected virtual void setCollider(ICollider collider, int collisionWeight = 0)
        {
            this.collider = collider;
            if (this.collider != null)
            {
                this.hasCollider = true;
                this.collisionWeight = collisionWeight;
            }
        }

        /*Called when an object touches another. Can be used for doing things such as detecting direct hits with projectiles, damage on touching a damaging trigger etc.*/
        public virtual void onCollidedBy(PositionalObject other)
        {

        }

        public virtual int getCollisionWeight()
        {
            return collisionWeight;
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

        //useful for predicting and compensating for collisions
        public virtual Vector3D getPredictedNextTickPos()
        {
            return pos + velocity;
        }

        public virtual void setXVelocity(double d)
        {
            velocity.x = d;
        }
        public virtual void setYVelocity(double d)
        {
            velocity.y = d;
        }
        public virtual void setZVelocity(double d)
        {
            velocity.z = d;
        }
        public virtual void addXVelocity(double d)
        {
            velocity.x += d;
        }
        public virtual void addYVelocity(double d)
        {
            velocity.y += d;
        }
        public virtual void addZVelocity(double d)
        {
            velocity.z += d;
        }
        public virtual void scaleXVelocity(double d)
        {
            velocity.x *= d;
        }
        public virtual void scaleYVelocity(double d)
        {
            velocity.y *= d;
        }
        public virtual void scaleZVelocity(double d)
        {
            velocity.z *= d;
        }
        public virtual void addVelocity(Vector3D v)
        {
            velocity += v;
        }
        public virtual void setVelocity(Vector3D v)
        {
            velocity = v;
        }

        public virtual void setAccel(Vector3D v)
        {
            acceleration = v;
        }
        public virtual void setXAccel(double v)
        {
            acceleration.x = v;
        }
        public virtual void setYAccel(double v)
        {
            acceleration.y = v;
        }
        public virtual void setZAccel(double v)
        {
            acceleration.z = v;
        }

        public virtual void setAirResistance(double d)
        {
            airResistance = d;
        }

        public virtual void setPosition(Vector3D newPos)
        {
            this.pos = newPos;
        }
        public virtual Vector3D getLerpPos()
        {
            if (hasDoneFirstUpdate)
            {
                return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
            }
            return pos;
        }
        public virtual double getLerpPitch()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickPitch + (pitch - prevTickPitch) * TicksAndFps.getPercentageToNextTick();
            }
            return pitch;
        }
        public virtual double getLerpYaw()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickYaw + (yaw - prevTickYaw) * TicksAndFps.getPercentageToNextTick();
            }
            return yaw;
        }
        public virtual double getLerpRoll()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickRoll + (roll - prevTickRoll) * TicksAndFps.getPercentageToNextTick();
            }
            return roll;
        }

        public virtual double posX { get => pos.x; set => pos.x = value; }
        public virtual double posY { get => pos.y; set => pos.y = value; }
        public virtual double posZ { get => pos.z; set => pos.z = value; }
    }
}
