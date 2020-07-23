using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib;
using static Raylib.Raylib;

namespace Project2D
{
    class Game
    {
        Raylib.Image logo;
        Raylib.Texture2D texture;
        TicksAndFps tickFps;

        public Game()
        {
            tickFps = new TicksAndFps(30.0D);
        }

        public void Init()
        {
            tickFps.init();
            //logo = LoadImage("..\\Images\\aie-logo-dark.jpg");
            //logo = LoadImage(@"..\Images\aie-logo-dark.jpg");
            logo = Raylib.Raylib.LoadImage("../Images/aie-logo-dark.jpg");
            texture = Raylib.Raylib.LoadTextureFromImage(logo);
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            tickFps.update();

            for(TicksAndFps.itteratior = 0; TicksAndFps.itteratior < tickFps.getTicksElapsed(); TicksAndFps.itteratior++)//for each tick that has elapsed since the start of last update, run the games logic enough times to catch up. 
            {
                Console.WriteLine("Tick!");
                onTick();
            }
        }
        private void onTick()
        {
            //insert game logic here
        }
        public void Draw()
        {
            Raylib.Raylib.BeginDrawing();

            Raylib.Raylib.ClearBackground(Color.WHITE);

            Raylib.Raylib.DrawText(tickFps.getFps().ToString(), 10, 10, 14, Color.RED);

            Raylib.Raylib.DrawTexture(texture, GetScreenWidth() / 2 - texture.width / 2, GetScreenHeight() / 2 - texture.height / 2, Color.WHITE);

            Raylib.Raylib.EndDrawing();
        }

    }
}
