using System;
using System.Collections.Generic;

namespace FredrickTechDemo
{
    /*This class will be responsable for debugging, measuring and testing performance of the subsystems in this program.*/
    public static class Profiler
    {
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
        private class Profile
        {
            private long startTime;
            private String name;
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
                printInfo();
                hasEnded = true;
            }

            private void printInfo()
            {  
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Profile: " + name + " measured " + (TicksAndFps.getMiliseconds() - startTime) + " miliseconds from start to finish.");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
    
}
