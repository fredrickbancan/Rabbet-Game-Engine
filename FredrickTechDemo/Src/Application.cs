using System;

namespace FredrickTechDemo
{
    /*This class acts as the main entry point for the program. Contains static functions for
      Printing information to the console.*/

    class Application
    {
        static void Main(string[] args)
        {
            
            GameInstance game = new GameInstance(1920, 1080, "Fredrick Math Library OpenGL Tech Demo (Using OpenTk library, excluding maths)");
            game.init();
            
            game.Run(); //Will start the OpenTk Game instance running. Eeach frame will call OnUpdateFrame and OnRenderFrame. I am using my own tickrate class to controll ticks (TicksAndFps.cs)  
        }
        #region Print functions
        public static void say(object s)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Info: " + s);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void warn(object s)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Warning: " + s);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void error(object s)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Error!: " + s);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void debug(object s)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Debug: " + s);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
        }
        #endregion

    }
}
