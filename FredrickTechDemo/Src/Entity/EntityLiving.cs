using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    public class EntityLiving : Entity
    {
        
        protected Vector3D frontVector;//vector pointing to the direction the entity is facing
        protected Vector3D upVector;
        protected Vector3D movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        protected double headPitch; // pitch of the living entity head
        public static readonly double defaultWalkSpeed = 0.02F;
        protected double walkSpeed = defaultWalkSpeed;
        public EntityLiving() : base()
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);
        }

        public EntityLiving(Vector3D pos) : base(pos)
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);
        }

        public override void onTick()
        {
            base.onTick();//do first

            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double) (Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
            if (isFlying)
            {
                frontVector.y = (double)Math.Sin(MathUtil.radians(headPitch));
            }
            else
            {//if the living entity isnt flying, it shouldnt be able to move up or down willingly
                frontVector.y = 0;
            }
            frontVector.z = (double) (Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
            frontVector.Normalize();

            this.moveByMovementVector();
        }
        private void moveByMovementVector()
        {
            double walkSpeedModified = walkSpeed;

            if (isFlying)
            {
                walkSpeedModified *= 3;
            }

            if(movementVector.Magnitude() > 0)
            {
                movementVector.Normalize();//movement vector is a unit vector.
                //change velocity based on movement
                velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement
                velocity += Vector3D.normalize(Vector3D.cross(frontVector, upVector)) * movementVector.x * walkSpeedModified;//strafing movement
                movementVector *= 0;//reset movement vector
            }
        }

        public void walkFowards()
        {
            ++movementVector.z;
        }
        public void walkBackwards()
        {
            --movementVector.z;
        }
        public void strafeRight()
        {
            ++movementVector.x;
        }
        public void strafeLeft()
        {
            --movementVector.x;
        }
        public void setHeadPitch(double pitch)
        {
            this.headPitch = pitch;
        }
        public double getHeadPitch()
        {
            return this.headPitch;
        }
        public Vector3D getFrontVector()
        {
            return this.frontVector;
        }

        public Vector3D getUpVector()
        {
            return this.upVector;
        }

        
    }
}
