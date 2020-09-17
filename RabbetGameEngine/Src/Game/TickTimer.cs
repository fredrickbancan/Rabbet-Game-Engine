namespace RabbetGameEngine
{
    /*A timer which can be initialized with a certain time interval. 
      After updating each tick, if the time interval worth of ticks has passed,
      the timer can be used to run a specific function or block of code.*/
    public class TickTimer
    {
        private readonly int activationTicks;//number of ticks required to be reached before this timer enables an activation.
        private int currentTicks;//number of ticks this timer has been updated in. Resets to 0 after an activation.
        private bool running = false;//set to true when this timer is started. False when this timer is paused.
        private bool activated = false;//set to true when this timer eaches its time interval worth of ticks.

        public TickTimer(float seconds, bool startTriggered = false, bool startPaused = false)
        {
            activationTicks = (int)TicksAndFps.getNumOfTicksForSeconds(seconds);
            currentTicks = startTriggered ? activationTicks : 0;
            running = !startPaused;
        }

        public void start()
        {
            running = true;
        }

        public void pause(bool triggerOnResume = false)
        {
            running = false;
            currentTicks = triggerOnResume ? activationTicks : currentTicks;
        }

        /*should be called every tick BEFORE any dependant code. returns true if timer is triggered.
          do not call onTick() with doFunctionAtIntervalIfTrueOnTick() on the same timer. This will cause a 
          double update.*/
        public bool onTick()
        {
            if (!running)
            {
                return false;
            }

            activated = false;//activated should be reset to false after each tick
            currentTicks++;
            if (currentTicks >= activationTicks)
            {
                currentTicks = 0;
                activated = true;
            }
            return activated;
        }

        /*should be called instead of onTick() with a pointer to a function to be called.
          This will cause the function to be called every time interval passing, as long as 
          the determinator parameter is true. The provided function will never be called in
          a shorter interval than the set interval of this tick timer. 
          Returns: true if the function was called.*/
        public bool doFunctionAtIntervalOnTick(System.Action function, bool determinator = true)
        {
            if (!running && !determinator)//do nothing if timer not running and determinator is false
            {
                return false;
            }

            currentTicks++;

            if (currentTicks >= activationTicks)
            {
                if (determinator)
                {
                    currentTicks = 0;
                    activated = true;
                }
                else
                {
                    running = false;
                    activated = false;
                }
            }
            else
            {
                activated = false;//activated should be reset to false after each tick
            }

            if (activated)
            {
                function();
            }

            //this allows the timer to count ticks up untill the interval, so next time determinator is true, the function will be called instantly.
            running = !activated || determinator;//set running to false if activated or if determinator is also false.
            

            return activated && determinator;
        }

        /*returns true if this timer has reached its interval in the previous onTick() update*/
        public bool triggered { get => activated; }
    }
}
