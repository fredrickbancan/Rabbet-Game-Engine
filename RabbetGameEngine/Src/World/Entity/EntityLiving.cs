using OpenTK;
using System;
namespace RabbetGameEngine
{
    /*List of all possible player/EntityLiving actions requestable via keyboard and mouse input or game logic*/
    public enum Action
    {
        none,
        fowards,
        strafeLeft,
        backwards,
        strafeRight,
        jump,
        attack,
        duck,
        sprint,
        interact
    };

    public class EntityLiving : Entity
    {
        
        protected EntityVehicle currentVehicle;
        protected Vector3 frontVector;//vector pointing to the direction the entity is facing
        protected Vector3 upVector;
        protected Vector3 movementVector;//a unit vector describing the direction of movement by this entity, e.g: walking
        protected float headPitch; // Pitch of the living entity head
        public static readonly float defaultWalkSpeed = 0.1572F;
        protected float walkSpeed = defaultWalkSpeed;
        

        /*array of actions being requested by player user. When an action is requested, the bool at the index of the action (enum) value
          is set to true. To detect if an action is requested, check the index. e.g: if(actions[Action.attack])*/
        protected bool[] actions = new bool[GameInstance.actionsCount];

        public EntityLiving() : base()
        {
            frontVector = new Vector3(0.0F, 0.0F, -1.0F);
            upVector = new Vector3(0.0F, 1.0F, 0.0F);
        }

        public EntityLiving(Vector3 pos) : base(pos)
        {
            frontVector = new Vector3(0.0F, 0.0F, -1.0F);
            upVector = new Vector3(0.0F, 1.0F, 0.0F);
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
                frontVector.X = (float)(Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
                frontVector.Y = (float)(Math.Sin(MathUtil.radians(headPitch)));
                frontVector.Z = (float)(Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
            }
            else
            {
                frontVector.X = (float)(Math.Cos(MathUtil.radians(yaw)));
                frontVector.Y = (float)(Math.Sin(MathUtil.radians(headPitch)));
                frontVector.Z = (float)(Math.Sin(MathUtil.radians(yaw)));
            }
            frontVector.Normalize();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected virtual void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            float walkSpeedModified = walkSpeed;

            if (!isGrounded)
            {
                if (isFlying)
                {
                    walkSpeedModified *= 0.1F;//reduce movespeed when flying as to not accellerate out of control
                }
                else
                {
                    walkSpeedModified *= 0.05F;//reduce movespeed when jumping or mid air 
                }
            }
            //setting movementVector.X to -1, 0 or 1 based on strafing actions
            movementVector.X = Convert.ToInt32(doingAction(Action.strafeRight)) - Convert.ToInt32(doingAction(Action.strafeLeft));

            //setting movementVector.Z to -1, 0 or 1 based on fowards/backwards actions
            movementVector.Z = Convert.ToInt32(doingAction(Action.fowards)) - Convert.ToInt32(doingAction(Action.backwards));

            if (movementVector != Vector3.Zero)
            {
                movementVector.Normalize();//normalize vector so player is same speed in any direction
            }

            velocity.X += frontVector.X * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            if(isFlying)velocity.Y += frontVector.Y * movementVector.Z * walkSpeedModified;//fowards and backwards movement for flying
            velocity.Z += frontVector.Z * movementVector.Z * walkSpeedModified;//fowards and backwards movement
            velocity += Vector3.Normalize(Vector3.Cross(frontVector, upVector)) * movementVector.X * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector

            if (doingAction(Action.jump) && isGrounded)
            {
                velocity.Y += 0.32F;//jump here
                isGrounded = false;
            }
        }

        public override void applyCollision(Vector3 direction, float overlap)
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
                        if (Vector3.Distance(ent.getPosition(), pos) < 4D)//if the entity is atleast within 4 units (meters)
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
                setVelocity(Vector3.Zero);
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

        public virtual void setHeadPitch(float Pitch)
        {
            this.headPitch = Pitch;
        }
        public virtual float getHeadPitch()
        {
            return this.headPitch;
        }
        public virtual Vector3 getFrontVector()
        {
            return this.frontVector;
        }

        public virtual Vector3 getUpVector()
        {
            return this.upVector;
        }

        public override Vector3 getLerpPos()
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
