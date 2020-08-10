using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    public class EntityVehicle : Entity
    {
        protected EntityLiving mountingEntity;
        protected Vector3D mountingOffset;
        protected Vector3D frontVector;//vector pointing to the direction the entity is facing
        protected Vector3D upVector;
        protected Vector3D movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        public static readonly double defaultTurnRate = 15.0F;
        public static readonly double defaultDriveSpeed = 0.3572F;
        protected double driveSpeed = defaultDriveSpeed;
        protected double turnRate = defaultTurnRate;

        public EntityVehicle() : base()
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);
            mountingOffset = this.pos;
        }

        public EntityVehicle(Vector3D pos) : base(pos)
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);

            mountingOffset = this.pos;
        }

        public override void onTick()
        {
            base.onTick();//do first
            mountingEntity.setPosition(mountingOffset);
            alignVectors();

            moveByMovementVector();
        }

        /*When called, aligns vectors according to the entities state and rotations.*/
        protected virtual void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected virtual void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded) walkSpeedModified = 0.0072D; else walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air and reduce movespeed when flying as to not accellerate out of control

            if(movementVector.Magnitude() > 0)
            {
                rotateYaw(movementVector.x * turnRate);//steer vehicle
            }

            //change velocity based on movement
            //movement vector is a unit vector.
            velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement

            movementVector *= 0;//reset movement vector
        }

        /*Called when an entity mounts this vehicle*/
        public virtual void setMountedEntity(EntityLiving entityMounting)
        {
            this.mountingEntity = entityMounting;
        }

        /*Called when player is mounting this vehicle and left clicks (for tank)*/
        public virtual void onLeftClick()
        {

        }

        public virtual void driveFowards()
        {
            movementVector.z++;
        }
        public virtual void driveBackwards()
        {
            movementVector.z--;
        }

        public virtual void turnLeft()
        {
            movementVector.x--;
        }
        public virtual void turnRight()
        {
            movementVector.x++;
        }

        public virtual Vector3D getMountingPos()
        {
            return mountingOffset;
        }
    }
}
