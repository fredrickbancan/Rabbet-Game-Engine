using OpenTK;
using System;
namespace Coictus
{
    public class EntityLiving : Entity
    {
        
        protected EntityVehicle currentVehicle;
        protected Vector3d frontVector;//vector pointing to the direction the entity is facing
        protected Vector3d upVector;
        protected Vector3d movementVector;//a unit vector describing the direction of movement by this entity, e.g: walking
        protected double headPitch; // Pitch of the living entity head
        public static readonly double defaultWalkSpeed = 0.1572F;
        protected double walkSpeed = defaultWalkSpeed;
        protected bool isPlayer = false;

        /*array of actions being requested by player user. When an action is requested, the bool at the index of the action (enum) value
          is set to true. To detect if an action is requested, check the index. e.g: if(actions[Action.attack])*/
        protected bool[] actions = new bool[GameInstance.actionsCount];

        public EntityLiving() : base()
        {
            frontVector = new Vector3d(0.0D, 0.0D, -1.0D);
            upVector = new Vector3d(0.0D, 1.0D, 0.0D);
        }

        public EntityLiving(Vector3d pos) : base(pos)
        {
            frontVector = new Vector3d(0.0D, 0.0D, -1.0D);
            upVector = new Vector3d(0.0D, 1.0D, 0.0D);
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

        public override void postTick()
        {
            resetActions();//this must be done in post tick so other entities (namely vehicles) can read this entity's movement when they tick
            base.postTick();
        }
        /*When called, aligns vectors according to the entities state and rotations.*/
        protected virtual void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            if (isFlying)
            {
                frontVector.X = Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch));
                frontVector.Y = Math.Sin(MathUtil.radians(headPitch));
                frontVector.Z = Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch));
            }
            else
            {
                frontVector.X = Math.Cos(MathUtil.radians(yaw));
                frontVector.Y = Math.Sin(MathUtil.radians(headPitch));
                frontVector.Z = Math.Sin(MathUtil.radians(yaw));
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
            //setting movementVector.X to -1, 0 or 1 based on strafing actions
            movementVector.X = Convert.ToDouble(doingAction(Action.strafeRight)) - Convert.ToDouble(doingAction(Action.strafeLeft));

            //setting movementVector.Z to -1, 0 or 1 based on fowards/backwards actions
            movementVector.Z = Convert.ToDouble(doingAction(Action.fowards)) - Convert.ToDouble(doingAction(Action.backwards));

            if (movementVector != Vector3d.Zero)
            {
                movementVector.Normalize();//normalize vector so player is same speed in any direction
            }

            velocity.X += frontVector.X * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            if(isFlying)velocity.Y += frontVector.Y * movementVector.Z * walkSpeedModified;//fowards and backwards movement for flying
            velocity.Z += frontVector.Z * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            velocity += Vector3d.Normalize(Vector3d.Cross(frontVector, upVector)) * movementVector.X * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector

            if (doingAction(Action.jump) && isGrounded)
            {
                velocity.Y += 0.32D;//jump here
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
            addAction(Action.jump);
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
                setVelocity(Vector3d.Zero);
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
            addAction(Action.fowards);
        }
        public virtual void walkBackwards()
        {
            addAction(Action.backwards);
        }
        public virtual void strafeRight()
        {
            addAction(Action.strafeRight);
        }
        public virtual void strafeLeft()
        {
            addAction(Action.strafeLeft);
        }

        public virtual bool getIsPlayer()//returns true if this entityliving is a player
        {
            return isPlayer;
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

        /*returns true if entity is doing the specified action*/
        public bool doingAction(Action act)
        {
            return actions[(int)act];
        }

        /*used for adding a entity action*/
        public void addAction(Action act)
        {
            actions[(int)act] = true;
        }

        /*should be called every update at the end of onTick() to reset the entity actions.*/
        public void resetActions()
        {
            for (int i = 0; i < GameInstance.actionsCount; i++)
            {
                actions[i] = false;
            }
        }
    }
}
