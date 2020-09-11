using OpenTK;

namespace Coictus
{
    /*Abstraction base class of objects with any type of position, rotation, velocity, interpolation.*/
    public class PositionalObject
    {
        protected Vector3d previousTickPos;
        protected Vector3d pos;
        protected Vector3d velocity;
        protected Vector3d acceleration;

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
            pos = new Vector3d();
            previousTickPos = pos;
            setYaw(-90D);//face neg z
        }
        public PositionalObject(Vector3d initialPosition)
        {
            pos = initialPosition;
            previousTickPos = pos;
            setYaw(-90D);//face neg z
        }

        /*Do this before manipulating any movement of this object*/
        public virtual void preTick()
        {
            previousTickPos = pos;
            prevTickPitch = pitch;
            prevTickYaw = yaw;
            prevTickRoll = roll;
        }


        /*Do this after manipulating any movement of this object*/
        public virtual void postTick()
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
        public virtual void applyCollision(Vector3d direction, double overlap)
        {
            //make sure direction is normal vec
            direction.Normalize();

            //correct position
            pos += direction * overlap;

            //clip velocity
            velocity *= MathUtil.oneMinusAbsolute(direction);//TODO: Only works with axis-aligned collision directions. anything else, will still work, but velocity will be slowed in wrong directions.
        }

        /*Apply a force to this entity from the location with the power.*/
        public virtual void applyImpulseFromLocation(Vector3d loc, double power)
        {
            //adding a tiny pos y bias to the impulses
            velocity += Vector3d.Normalize((pos + new Vector3d(0, 0.5, 0)) - loc) * power;
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
        public virtual Vector3d getPosition()
        {
            return this.pos;
        }
        public virtual Vector3d getVelocity()
        {
            return this.velocity;
        }

        //useful for predicting and compensating for collisions
        public virtual Vector3d getPredictedNextTickPos()
        {
            return pos + velocity;
        }

        public virtual void setXVelocity(double d)
        {
            velocity.X = d;
        }
        public virtual void setYVelocity(double d)
        {
            velocity.Y = d;
        }
        public virtual void setZVelocity(double d)
        {
            velocity.Z = d;
        }
        public virtual void addXVelocity(double d)
        {
            velocity.X += d;
        }
        public virtual void addYVelocity(double d)
        {
            velocity.Y += d;
        }
        public virtual void addZVelocity(double d)
        {
            velocity.Z += d;
        }
        public virtual void scaleXVelocity(double d)
        {
            velocity.X *= d;
        }
        public virtual void scaleYVelocity(double d)
        {
            velocity.Y *= d;
        }
        public virtual void scaleZVelocity(double d)
        {
            velocity.Z *= d;
        }
        public virtual void addVelocity(Vector3d v)
        {
            velocity += v;
        }
        public virtual void setVelocity(Vector3d v)
        {
            velocity = v;
        }

        public virtual void setAccel(Vector3d v)
        {
            acceleration = v;
        }
        public virtual void setXAccel(double v)
        {
            acceleration.X = v;
        }
        public virtual void setYAccel(double v)
        {
            acceleration.Y = v;
        }
        public virtual void setZAccel(double v)
        {
            acceleration.Z = v;
        }

        public virtual void setAirResistance(double d)
        {
            airResistance = d;
        }

        public virtual void setPosition(Vector3d newPos)
        {
            this.pos = newPos;
        }
        public virtual Vector3d getLerpPos()
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

        public virtual double posX { get => pos.X; set => pos.X = value; }
        public virtual double posY { get => pos.Y; set => pos.Y = value; }
        public virtual double posZ { get => pos.Z; set => pos.Z = value; }
    }
}
