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
        protected Vector3 pos;
        protected float pitch, yaw, roll = 0;

        public Entity()
        {
            this.pos = new Vector3();
        }
        
        public Entity(Vector3 spawnPosition)
        {
            this.pos = spawnPosition;
        }

        public Vector3 getPosition()
        {
            return this.pos;
        }

    }
}
