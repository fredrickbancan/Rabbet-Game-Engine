using Coictus.Models;
using OpenTK;

namespace Coictus
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics is an entity.*/

    public class Entity : PositionalObject
    {
        protected World currentPlanet;
        protected EntityModel entityModel;
        protected bool hasModel = false;
        protected bool isFlying = false;
        protected bool isGrounded = false;
        private bool removalFlag = false;// true if this entity should be removed in the next tick
        protected int existedTicks = 0;//number of ticks this entity has existed for
        protected bool isPlayer = false;
        public Entity() : base()
        {
            setYAccel(-gravity);
        }
        
        public Entity(Vector3 spawnPosition) : base(spawnPosition)
        {
            setYAccel(-gravity);
        }

        public override void onFrame()
        {
            if (hasModel && entityModel.exists())
            {
                entityModel.onFrame();
            }
            base.onFrame();
        }

        /*Called every tick*/
        public virtual void onTick()
        {
            existedTicks++;
            if (existedTicks < 0) existedTicks = 0;//incase of int overflow
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (isFlying) { setYAccel(0); } else { setYAccel(-gravity); }
            if (velocity.Y != 0) isGrounded = false;//simple detection of being on the ground. Very basic.
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

            

            //do last
            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }
        }

        /*Do this after manipulating any movement of this object*/
        public override void postTick()//Overriding in entity so we can apply different frictions on axis depending on state of entity.
        {
            if (!isGrounded)
            {
                velocity += acceleration;
                velocity *= (1 - airResistance);
            }
            else 
            {
                velocity += acceleration;
                velocity.X *= (1 - groundResistance);
                velocity.Y *= (1 - airResistance);
                velocity.Z *= (1 - groundResistance);
            }

            pos += velocity;

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //updating hitboxes 
            if (hasCollider && collider != null)
            {
                collider.onTick();
            }
        }

        /*This method can be called to update the collider of this entity manually.
          e.g, this is useful for correcting hitboxes after collisions so they are ready to be 
          rendered for debugging*/
        public virtual void tickUpdateCollider()
        {
            if (hasCollider && collider != null)
            {
                collider.onTick();
            }
        }
        
        public override void applyCollision(Vector3 direction, float overlap)
        {
            if (direction.Y >= 0.35D)//if the entity is being collided from a generally upwards direction
            {
                isGrounded = true;
            }
            base.applyCollision(direction, overlap);
        }

        public virtual bool getHasCollider()
        {
            return hasCollider;
        }

        public virtual ICollider getCollider()
        {
            return collider;
        }
        public virtual void setCurrentPlanet(World p)
        {
            this.currentPlanet = p;
        }

        //removes this entity from existance
        public virtual void ceaseToExist()
        {
            hasModel = false;
            entityModel = null;
            removalFlag = true;
        }

        public bool getIsMarkedForRemoval()
        {
            return removalFlag;
        }
        protected virtual void updateModel()
        {
            entityModel.onTick();
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

        public virtual bool getIsGrounded()
        {
            return isGrounded;
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
        public virtual bool getIsPlayer()//returns true if this entityliving is a player
        {
            return isPlayer;
        }

    }
}
