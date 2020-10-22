using System;
using System.Collections.Generic;

namespace RabbetGameEngine.Debugging
{
    /// <summary>
    /// This class will be responsable for debugging, measuring and testing performance of the subsystems in this program.
    /// </summary>
    public static class Profiler
    {
        private static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();

        /// <summary>
        /// Calling this function with a name will either create and begin a profile or end an existing running profile, or run an existing non running profile.
        /// Apon ending a profile the time difference between starting and ending will be displayed. This allows us to measure how long different things take.
        /// </summary>
        /// <param name="profileName">Name of profile to begin/end </param>
        public static void beginEndProfile(string profileName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }

            if (profiles.TryGetValue(profileName, out Profile foundProfile))
            {
                if (foundProfile.hasEnded)
                {
                    foundProfile.begin();
                }
                else
                {
                    foundProfile.end();
                }
            }
            else
            {
                profiles.Add(profileName, new Profile(profileName).begin());
            }
        }

        /// <summary>
        /// Returns the average used time in ms for the requested profile per tick. returns -1 if profile isnt found
        /// </summary>
        /// <param name="profileName">Name of profile to get averages for</param>
        /// <returns></returns>
        public static float getAverageForProfile(string profileName)
        {
            if (!GameSettings.debugScreen)
            {
                return 0;
            }
            if (profiles.TryGetValue(profileName, out Profile foundProfile))
            {
                return foundProfile.getAverageCompletionTimeMS();
            }
            return -1;
        }

        public static void updateAverages()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            foreach (Profile p in profiles.Values)
            {
                p.updateAverage();
            }
        }

        
        /*private class for profiles.*/
        private class Profile
        {
            private int[] recordedTimes = new int[100];
            private long startTime;
            private int timeSpentInCurrentTick;
            private string name;
            private int updateIndex;
            private int combinedTimePassed;
            public bool hasEnded = true;

            public Profile(string name)
            {
                this.name = name;
            }

            public Profile begin()
            {
                startTime = TicksAndFps.nanoTime();
                hasEnded = false;
                return this;
            }
            
            public void end()
            {
                timeSpentInCurrentTick += (int)(TicksAndFps.nanoTime() - startTime);
                hasEnded = true;
            }

            /// <summary>
            /// should be called at the end of each tick.
            /// </summary>
            public void updateAverage()
            {
                combinedTimePassed -= recordedTimes[updateIndex];
                combinedTimePassed += timeSpentInCurrentTick;
                recordedTimes[updateIndex++] = timeSpentInCurrentTick;
                timeSpentInCurrentTick = 0;
                updateIndex %= 100;//keep update index within 100
            }

            public float getAverageCompletionTimeMS()
            {
                return ((float)combinedTimePassed / 100F) / 1000000F;
            }

            private void printInfo()
            {  
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Profile: \"" + name + "\" measured " + (TicksAndFps.getRealTimeMills() - startTime) + " miliseconds from start to finish.");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
    
}
