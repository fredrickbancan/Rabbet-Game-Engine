using System;
using System.Collections.Generic;

namespace Coictus
{
    /*This class will be responsable for debugging, measuring and testing performance of the subsystems in this program.*/
    public static class Profiler
    {
        public static String textRender2DBuildingName = "TextRenderer2D Building";
        public static String gameLoopName = "Game Loop";
        public static String collisionsName = "Collisions";
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

        
        /*private class for profiles.*/
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
                float result =  (float)combinedTimePassed / (float)totalUpdates;

                if(totalUpdates >= 100)//resetting after 100 updates to give a more accurate result
                {
                    totalUpdates = 0;
                    combinedTimePassed = 0;
                }
                return result;
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
