using NVorbis;
using System;

namespace RabbetGameEngine.Sound
{
    public class Sound
    {
        private string soundDir = null;
        private int sampleRate = 0;
        private TimeSpan totalTime;
        private int channels = 0;
        private long totalSamples = 0;
        private float[] samplesBuffer;
        public Sound(string dir)
        {
            if(dir == "debug")
            {

            }
            else
            {
                soundDir = dir;
                loadOGG(dir);
            }
        }

        private void loadOGG(string dir)
        {
            try
            {
                VorbisReader reader = new VorbisReader(dir);
                channels = reader.Channels;
                sampleRate = reader.SampleRate;
                totalTime = reader.TotalTime;
                totalSamples = reader.TotalSamples;
                samplesBuffer = new float[totalSamples];
                reader.ReadSamples(samplesBuffer, 0, samplesBuffer.Length);
                reader.Dispose();
            }
            catch (Exception e)
            {
                Application.error(e.Message + " | Sound file dir: " + soundDir);
            }
        }

        public int getSampleRate()
        {
            return sampleRate;
        }

        public int getChannelCount()
        {
            return channels;
        }

        public long getTotalSamples()
        {
            return totalSamples;
        }

        public float[] getData()
        {
            return samplesBuffer;
        }

        public TimeSpan getTotalTime()
        {
            return totalTime;
        }


        private void loadDebugSound()
        {

        }

        public void delete()
        {

        }
    }
}
