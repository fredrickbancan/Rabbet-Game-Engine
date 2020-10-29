using OpenTK.Windowing.Desktop;
using System;
namespace RabbetGameEngine
{
    /*This class acts as the main entry point for the program. Contains static functions for
      Printing information to the console.*/

    class Application
    {
        static void Main(string[] args)
        {
            GameInstance game = new GameInstance(GameWindowSettings.Default, NativeWindowSettings.Default);
            game.Run();
        }
        public static readonly string version = "0.0.8_indev";
        public static readonly string applicationName = "RabbetGameEngine " + version;

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

        /// <summary>
        /// prints the error object provided and then waits for input. afterwards closes application.
        /// </summary>
        /// <param name="s">Error message</param>
        public static void error(object s)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Error!: " + s);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            GameInstance.get.Close();
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
