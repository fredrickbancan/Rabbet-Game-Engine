using FredrickTechDemo.FredsMath;
namespace FredrickTechDemo
{
    /*Base class for every entity in the game, Anything with movement, vectors,
      physics, inventory and/or non-batched draw call is an entity.*/
    class Entity
    {
        protected Vector3F pos;
        protected float pitch, yaw, roll = 0;

        public Entity()
        {
            this.pos = new Vector3F();
        }
        
        public Entity(Vector3F spawnPosition)
        {
            this.pos = spawnPosition;
        }

        public Vector3F getPosition()
        {
            return this.pos;
        }

    }
}
