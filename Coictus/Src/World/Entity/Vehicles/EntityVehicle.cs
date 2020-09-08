using OpenTK;
using System;

namespace Coictus
{
    public class EntityVehicle : Entity
    {
        protected EntityLiving mountingEntity;
        protected Vector3d mountingOffset;
        protected Vector3d frontVector;//vector pointing to the direction the entity is facing
        protected Vector3d upVector;
        protected Vector3d movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        public static readonly double defaultTurnRate = 2.5F;//degrees per tick
        public static readonly double defaultDriveSpeed = 0.15F;
        protected double driveSpeed = defaultDriveSpeed;
        protected double turnRate = defaultTurnRate;

        public EntityVehicle() : base()
        {
            frontVector = new Vector3d(0.0F, 0.0F, -1.0F);
            upVector = new Vector3d(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3d(0.0F, 0.0F, 0.0F);
            mountingOffset  = new Vector3d(0.0F, 1.9F, 0.0F);
        }

        public EntityVehicle(Vector3d pos) : base(pos)
        {
            frontVector = new Vector3d(0.0F, 0.0F, -1.0F);
            upVector = new Vector3d(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3d(0.0F, 0.0F, 0.0F);

            mountingOffset = new Vector3d(0.0F, 1.9F, 0.0F);
        }


        public override void onTick()
        {
            base.onTick();//do first
            if (mountingEntity != null)
            {
                alignVectors();

                moveByMovementVector();
            }
        }

        public override void postTickMovement()
        {
            base.postTickMovement();
            if (mountingEntity != null)
            {
                mountingEntity.setPosition(pos + mountingOffset);
                mountingEntity.setVelocity(velocity);
            }
        }

        public override void applyCollision(Vector3d direction, double overlap)
        {
            base.applyCollision(direction, overlap);
            if (mountingEntity != null)
            {
                mountingEntity.setPosition(pos + mountingOffset);
                mountingEntity.setVelocity(velocity);
            }
        }

        /*When called, aligns vectors according to the entities state and rotations.*/
        protected virtual void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.X = (double)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.Z = (double)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected virtual void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded) walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air 



            //change velocity based on movement
            //movement vector is a unit vector.
            movementVector.Normalize();
            rotateYaw(movementVector.X * movementVector.Z * turnRate);//steer vehicle, if reversing then vehicle will turn opposite way
            velocity += frontVector * movementVector.Z * walkSpeedModified;//fowards and backwards movement

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
            movementVector.Z++;
        }
        public virtual void driveBackwards()
        {
            movementVector.Z--;
        }

        public virtual void turnLeft()
        {
            movementVector.X--;
        }
        public virtual void turnRight()
        {
            movementVector.X++;
        }

        public virtual Vector3d getMountingOffset()
        {
            return mountingOffset;
        }
    }
}
