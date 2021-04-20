using System.Collections.Generic;

namespace RabbetGameEngine
{
    /// <summary>
    /// This class is responsable for debugging, measuring and testing performance of the subsystems in this program.
    /// </summary>
    public static class Profiler
    {
        private static Section rootSection = new Section("root", false);
        private static Section rootTickSection = new Section("rootTick", true);

        public static void startRoot()
        {
            rootSection.begin();
        }
        public static void endRoot()
        {
            rootSection.end();
        }
        public static void startTick()
        {
            rootTickSection.begin();
        }
        public static void endTick()
        {
            rootTickSection.end();
        }
        public static void startTickSection(string sectionName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootTickSection.startSection(sectionName, true);
        }
        public static void startSection(string sectionName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.startSection(sectionName, false);
        }
        public static void endCurrentTickSection()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootTickSection.endCurrentSection();
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
            rootSection.startSection(sectionName, false);
        }
        public static void endStartTickSection(string sectionName)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootTickSection.endCurrentSection();
            rootTickSection.startSection(sectionName, true);
        }
        public static void getFrameProfilingData(List<string> lines, List<Color> lineColors)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            lines.Add("Profiler averages [frame times]");
            lineColors.Add(Color.white);
            lines.Add("{");
            lineColors.Add(Color.white);
            rootSection.getProfilingData(lines, lineColors, "");
            lines.Add("}");
            lineColors.Add(Color.white);
        }

        public static void getTickProfilingData(List<string> lines, List<Color> lineColors)
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            lines.Add("Profiler averages [tick times]");
            lineColors.Add(Color.white);
            lines.Add("{");
            lineColors.Add(Color.white);
            rootTickSection.getProfilingData(lines, lineColors, "");
            lines.Add("}");
            lineColors.Add(Color.white);
        }

        public static void onFrame()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootSection.updateAverages();
        }

        public static void onTick()
        {
            if (!GameSettings.debugScreen)
            {
                return;
            }
            rootTickSection.updateAverages();
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
            public bool isTickProfile = true;

            public string sectionName = "";
            public Section(string sectionName, bool tickProfile)
            {
                this.isTickProfile = tickProfile;
                this.sectionName = sectionName;
                this.currentSection = this;
            }
            public void getProfilingData(List<string> lines, List<Color> lineColors, string indentation)
            {
                string subsections = "";
                if (subSections.Count > 0) subsections = "(" + getSubSectionCount() + " sub sections)";
                double avg = getAverageTimeSpentPerTick();
                Color lineCol;

                if (isTickProfile)
                {
                    lineCol = (avg <= 1.0D) ? Color.green : (avg <= 2.5D) ? Color.yellow : Color.red;
                }
                else
                {
                    lineCol = (avg <= 0.5D) ? Color.green : (avg <= 1.25D) ? Color.yellow : Color.red;
                }

                lines.Add(indentation + sectionName + subsections + ": " + avg.ToString("0.0000 ms"));
                lineColors.Add(lineCol);
                foreach (Section s in subSections.Values)
                {
                    s.getProfilingData(lines, lineColors, indentation + "   ");
                }
            }

            public int getSubSectionCount()
            {
                int result = subSections.Count;
                foreach (Section s in subSections.Values)
                {
                    result += s.getSubSectionCount();
                }
                return result;
            }

            public void startSection(string sectionName, bool tickProfile)
            {

                if (currentSection != this)
                {
                    currentSection.startSection(sectionName, tickProfile);
                    return;
                }

                if (subSections.TryGetValue(sectionName, out Section s))
                {
                    currentSection = s.begin();
                    return;
                }
                subSections.Add(sectionName, (currentSection = new Section(sectionName, tickProfile).begin()));
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
                if (currentSection != this)
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
