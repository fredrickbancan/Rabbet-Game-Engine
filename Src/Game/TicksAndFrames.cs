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
        private static long millInSec2FPS = (long)TimeSpan.FromSeconds(1).TotalMilliseconds / 2L;
        private static double[] frameTimes = new double[200];
        private static int frameTimeIndex = 0;
        public static void init(int tps)
        {
            ticksPerSecond = tps;
            msPerTick = 1000D / (double)ticksPerSecond;
            lastFrameTime = nanoTime();
            applicationTime = getRealTimeMills();
            currentFrameTime = nanoTime();
        }

        /*called every frame*/
        public static void updateFPS()
        {
            /*updating FPS*/
            lastFrameTime = currentFrameTime;
            currentFrameTime = nanoTime();
            deltaFrameTime = (currentFrameTime - lastFrameTime) / 1000000D;
            timer += deltaFrameTime;

            if(timer >= 1000D)
            {
                framesPerSec = frames;
                frames = 0;
                timer -= 1000D;
            }
            frames++;

            frameTimes[frameTimeIndex++] = deltaFrameTime;
            frameTimeIndex = frameTimeIndex % 200;
        }
        
        public static double[] getFrameTimes()
        {
            return frameTimes;
        }

        public static int getFrameIndex()
        {
            return frameTimeIndex;
        }

        public static double getAverageFrameTime()
        {
            double total = 0D;
            for(int i = 0; i < 200; i++)
            {
                total += frameTimes[i];
            }
            return total / 200D;
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
                if(getRealTimeMills() - applicationTime >= millInSec2FPS)
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
