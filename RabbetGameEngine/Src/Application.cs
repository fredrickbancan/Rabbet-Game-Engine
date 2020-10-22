using System;
using System.Drawing;
using System.Windows.Forms;

namespace RabbetGameEngine
{
    /*This class acts as the main entry point for the program. Contains static functions for
      Printing information to the console.*/

    class Application
    {
        static void Main(string[] args)
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            GameInstance game = new GameInstance(resolution.Width, resolution.Height, resolution.Width/2, resolution.Height/2, "Rabbet Game Engine version " + version);
            game.Run(); //Will start the OpenTk Game instance running. Eeach frame will call OnUpdateFrame and OnRenderFrame. I am using my own tickrate class to controll ticks (TicksAndFps.cs)  
            game.Dispose();
        }
        public static readonly string version = "0.0.7_id";

        #region Print functions
        public static void infoPrint(object s)
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
        public static void debugPrint(object s)
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
