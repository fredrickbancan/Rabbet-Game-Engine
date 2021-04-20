using OpenTK.Mathematics;

namespace RabbetGameEngine
{
    public class PositionalObject
    {
        protected Vector3 prevTickPos;
        protected Vector3 pos;
        protected Vector3 velocity;
        protected Vector3 acceleration;
        protected float pitch;
        protected float yaw;
        protected float roll;
        protected float prevTickPitch;
        protected float prevTickYaw;
        protected float prevTickRoll;

        public void rotateRoll(float amount)
        {
            roll += amount;
        }
        public void rotateYaw(float amount)
        {
            yaw += amount;
        }
        public void rotatePitch(float amount)
        {
            pitch += amount;
        }

        public void setRoll(float amount)
        {
            roll = amount;
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
        public float getRoll()
        {
            return roll;
        }
        public float getPitch()
        {
            return pitch;
        }
        public Vector3 getPosition()
        {
            return this.pos;
        }
        public Vector3 getPrevTickPosition()
        {
            return this.prevTickPos;
        }
        public Vector3 getVelocity()
        {
            return this.velocity;
        }

        //useful for predicting and compensating for collisions
        public virtual Vector3 getPredictedNextTickPos()
        {
            return pos + velocity;
        }
        public void addVelocity(Vector3 v)
        {
            velocity += v;
        }
        public void setVelocity(Vector3 v)
        {
            velocity = v;
        }
        public void setAccel(Vector3 v)
        {
            acceleration = v;
        }

        public void setXAccel(float v)
        {
            acceleration.X = v;
        }
        public void setYAccel(float v)
        {
            acceleration.Y = v;
        }
        public void setZAccel(float v)
        {
            acceleration.Z = v;
        }
        public Vector3 lerpPos
        {
            get
            {
                return prevTickPos + (pos - prevTickPos) * TicksAndFrames.getPercentageToNextTick();
            }
        }

        public float lerpPitch
        {
            get
            {
                return prevTickPitch + (pitch - prevTickPitch) * TicksAndFrames.getPercentageToNextTick();
            }
        }

        public float lerpYaw
        {
            get
            {
                return prevTickYaw + (yaw - prevTickYaw) * TicksAndFrames.getPercentageToNextTick();
            }
        }

        public float lerpRoll
        {
            get
            {
                return prevTickRoll + (roll - prevTickRoll) * TicksAndFrames.getPercentageToNextTick();
            }
        }
        public float posX { get => pos.X; set => pos.X = value; }
        public float posY { get => pos.Y; set => pos.Y = value; }
        public float posZ { get => pos.Z; set => pos.Z = value; }
    }
}
