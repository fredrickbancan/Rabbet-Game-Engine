using System;
using System.Diagnostics;

namespace RabbetGameEngine
{
    /*This class is responasble for providing game loop timings so that the game runs
      at a consistant rate over multiple fps rates and refresh rates.*/
    public static class TicksAndFrames
    {
        private static long currentFrameTime = 0;
        private static long lastFrameTime = 0;
        private static double deltaFrameTime;
        private static int ticksPerSecond;
        private static double msPerTick;
        private static long applicationTime; //last updated UTC time of application. When ticking game, needs to be ticked untill application catches up with real time.
        private static double percentToNextTick; //a decimal value between 0 and 1 which can be used as a percentage of progress towards next tick, usefull for interpolation.
        private static int frames;
        private static int framesPerSec;
        private static double timer;
        private static long millInSec5FPS = (long)TimeSpan.FromSeconds(1).TotalMilliseconds / 5;
        public static void init(int tps)
        {
            ticksPerSecond = tps;
            msPerTick = 1000D / (double)ticksPerSecond;
            lastFrameTime = getRealTimeMills();
            applicationTime = getRealTimeMills();
            currentFrameTime = getRealTimeMills();
        }

        /*called every frame*/
        public static void updateFPS()
        {
            /*updating FPS*/
            lastFrameTime = currentFrameTime;
            currentFrameTime = getRealTimeMills();
            deltaFrameTime = (currentFrameTime - lastFrameTime) / 1000D;
            timer += deltaFrameTime;

            if(timer >= 1D)
            {
                framesPerSec = frames;
                frames = 0;
                timer -= 1D;
            }
            frames++;
            
        }
        
        /// <summary>
        /// runs the provided onTick function repeatidly untill application has synced with realtime
        /// minimum frame rate is 5 FPS.
        /// </summary>
        /// <param name="onTickFunc">Reference to the onTick() function to be called</param>
        public static void doOnTickUntillRealtimeSync(System.Action onTickFunc)
        {
            while((applicationTime + msPerTick) < getRealTimeMills())
            {
                onTickFunc();
                applicationTime += (long)msPerTick;
                if(getRealTimeMills() - applicationTime >= millInSec5FPS)
                {
                    applicationTime = getRealTimeMills() - (long)msPerTick;
                    break;
                }
            }
            if(!GameInstance.paused)
            percentToNextTick = (double)(getRealTimeMills() - applicationTime) / msPerTick;
        }

        public static float getNumOfTicksForSeconds(float seconds)
        {
            return (float) (seconds * ticksPerSecond);
        }

        public static float getPercentageToNextTick()
        {
            return (float)percentToNextTick;
        }

        public static long getRealTimeMills()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// returns a nano second time stamp for comparing processes
        /// </summary>
        /// <returns>A nano second time stamp for comparing processes</returns>
        public static long nanoTime()
        {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        /*ticks per second*/
        public static int tps { get => ticksPerSecond; }

        /*seconds per tick*/
        public static float spt { get => 1F / (float)ticksPerSecond; }

        /*frames per second*/
        public static int fps { get => framesPerSec; }

    }
}
