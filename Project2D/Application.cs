using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib;
using static Raylib.Raylib;

namespace Project2D
{
    class Application
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            Raylib.Raylib.InitWindow(1440, 850, "Fredrick Math Library Tech Demo");

            game.Init();

            while (!WindowShouldClose())
            {
                game.Update();
                game.Draw();
            }

            game.Shutdown();

            Raylib.Raylib.CloseWindow();
        }
    }
}
