using System.Collections.Generic;

namespace RabbetGameEngine.Debugging
{
    /// <summary>
    /// This class is responsable for debugging, measuring and testing performance of the subsystems in this program.
    /// </summary>
    public static class Profiler
    {
        private static Section rootSection = new Section("root");

        public static void startRoot()
        {
            rootSection.begin();
        }
        public static void endRoot()
        {
            rootSection.end();
        }

        public static void startSection(string sectionName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.startSection(sectionName);
        }

        public static void endCurrentSection()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.endCurrentSection();
        }

        public static void endStartSection(string sectionName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.endCurrentSection();
            rootSection.startSection(sectionName);
        }
        public static void getFrameProfilingData(List<string> lines)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            lines.Clear();
            lines.Add("Profiler averages [frame times]");
            lines.Add("{");
            rootSection.getProfilingData(lines, ""); 
            lines.Add("}");
        }

        public static void onFrame()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.updateAverages();
        }

        private class Section
        {
            private long[] recordedTimes = new long[100];
            private long startTime;
            private long timeSpentInCurrentTick;
            private int updateIndex;
            private long combinedTimePassed;
            public bool hasEnded = true;
            private Section currentSection = null;
            public Dictionary<string, Section> subSections = new Dictionary<string, Section>();

            public string sectionName = "";
            public Section(string sectionName)
            {
                this.sectionName = sectionName;
                this.currentSection = this;
            }
            public void getProfilingData(List<string> lines, string indentation)
            {
                string subsections = "";
                if (subSections.Count > 0) subsections = "(" + getSubSectionCount() + " sub sections)";
                lines.Add(indentation + sectionName + subsections + ": " + getAverageTimeSpentPerTick().ToString("0.00 ms"));
                foreach(Section s in subSections.Values)
                {
                    s.getProfilingData(lines, indentation + "   ");
                }
            }

            public int getSubSectionCount()
            {
                int result = subSections.Count;
                foreach(Section s in subSections.Values)
                {
                    result += s.getSubSectionCount();
                }
                return result;
            }

            public void startSection(string sectionName)
            {

                if(currentSection!=this)
                {
                    currentSection.startSection(sectionName);
                    return;
                }

                if (subSections.TryGetValue(sectionName, out Section s))
                {
                    currentSection = s.begin();
                    return;
                }
                subSections.Add(sectionName, (currentSection = new Section(sectionName).begin()));
            }

            public Section begin()
            {
                startTime = TicksAndFrames.nanoTime();
                hasEnded = false;
                return this;
            }
            public Section end()
            {
                timeSpentInCurrentTick += TicksAndFrames.nanoTime() - startTime;
                hasEnded = true;
                return this;
            }

            public bool endCurrentSection()
            {
                if(currentSection != this)
                {
                    if (currentSection.endCurrentSection()) currentSection = this;
                    return false;
                }
                end();
                return true;
            }

            public void updateAverages()
            {
                combinedTimePassed -= recordedTimes[updateIndex];
                combinedTimePassed += timeSpentInCurrentTick;
                recordedTimes[updateIndex++] = timeSpentInCurrentTick;
                if (timeSpentInCurrentTick > 100000000L) Application.warn("Somethings taking too long! Section " + sectionName + " took " + ((double)timeSpentInCurrentTick / 1000000D).ToString("0.00 ms") + " to run!");
                timeSpentInCurrentTick = 0;
                updateIndex %= 100;//keep update index within 100
                foreach (Section s in subSections.Values)
                {
                    s.updateAverages();
                }
            }

            public double getAverageTimeSpentPerTick()
            {
                return ((double)combinedTimePassed / 100D) / 1000000D;
            }

        }
    }
    
}
