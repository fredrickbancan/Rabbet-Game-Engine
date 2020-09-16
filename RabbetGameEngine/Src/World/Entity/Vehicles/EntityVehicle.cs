using OpenTK;
using System;

namespace RabbetGameEngine
{
    public class EntityVehicle : Entity
    {
        protected EntityLiving mountingEntity;
        protected Vector3 mountingOffset;
        protected Vector3 frontVector;//vector pointing to the direction the entity is facing
        protected Vector3 upVector;
        protected Vector3 movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        public static readonly float defaultTurnRate = 2.5F;//degrees per tick
        public static readonly float defaultDriveSpeed = 0.15F;
        protected float driveSpeed = defaultDriveSpeed;
        protected float turnRate = defaultTurnRate;
        protected bool playerDriving = false;
        public EntityVehicle() : base()
        {
            frontVector = new Vector3(0.0F, 0.0F, -1.0F);
            upVector = new Vector3(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3(0.0F, 0.0F, 0.0F);
            mountingOffset  = new Vector3(0.0F, 1.9F, 0.0F);
        }

        public EntityVehicle(Vector3 pos) : base(pos)
        {
            frontVector = new Vector3(0.0F, 0.0F, -1.0F);
            upVector = new Vector3(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3(0.0F, 0.0F, 0.0F);

            mountingOffset = new Vector3(0.0F, 1.9F, 0.0F);
        }


        public override void onTick()
        {
            base.onTick();//do first
            if (mountingEntity != null)
            {
                playerDriving = mountingEntity.getIsPlayer();
                if (mountingEntity.doingAction(Action.attack))
                {
                    onDriverAttack();
                }
                alignVectors();

                //setting movementVector.X to -1, 0 or 1 based on driver strafing actions
                movementVector.X = Convert.ToInt32(mountingEntity.doingAction(Action.strafeRight)) - Convert.ToInt32(mountingEntity.doingAction(Action.strafeLeft));

                //setting movementVector.Z to -1, 0 or 1 based on driver fowards/backwards actions
                movementVector.Z = Convert.ToInt32(mountingEntity.doingAction(Action.fowards)) - Convert.ToInt32(mountingEntity.doingAction(Action.backwards));


                moveByMovementVector();
            }
        }

        public override void postTick()
        {
            base.postTick();
            if (mountingEntity != null)
            {
                mountingEntity.setPosition(pos + mountingOffset);
                mountingEntity.setVelocity(velocity);
            }
        }

        public override void applyCollision(Vector3 direction, float overlap)
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
            frontVector.X = (float)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.Z = (float)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.Normalize();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected virtual void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            float walkSpeedModified = driveSpeed;

            if (!isGrounded) walkSpeedModified = 0.02F;//reduce movespeed when jumping or mid air 



            //change velocity based on movement
            //movement vector is a unit vector.
            if(movementVector.Length > 0)
            movementVector.Normalize();
            rotateYaw(movementVector.X * movementVector.Z * turnRate);//steer vehicle, if reversing then vehicle will turn opposite way
            velocity += frontVector * movementVector.Z * walkSpeedModified;//fowards and backwards movement

            movementVector *= 0;//reset movement vector
        }

        /*Called when an entity mounts this vehicle*/
        public virtual void setMountedEntity(EntityLiving entityMounting)
        {
            this.mountingEntity = entityMounting;
            if(mountingEntity == null)
            {
                playerDriving = false;
            }
        }

        /*Called when the driving entity attacks (for tank)*/
        public virtual void onDriverAttack()
        {

        }

        public virtual bool getIsplayerDriving()
        {
            return playerDriving;
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

        public virtual Vector3 getMountingOffset()
        {
            return mountingOffset;
        }

        /*May return null.*/
        public virtual EntityLiving getMountingEntity()
        {
            return mountingEntity;
        }
    }
}
