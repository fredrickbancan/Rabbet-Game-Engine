using NVorbis;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;

namespace RabbetGameEngine.Sound
{
    public class Sound
    {
        private string soundDir = null;
        private int sampleRate = 0;
        private TimeSpan totalTime;
        private int channels = 0;
        private long totalSamples = 0;
        private int bufferID;
        public Sound(string dir)
        {
            if(dir == "debug")
            {
                byte[]  data = new byte[1000];
                GameInstance.rand.NextBytes(data);
                sampleRate = 11025;
                channels = 1;
                totalTime = TimeSpan.FromSeconds(1);
                bufferID = AL.GenBuffer();
                AL.BufferData(bufferID, ALFormat.Mono16, data, getSampleRate());

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
                reader.ClipSamples = true;
                List<short> data = new List<short>();
                float[] samplesBuffer = new float[totalSamples * channels];
                int count = 0;
                while((count = reader.ReadSamples(samplesBuffer, 0, samplesBuffer.Length)) > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        short temp = (short)(Math.Clamp( 32767f * samplesBuffer[i], -32767f, 32767f));
                        data.Add(temp);
                    }
                }
                reader.Dispose();
                short[] arrData = data.ToArray();
                bufferID = AL.GenBuffer();
                if (isStereo())
                {
                    AL.BufferData(bufferID, ALFormat.Stereo16, arrData, getSampleRate());
                }
                else
                {
                    AL.BufferData(bufferID, ALFormat.Mono16, arrData, getSampleRate());
                }
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

        public int getBufferID()
        {
            return bufferID;
        }

        public TimeSpan getTotalTime()
        {
            return totalTime;
        }

        public bool isStereo()
        {
            return channels >= 2;
        }

        public void delete()
        {
            AL.DeleteBuffer(bufferID);
        }
    }
}
