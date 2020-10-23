using OpenTK;
using RabbetGameEngine.Physics;

namespace RabbetGameEngine
{
    /*Abstraction base class of objects with any type of position, rotation, velocity, interpolation.*/
    public class PositionalObject
    {
        protected Vector3 previousTickPos;
        protected Vector3 pos;
        protected Vector3 velocity;
        protected Vector3 acceleration;

        public static readonly float defaultAirResistance = 0.00272F;
        public static readonly float defaultGroundResistance = 0.3572F;
        public static readonly float defaultGravity = 0.03572F;
        protected float airResistance = defaultAirResistance;
        protected float groundResistance = defaultGroundResistance;
        public float gravity = defaultGravity;

        protected float pitch;
        protected float yaw;
        protected float roll;
        protected float prevTickPitch;
        protected float prevTickYaw;
        protected float prevTickRoll;

        protected ICollider collider = null;
        protected bool hasCollider = false;
        protected bool hasDoneFirstUpdate = false;
        protected bool isGrounded = false;
        protected bool hasCollided = false;

        public PositionalObject()
        {
            pos = new Vector3();
            previousTickPos = pos;
            setYaw(-90F);//face neg z
        }
        public PositionalObject(Vector3 initialPosition)
        {
            pos = initialPosition;
            previousTickPos = pos;
            setYaw(-90F);//face neg z
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

            //do last
            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }
        }

        /*Called every frame, can be overwritten*/
        public virtual void onFrame()
        {
            
        }

        /*Apply a force to this entity from the location with the power.*/
        public virtual void applyImpulseFromLocation(Vector3 loc, float power)
        {
            //adding a tiny pos y bias to the impulses
            velocity += Vector3.Normalize((pos + new Vector3(0, 0.5F, 0)) - loc) * power;
        }

        public virtual void rotateRoll(float amount)
        {
            roll += amount;
        }
        public virtual void rotateYaw(float amount)
        {
            yaw += amount;
        }
        public virtual void rotatePitch(float amount)
        {
            pitch += amount;
        }

        public virtual void setRoll(float amount)
        {
            roll = amount;
        }

        public virtual void setYaw(float amount)
        {
            yaw = amount;
        }
        public virtual void setPitch(float amount)
        {
            pitch = amount;
        }
        public virtual float getYaw()
        {
            return yaw;
        }
        public virtual float getRoll()
        {
            return roll;
        }
        public virtual float getPitch()
        {
            return pitch;
        }
        public virtual Vector3 getPosition()
        {
            return this.pos;
        }
        public virtual Vector3 getVelocity()
        {
            return this.velocity;
        }

        //useful for predicting and compensating for collisions
        public virtual Vector3 getPredictedNextTickPos()
        {
            return pos + velocity;
        }

        public virtual void setXVelocity(float d)
        {
            velocity.X = d;
        }
        public virtual void setYVelocity(float d)
        {
            velocity.Y = d;
        }
        public virtual void setZVelocity(float d)
        {
            velocity.Z = d;
        }
        public virtual void addXVelocity(float d)
        {
            velocity.X += d;
        }
        public virtual void addYVelocity(float d)
        {
            velocity.Y += d;
        }
        public virtual void addZVelocity(float d)
        {
            velocity.Z += d;
        }
        public virtual void scaleXVelocity(float d)
        {
            velocity.X *= d;
        }
        public virtual void scaleYVelocity(float d)
        {
            velocity.Y *= d;
        }
        public virtual void scaleZVelocity(float d)
        {
            velocity.Z *= d;
        }
        public virtual void addVelocity(Vector3 v)
        {
            velocity += v;
        }
        public virtual void setVelocity(Vector3 v)
        {
            velocity = v;
        }

        public virtual void setAccel(Vector3 v)
        {
            acceleration = v;
        }
        public virtual void setXAccel(float v)
        {
            acceleration.X = v;
        }
        public virtual void setYAccel(float v)
        {
            acceleration.Y = v;
        }
        public virtual void setZAccel(float v)
        {
            acceleration.Z = v;
        }

        public virtual void setAirResistance(float d)
        {
            airResistance = d;
        }

        public virtual void setPosition(Vector3 newPos)
        {
            this.pos = newPos;
        }
        public virtual Vector3 getLerpPos()
        {
            if (hasDoneFirstUpdate)
            {
                return previousTickPos + (pos - previousTickPos) * TicksAndFps.getPercentageToNextTick();
            }
            return pos;
        }
        public virtual float getLerpPitch()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickPitch + (pitch - prevTickPitch) * TicksAndFps.getPercentageToNextTick();
            }
            return pitch;
        }
        public virtual float getLerpYaw()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickYaw + (yaw - prevTickYaw) * TicksAndFps.getPercentageToNextTick();
            }
            return yaw;
        }
        public virtual float getLerpRoll()
        {
            if (hasDoneFirstUpdate)
            {
                return prevTickRoll + (roll - prevTickRoll) * TicksAndFps.getPercentageToNextTick();
            }
            return roll;
        }

        public virtual bool getHasCollider()
        {
            return hasCollider;
        }
        public virtual bool getIsGrounded()
        {
            return isGrounded;
        }

        public virtual void setIsGrounded(bool flag)
        {
            isGrounded = flag;
        }

        public virtual ref ICollider getColliderHandle()
        {
            return ref collider;
        }
        public virtual ICollider getCollider()
        {
            return collider;
        }

        public void setHasCollided(bool flag)
        {
            hasCollided = flag;
        }

        public virtual void setCollider(ICollider collider)
        {
            this.collider = collider;
        }
        public virtual float posX { get => pos.X; set => pos.X = value; }
        public virtual float posY { get => pos.Y; set => pos.Y = value; }
        public virtual float posZ { get => pos.Z; set => pos.Z = value; }
    }
}
