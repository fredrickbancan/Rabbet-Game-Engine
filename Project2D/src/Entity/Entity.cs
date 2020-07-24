using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredsMath;
namespace FredrickTechDemo
{
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
