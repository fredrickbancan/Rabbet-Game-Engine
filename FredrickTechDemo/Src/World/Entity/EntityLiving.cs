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
        public static readonly double defaultWalkSpeed = 0.1572F;
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
                frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)) * (double)(Math.Cos(MathUtil.radians(headPitch))));
                frontVector.y = (double)Math.Sin(MathUtil.radians(headPitch));
                frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)) * (double)(Math.Cos(MathUtil.radians(headPitch))));
            }
            else
            {
                frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)));
                frontVector.y = (double)Math.Sin(MathUtil.radians(headPitch));
                frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)));
            }
            frontVector.normalize();
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
            movementVector.normalize();//normalize vector so player is same speed in any direction

            velocity.x += frontVector.x * movementVector.z * walkSpeedModified;//fowards and backwards movement
            if(isFlying)velocity.y += frontVector.y * movementVector.z * walkSpeedModified;//fowards and backwards movement for flying
            velocity.z += frontVector.z * movementVector.z * walkSpeedModified;//fowards and backwards movement
            velocity += Vector3D.normalize(Vector3D.cross(frontVector, upVector)) * movementVector.x * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector

            if (isJumping)// if player jumping or flying up
            {
                velocity.y += 0.32D;//jump 
                isJumping = false;
                isGrounded = false;
            }
        }

        public override void applyCollision(Vector3D direction, double overlap)
        {
            if (currentVehicle == null)
            {
                base.applyCollision(direction, overlap);
            }
        }

        public virtual void jump()
        {
            if (isGrounded && velocity.y <= 0)
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
                        if (Vector3D.distance(ent.getPosition(), pos) < 4D)//if the entity is atleast within 4 units (meters)
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
                setVelocity(new Vector3D(0));
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

        public override Vector3D getLerpPos()
        {
            if(currentVehicle != null)
            {
                return currentVehicle.getLerpPos() + currentVehicle.getMountingOffset();
            }
            return base.getLerpPos();
        }
    }
}
