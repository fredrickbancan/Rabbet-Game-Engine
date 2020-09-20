using System;
using System.Collections.Generic;

namespace RabbetGameEngine.Debugging
{
    /*This class will be responsable for debugging, measuring and testing performance of the subsystems in this program.*/
    public static class Profiler
    {
        public static string textRender2DBuildingName = "TextRenderer2D Building";
        public static string gameLoopName = "Game Loop";
        public static string collisionsName = "Collisions";
        public static string renderingName = "Rendering";
        private static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();

        /*Calling this function with a name will either create and begin a profile or end an existing running profile, or run an existing non running profile.
          Apon ending a profile the time difference between starting and ending will be displayed. This allows us to measure how long different things take.*/
        public static void beginEndProfile(string profileName)
        {
            if(!GameSettings.debugScreen)
            {
                return;
            }

            if(profiles.TryGetValue(profileName, out Profile foundProfile))
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

        /*Returns the average completion time in ms for the requested profile. returns -1 if profile isnt found*/
        public static float getAveragesForProfile(string profileName)
        {
            if (!GameSettings.debugScreen)
            {
                return 0;
            }
            if (profiles.TryGetValue(profileName, out Profile foundProfile))
            {
                return foundProfile.getAverageCompletionTime();
            }
            return 0;
        }

        
        /*private class for profiles.*/
        private class Profile
        {
            private int[] recordedTimes = new int[100];
            private long startTime;
            private string name;
            private int updateIndex;
            private int combinedTimePassed;
            public bool hasEnded = false;

            public Profile(string name)
            {
                this.name = name;
            }

            public Profile begin()
            {
                startTime = TicksAndFps.getCurrentTime();
                hasEnded = false;
                return this;
            }
            
            public void end()
            {
                int time = (int)(TicksAndFps.getCurrentTime() - startTime);
                combinedTimePassed -= recordedTimes[updateIndex];
                combinedTimePassed += time;
                recordedTimes[updateIndex++] = time;
                updateIndex %= 100;//keep update index within 100
                hasEnded = true;
            }

            public float getAverageCompletionTime()
            {
                return (float)combinedTimePassed / 100F;
            }

            private void printInfo()
            {  
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Profile: \"" + name + "\" measured " + (TicksAndFps.getCurrentTime() - startTime) + " miliseconds from start to finish.");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
    
}
