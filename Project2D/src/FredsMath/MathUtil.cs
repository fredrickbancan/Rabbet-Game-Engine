using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredsMath
{
    static class MathUtil
    {
        public static float normalize(float min, float max, float val)//returns a float in between 0 and 1 representing the percentage that VAL is from Min to Max e.g: min of 0 and max of 100 with value 50 will return 0.5
        {
            return (val - min/*amount between min and the value*/) / (max - min/*difference between min and max*/);
        }
    }
}
