using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    public class EntityLiving : Entity
    {
        protected EntityVehicle currentVehicle;
        protected Vector3D frontVector;//vector pointing to the direction the entity is facing
        protected Vector3D upVector;
        protected Vector3D movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        protected bool isJumping = false;
        protected double headPitch; // Pitch of the living entity head
        public static readonly double defaultWalkSpeed = 0.3572F;
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

            alignVectors();

            moveByMovementVector();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected virtual void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = walkSpeed;

            if (!isGrounded)
            {
                if (isFlying)
                {
                    walkSpeedModified *= 0.05599104113337D;//reduce movespeed when flying as to not accellerate out of control
                }
                else
                {
                    walkSpeedModified *= 0.02015677491601D;//reduce movespeed when jumping or mid air 
                }
            }

            //change velocity based on movement
            //movement vector is a unit vector.
            movementVector.Normalize();//normalize vector so player is same speed in any direction
            velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement
            velocity += Vector3D.normalize(Vector3D.cross(frontVector, upVector)) * movementVector.x * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector

            if (isJumping)// if player jumping or flying up
            {
                velocity.y += 0.32D;//jump 
                isJumping = false;
            }
        }

        /*When called, aligns vectors according to the entities state and rotations.*/
        protected virtual void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)));
            if (isFlying)
            {
                frontVector.y = (double)Math.Sin(MathUtil.radians(headPitch));
            }
            else
            {//if the living entity isnt flying, it shouldnt be able to move up or down willingly
                frontVector.y = 0;
            }
            frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        public virtual void jump()
        {
            if (isGrounded && velocity.y <= 0)
            {
              
                isJumping = true;
            }
        }

        public virtual void mountVehicle(EntityVehicle theVehicle)
        {
            this.pos = theVehicle.getMountingPos();
            currentVehicle = theVehicle;
            theVehicle.setMountedEntity(this);
        }

        public virtual void walkFowards()
        {
            if (currentVehicle != null)
            {
                currentVehicle.driveFowards();
            }
            else
            {
                movementVector.z++;
            }
        }
        public virtual void walkBackwards()
        {
            if (currentVehicle != null)
            {
                currentVehicle.driveBackwards();
            }
            else
            {
                movementVector.z--;
            }
        }
        public virtual void strafeRight()
        {
            if (currentVehicle != null)
            {
                currentVehicle.turnRight();
            }
            else
            {
                movementVector.x++;
            }
        }
        public virtual void strafeLeft()
        {
            if (currentVehicle != null)
            {
                currentVehicle.turnLeft();
            }
            else
            {
                movementVector.x--;
            }
        }

        public virtual void setHeadPitch(double Pitch)
        {
            this.headPitch = Pitch;
        }
        public virtual double getHeadPitch()
        {
            return this.headPitch;
        }
        public virtual Vector3D getFrontVector()
        {
            return this.frontVector;
        }

        public virtual Vector3D getUpVector()
        {
            return this.upVector;
        }

        
    }
}
