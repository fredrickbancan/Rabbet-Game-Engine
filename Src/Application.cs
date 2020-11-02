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
            NativeWindowSettings w = new NativeWindowSettings();
            w.StartFocused = true;
            w.StartVisible = true;

            GameWindowSettings g = new GameWindowSettings();
            GameInstance game = new GameInstance(g, w);
            try
            {

            game.Run();
            }
            catch(Exception e)
            {
                Application.error("Failed to run game, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
        }
        public static readonly string version = "0.1.0_indev";
        public static readonly string applicationName = "RabbetGameEngine " + version;

        #region Print functions
        public static void infoPrint(object s)
        {
            Console.WriteLine("Info: " + s);
        }

        public static void warn(object s)
        {
            Console.WriteLine("Warning: " + s);
        }

        /// <summary>
        /// prints the error object provided and then waits for input. afterwards closes application.
        /// </summary>
        /// <param name="s">Error message</param>
        public static void error(object s)
        {
            Console.WriteLine("Error!: " + s);
            Console.ReadKey();
        }
        public static void debugPrint(object s)
        {
            Console.WriteLine("Debug: " + s);
        }
        #endregion
    }
}
