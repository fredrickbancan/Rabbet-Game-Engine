using Coictus.FredsMath;
using Coictus.Models;

namespace Coictus
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics is an entity.*/

    public class Entity : PositionalObject
    {
        
        private double groundPlaneHeight = 0.0000D;
        protected World currentPlanet;
        protected EntityModel entityModel;
        protected bool hasModel = false;
        protected bool isFlying = false;
        protected bool isGrounded = false;
        private bool removalFlag = false;// true if this entity should be removed in the next tick
        protected int existedTicks = 0;//number of ticks this entity has existed for
        public Entity() : base()
        {
            setYAccel(-gravity);
        }
        
        public Entity(Vector3D spawnPosition) : base(spawnPosition)
        {
            setYAccel(-gravity);
        }

        /*Called every tick*/
        public virtual void onTick()
        {
            existedTicks++;
            if (existedTicks < 0) existedTicks = 0;//incase of int overflow
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (isFlying) { setYAccel(0); } else { setYAccel(-gravity); }
            if (velocity.y > 0) isGrounded = false;
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
        public override void postTickMovement()//Overriding in entity so we can apply different frictions on axis depending on state of entity.
        {
            if (!isGrounded)
            {
                velocity += acceleration;
                velocity *= (1 - airResistance);
            }
            else 
            {
                velocity += acceleration;
                velocity.x *= (1 - groundResistance);
                velocity.y *= (1 - airResistance);
                velocity.z *= (1 - groundResistance);
            }

            pos += velocity;

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //updating hitboxes 
            if (hasCollider && collider != null)
            {
                collider.onTick();
            }
        }

        
        
        public override void applyCollision(Vector3D direction, double overlap)
        {
            if (direction.y >= 0.35D)//if the entity is being collided from a generally upwards direction
            {
                isGrounded = true;//TODO: make so when the entity is NOT colliding from the bottom
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
            if (hasModel && entityModel != null)
            {
                entityModel.delete();
            }
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
        
    }
}
