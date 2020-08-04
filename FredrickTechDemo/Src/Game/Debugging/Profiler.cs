using System;
using System.Collections.Generic;

namespace FredrickTechDemo
{
    /*This class will be responsable for debugging, measuring and testing performance of the subsystems in this program.*/
    public static class Profiler
    {
        public static String textRenderer2DSubmittingName = "TextRenderer2D Submitting";
        public static String gameLoopName = "Game Loop";
        public static String renderingName = "Rendering";
        private static Dictionary<String, Profile> profiles = new Dictionary<String, Profile>();

        /*Calling this function with a name will either create and begin a profile or end an existing running profile, or run an existing non running profile.
          Apon ending a profile the time difference between starting and ending will be displayed. This allows us to measure how long different things take.*/
        public static void beginEndProfile(String profileName)
        {
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
        public static float getAveragesForProfile(String profileName)
        {
            if (profiles.TryGetValue(profileName, out Profile foundProfile))
            {
                return foundProfile.getAverageCompletionTime();
            }
            return -1;
        }

        

        private class Profile
        {
            private long startTime;
            private String name;
            private int totalUpdates;
            private long combinedTimePassed;
            public bool hasEnded = false;

            public Profile(String name)
            {
                this.name = name;
            }

            public Profile begin()
            {
                startTime = TicksAndFps.getMiliseconds();
                hasEnded = false;
                return this;
            }
            
            public void end()
            {
                combinedTimePassed += (TicksAndFps.getMiliseconds() - startTime);
                totalUpdates++;
                hasEnded = true;
            }

            public float getAverageCompletionTime()
            {
                if(totalUpdates < 1)
                {
                    return (float)combinedTimePassed; 
                }
                return (float)combinedTimePassed / (float)totalUpdates;
            }
            private void printInfo()
            {  
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Profile: \"" + name + "\" measured " + (TicksAndFps.getMiliseconds() - startTime) + " miliseconds from start to finish.");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
    
}
