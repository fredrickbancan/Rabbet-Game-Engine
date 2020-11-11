using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Diagnostics;

namespace RabbetGameEngine
{

    //TODO: Impliment some sort of cross platform error screen to avoid crashes and properly shut down game.
    class Application
    {
        private static Process process;
        static void Main(string[] args)
        {
            process = Process.GetCurrentProcess();   
           
            try
            {
                NativeWindowSettings w = new NativeWindowSettings();
                w.StartFocused = true;
                w.StartVisible = true;
                w.Profile = ContextProfile.Core;

                GameWindowSettings g = new GameWindowSettings();
                GameInstance game = new GameInstance(g, w);
                game.Run();
            }
            catch(Exception e)
            {
                Application.error("Failed to run game, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            finally
            {
                process.Dispose();
            }
        }
        public static readonly string version = "0.1.0_indev";
        public static readonly string applicationName = "RabbetGameEngine " + version;

        /// <summary>
        /// returns the number of bytes of memory used by this application
        /// </summary>
        public static long getMemoryUsageBytes()
        {
            return (process = Process.GetCurrentProcess()).PrivateMemorySize64;
        }


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
