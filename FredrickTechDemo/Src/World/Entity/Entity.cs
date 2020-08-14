using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;

namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics is an entity.*/

    public class Entity : PositionalObject
    {
        protected ICollider collider = null;
        protected bool hasCollider = false;
        private double groundPlaneHeight = 0.0000D;
        protected Planet currentPlanet;
        protected EntityModel entityModel;
        public static readonly double defaultAirResistance = 0.03572F;
        public static readonly double defaultGroundResistance = 0.72F;
        public static readonly double defaultGravity = 0.03572F;
        protected bool hasModel = false;
        protected double resistance = defaultAirResistance;
        protected double gravity = defaultGravity;
        protected bool isFlying = false;
        protected bool isGrounded = false;
        private bool removalFlag = false;// true if this entity should be removed in the next tick
        protected int existedTicks = 0;//number of ticks this entity has existed for
        public Entity() : base()
        {
        }
        
        public Entity(Vector3D spawnPosition) : base(spawnPosition)
        {
        }

        /*Called every tick*/
        public virtual void onTick()
        {
            existedTicks++;
            if (existedTicks < 0) existedTicks = 0;//incase of int overflow
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //resist velocity differently depending on state
            if (isGrounded) { resistance = defaultGroundResistance; } else { resistance = defaultAirResistance; }
            if (posY <= groundPlaneHeight) { isGrounded = true; } else { isGrounded = false; }//basic ground level collision detection, in this case there is a ground plane collider at 0.0000D

            base.preTickMovement();//do before movement
            /*decelerate velocity by air resistance (not accurate to real life)*/
            scaleXVelocity(1 - resistance);
            scaleYVelocity(1 - defaultAirResistance);
            scaleZVelocity(1 - resistance);

            //decrease entity y velocity by gravity, will not spiral out of control due to terminal velocity.
            if (!isFlying && !isGrounded) { addYVelocity(-gravity); }

            //to prevent the entity from going through  the ground plane, if the next position increased by velocity will place the entity
            //below the ground plane, it will be given a value of 0.0000D -pos.y, so when position is increased by velocity, they cancel out resulting
            //in perfect 0, which stops the entity perfectly on the ground plane.
            if (getPredictedNextTickPos().y < groundPlaneHeight)
            {
                setYVelocity(groundPlaneHeight - posY);
                isGrounded = true;
            }
            base.postTickMovement();//do after movement 
            
            if (hasModel)
            {
                if (entityModel.exists())
                {
                    updateModel();
                }
                else
                {
                    hasModel = false;
                }
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //updating hitboxes below
            if(hasCollider && collider != null)
            {
                collider.onTick();
            }



            //do last
            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }
        }

        /*used for setting the collider of this entity by classes which extend entity.*/
        protected virtual void setCollider(ICollider collider)
        {
            this.collider = collider;
            if(this.collider != null)
            {
                this.hasCollider = true;
            }
        }

        public virtual bool getHasCollider()
        {
            return hasCollider;
        }

        public virtual ICollider getCollider()
        {
            return collider;
        }
        public virtual void setCurrentPlanet(Planet p)
        {
            this.currentPlanet = p;
        }

        //removes this entity from existance
        public virtual void ceaseToExist()
        {
            entityModel.delete();
            removalFlag = true;
        }

        public bool getIsMarkedForRemoval()
        {
            return removalFlag;
        }
        protected virtual void updateModel()
        {
            entityModel.updateModel();
        }
        public virtual bool getHasModel()
        {
            return hasModel && entityModel != null && entityModel.exists();
        }

        public virtual EntityModel getEntityModel()
        {
            return this.entityModel;
        }
        
        public virtual bool getIsFlying()
        {
            return isFlying;
        }

        public virtual void setFlying(bool flag)
        {
            this.isFlying = flag;
        }

        public virtual void toggleFlying()
        {
            if(!isFlying)
            {
                isFlying = true;
            }
            else
            {
                isFlying = false;
            }
        }
        
    }
}
