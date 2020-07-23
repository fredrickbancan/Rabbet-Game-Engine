using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo
{
    class TicksAndFps
    {
        Stopwatch stopwatch = new Stopwatch();
        public static int itteratior;//a static int dedicated to being used in a for loop to itterate the game to catch up after lag.
        private long currentTime = 0;
        private long lastTime = 0;
        private double timer = 0;
        private int fps = 1;
        private int frames;
        private double deltaTime = 0.005D;
        private double ticksPerSecond = 0;
        private int ticksElapsed = 0;
        private double timePerTick;
        private double partialTicks;
        private double partialTicksIncreasing;
        

        public TicksAndFps(double ticksPerSecond)
        {
            this.ticksPerSecond = ticksPerSecond;
            timePerTick = 1 / ticksPerSecond;
        }

        public void init()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;
        }

        public void update()
        {
            /*updating FPS*/
            lastTime = currentTime;
            currentTime = stopwatch.ElapsedMilliseconds;
            deltaTime = (currentTime - lastTime) / 1000.0f;
            timer += deltaTime;
            if (timer >= 1)
            {
                fps = frames;
                frames = 0;
                timer -= 1;
            }
            frames++;

            /*updating Ticks*/

            /*resets tick counts if atleast 1 tick time has passed. Update() must only be called after using ticksElapsed to tick the game. This way we can
	          access the accumulated tick value since the last update before it resets.*/
            if (ticksElapsed > 0)
            {
                ticksElapsed = 0;
                partialTicks = 0;
            }


            /*increase partialTicks by the time passed divided by time per tick. Time per tick is calulcated in constructor and is a constant.
            In other words, How many "time per ticks"'s has passed since the last update? Each time the time of one tick is passed (30 tps is 0.0333333 seconds)
            parialTicks is increased by 1.*/
            partialTicks += deltaTime / timePerTick;
            partialTicksIncreasing += deltaTime / timePerTick;
            /*ticksElapsed is equal to the number of times the passed time has reached the full duration of one tick (at 30 ticks per second, thats 0.0333333)*/
            ticksElapsed = (int)partialTicks;

            /*limit ticks elapsed to 10 so the game does not speed out of control in some circumstances*/
            if (ticksElapsed > 10)
            {
                ticksElapsed = 10;
            }
        }

        public int getTicksElapsed()
        {
            return this.ticksElapsed;
        }
        public double getPartialTicks()
        {
            return this.partialTicks;
        }

        public int getFps()
        {
            return fps;
        }
    }
}
