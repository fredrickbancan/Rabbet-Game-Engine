namespace RabbetGameEngine
{
    /*This class is responasble for providing game loop timings so that the game runs
      at a consistant rate over multiple fps rates and refresh rates.*/
    public static class TicksAndFps
    {
        private static long currentFrameTime = 0;
        private static long lastFrameTime = 0;
        private static double deltaFrameTime;
        private static int ticksPerSecond;
        private static double msPerTick;
        private static long applicationTime; //last updated UTC time of application. When ticking game, needs to be ticked untill application catches up with real time.
        private static double percentToNextTick; //a decimal value between 0 and 1 which can be used as a percentage of progress towards next tick, usefull for interpolation.
        private static bool paused = false; //true when game is paused
        private static int frames;
        private static int framesPerSec;
        private static double timer;
        public static void init(int tps)
        {
            ticksPerSecond = tps;
            msPerTick = 1000D / (double)ticksPerSecond;
            lastFrameTime = getRealTimeMS();
            applicationTime = getRealTimeMS();
            currentFrameTime = getRealTimeMS();
        }

        /*called every frame*/
        public static void updateFPS()
        {
            /*updating FPS*/
            lastFrameTime = currentFrameTime;
            currentFrameTime = getRealTimeMS();
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

        /*runs the provided onTick function repeatidly untill application has synced with realtime*/
        public static void doOnTickUntillRealtimeSync(System.Action onTickFunc)//experimental
        {
            if(paused)
            {
                while ((applicationTime + msPerTick) < getRealTimeMS())
                {
                    applicationTime += (long)msPerTick;
                }
                return;
            }
            while((applicationTime + msPerTick) < getRealTimeMS())
            {
                onTickFunc();
                applicationTime += (long)msPerTick;
            }
            percentToNextTick = (double)(getRealTimeMS() - applicationTime) / msPerTick;
        }

        public static void pause()
        {
            paused = true;
        }

        public static void unPause()
        {
            paused = false;
        }

        public static float getNumOfTicksForSeconds(float seconds)
        {
            return (float) (seconds * ticksPerSecond);
        }

        public static float getPercentageToNextTick()
        {
            return (float)percentToNextTick;
        }

        public static long getRealTimeMS()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /*ticks per second*/
        public static int tps { get => ticksPerSecond; }

        /*seconds per tick*/
        public static float spt { get => 1F / (float)ticksPerSecond; }

        /*frames per second*/
        public static int fps { get => framesPerSec; }

    }
}
