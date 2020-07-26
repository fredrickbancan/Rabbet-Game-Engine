using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo
{
    static class GameSettings
    {
        public static float fov = 80; //fov of player camera
        public static bool vsync = false;// DO NOT set to true for now, causes game loop speed to be limited by screen refresh rate.
    }
}
