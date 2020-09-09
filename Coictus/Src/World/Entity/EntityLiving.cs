using OpenTK;
using System;
namespace Coictus
{
    public class EntityLiving : Entity
    {
        
        protected EntityVehicle currentVehicle;
        protected Vector3d frontVector;//vector pointing to the direction the entity is facing
        protected Vector3d upVector;
        protected Vector3d movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        protected bool isJumping = false;
        protected double headPitch; // Pitch of the living entity head
        public static readonly double defaultWalkSpeed = 0.1572F;
        protected double walkSpeed = defaultWalkSpeed;
        public EntityLiving() : base()
        {
            frontVector = new Vector3d(0.0F, 0.0F, -1.0F);
            upVector = new Vector3d(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3d(0.0F, 0.0F, 0.0F);
        }

        public EntityLiving(Vector3d pos) : base(pos)
        {
            frontVector = new Vector3d(0.0F, 0.0F, -1.0F);
            upVector = new Vector3d(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3d(0.0F, 0.0F, 0.0F);
        }

        public override void onTick()
        {
            base.onTick();//do first

            //prevent head from rotating up and down too much
            if(headPitch > 90F)
            {
                headPitch = 90F;
            }
            else if(headPitch < -90F)
            {
                headPitch = -90F;
            }

            alignVectors();

            moveByMovementVector(); 
        }

        /*When called, aligns vectors according to the entities state and rotations.*/
        protected virtual void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            if (isFlying)
            {
                frontVector.X = (double)(Math.Cos(MathUtil.radians(yaw)) * (double)(Math.Cos(MathUtil.radians(headPitch))));
                frontVector.Y = (double)Math.Sin(MathUtil.radians(headPitch));
                frontVector.Z = (double)(Math.Sin(MathUtil.radians(yaw)) * (double)(Math.Cos(MathUtil.radians(headPitch))));
            }
            else
            {
                frontVector.X = (double)(Math.Cos(MathUtil.radians(yaw)));
                frontVector.Y = (double)Math.Sin(MathUtil.radians(headPitch));
                frontVector.Z = (double)(Math.Sin(MathUtil.radians(yaw)));
            }
            frontVector.Normalize();
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
                    walkSpeedModified *= 0.1D;//reduce movespeed when flying as to not accellerate out of control
                }
                else
                {
                    walkSpeedModified *= 0.05D;//reduce movespeed when jumping or mid air 
                }
            }

            //change velocity based on movement
            //movement vector is a unit vector.
            if(movementVector.Length > 0)
            movementVector.Normalize();//normalize vector so player is same speed in any direction

            velocity.X += frontVector.X * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            if(isFlying)velocity.Y += frontVector.Y * movementVector.Z * walkSpeedModified;//fowards and backwards movement for flying
            velocity.Z += frontVector.Z * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            velocity += Vector3d.Normalize(Vector3d.Cross(frontVector, upVector)) * movementVector.X * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector

            if (isJumping)// if player jumping or flying up
            {
                velocity.Y += 0.32D;//jump 
                isJumping = false;
                isGrounded = false;
            }
        }

        public override void applyCollision(Vector3d direction, double overlap)
        {
            if (currentVehicle == null)
            {
                base.applyCollision(direction, overlap);
            }
        }

        public virtual void jump()
        {
            if (isGrounded && velocity.Y <= 0)
            {
                isJumping = true;
            }
        }

        /*When called, this entity will attempt to interact with an object, could be anything.
          For now this will only let this entity mount and dismount nearby vehicles.*/
        public virtual void interact()
        {
            if(currentPlanet != null)
            {
                if (currentVehicle != null)
                {
                    this.unmountVehicle();
                }
                else 
                {
                    foreach (Entity ent in currentPlanet.entities.Values)
                    {
                        if (Vector3d.Distance(ent.getPosition(), pos) < 4D)//if the entity is atleast within 4 units (meters)
                        {
                            if (ent is EntityVehicle vehicle)
                            {
                                this.mountVehicle(vehicle);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public virtual void unmountVehicle()
        {
            if (currentVehicle != null)
            {
                setVelocity(new Vector3d(0));
                currentVehicle.setMountedEntity(null);
                currentVehicle = null;
            }
        }

        public virtual void mountVehicle(EntityVehicle theVehicle)
        {
            this.pos = theVehicle.getPosition() + theVehicle.getMountingOffset();
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
                movementVector.Z++;
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
                movementVector.Z--;
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
                movementVector.X++;
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
                movementVector.X--;
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
        public virtual Vector3d getFrontVector()
        {
            return this.frontVector;
        }

        public virtual Vector3d getUpVector()
        {
            return this.upVector;
        }

        public override Vector3d getLerpPos()
        {
            if(currentVehicle != null)
            {
                return currentVehicle.getLerpPos() + currentVehicle.getMountingOffset();
            }
            return base.getLerpPos();
        }
    }
}
