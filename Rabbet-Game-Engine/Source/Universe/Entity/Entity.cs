using OpenTK.Mathematics;
using RabbetGameEngine.Models;
namespace RabbetGameEngine
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics is an entity.*/

    public class Entity : PositionalObject
    {
        protected Planet currentPlanet;
        protected EntityModel entityModel;
        protected bool hasModel = false;
        protected bool isFlying = false;
        private bool removalFlag = false;// true if this entity should be removed in the next tick
        protected int existedTicks = 0;//number of ticks this entity has existed for
        protected bool isPlayer = false;
        protected bool isProjectile = false;
        protected bool isLiving = false;
        public Entity(Planet planet) : base()
        {
            currentPlanet = planet;
            setYAccel(-gravity);
        }
        
        public Entity(Planet planet, Vector3 spawnPosition) : base(spawnPosition)
        {
            currentPlanet = planet;
            setYAccel(-gravity);
        }

        public override void onFrame()
        {
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
            //do last
            if (!hasDoneFirstUpdate)
            {
                hasDoneFirstUpdate = true;
            }
        }

        public virtual void setCurrentPlanet(Planet p)
        {
            this.currentPlanet = p;
        }

        //removes this entity from existance
        public virtual void ceaseToExist()
        {
            hasModel = false;
            entityModel = null;
            removalFlag = true;

            if(GameSettings.debugScreen)
            {
                currentPlanet.removeLabelFromObject(this);
            }
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
            return hasModel && entityModel != null;
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

        public virtual void onCollideWithEntity(Entity ent)
        {
            
        }

        public bool getIsPlayer()//returns true if this entityliving is a player
        {
            return isPlayer;
        }
        public bool getIsProjectile()
        {
            return isProjectile;
        }

        public bool getIsLiving()
        {
            return isLiving;
        }

    }
}
