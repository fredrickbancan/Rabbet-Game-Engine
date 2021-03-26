using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Diagnostics;

namespace RabbetGameEngine
{
    //TODO: Impliment a more streamlined publishing routine and create an installer
    //TODO: Impliment some sort of cross platform error screen to avoid crashes and properly shut down game.
    class Application
    {
        public static readonly string version = "0.1.2_indev";
        public static readonly string applicationName = "RabbetGameEngine " + version;
        private static long ramUsageBytes = 0L;

        public static long ramUsageInBytes { get => ramUsageBytes; }

        private static TickTimer ramUsageTimer;
        static void Main(string[] args)
        {
            try
            {
                NativeWindowSettings w = new NativeWindowSettings();
                w.StartFocused = true;
                w.StartVisible = true;
                w.Profile = ContextProfile.Core;
                GameWindowSettings g = new GameWindowSettings();
                GameInstance game = new GameInstance(g, w);
                ramUsageTimer = new TickTimer(2.48F, true, false);
                game.Run();
            }
            catch(Exception e)
            {
                Application.error("Failed to run game, Exception: " + e.Message + "\nStack Trace: " + e.StackTrace);
            }
            
        }
        
        public static void updateRamUsage()
        {
            ramUsageTimer.update();
            if(ramUsageTimer.triggered)
            {
                ramUsageBytes = (Process.GetCurrentProcess()).PrivateMemorySize64;
            }
        }

        public static void checkGLErrors()
        {
            switch(GL.GetError())
            {
                case ErrorCode.NoError:
                    break;
                case ErrorCode.InvalidEnum:
                    error("GL invalid enum");
                    break;
                case ErrorCode.InvalidValue:
                    error("GL invalid value");
                    break;
                case ErrorCode.InvalidOperation:
                    error("GL invalid operation");
                    break;
                case ErrorCode.StackOverflow:
                    error("GL stack overflow");
                    break;
                case ErrorCode.StackUnderflow:
                    error("GL stack underflow");
                    break;
                case ErrorCode.OutOfMemory:
                    error("GL Out of Memory");
                    break;
                case ErrorCode.ContextLost:
                    error("GL context lost");
                    break;
                case ErrorCode.InvalidFramebufferOperation:
                    error("GL invalid Framebuffer Operation");
                    break;
            }
        }

        public static string getAppTimeStamp()
        {
            string s = applicationName + " " +  (DateTime.Now.Day < 10 ? "0" : "") + DateTime.Now.Day + "-" + (DateTime.Now.Month < 10 ? "0" : "") + DateTime.Now.Month + "-" + DateTime.Now.Year + "_" + (DateTime.Now.Hour < 10 ? "0" : "") + DateTime.Now.Hour + "-" + (DateTime.Now.Minute < 10 ? "0" : "") + DateTime.Now.Minute + "-" + (DateTime.Now.Second < 10 ? "0" : "") + DateTime.Now.Second + "-" + DateTime.Now.Millisecond;
            return s;
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
            GameInstance.get.onError();
            Console.ReadKey();
        }
        public static void debugPrint(object s)
        {
            Console.WriteLine("Debug: " + s);
        }
        #endregion
    }
}
