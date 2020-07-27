using FredrickTechDemo.FredsMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo
{
    class Vertex
    {
        public Vector3F pos;
        public ColourF colour;
        public Vertex(Vector3F pos, ColourF colour)
        {
            this.pos = pos;
            this.colour = colour;
        }
    }
}
