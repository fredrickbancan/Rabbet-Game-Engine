using OpenTK.Audio.OpenAL;

namespace RabbetGameEngine.Sound
{
    public class PlayingSound
    {
        public static readonly int extraPlayMills = 100;
        public int bufferID = 0;
        public int srcID = 0;
        public Sound theSound = null;
        public bool finishedPlaying = false;
        public long timeStampMills = 0;
        private long soundLengthMills = 0;
        public PlayingSound(int buffer, int src, Sound snd, long timeStamp)
        {
            bufferID = buffer;
            srcID = src;
            theSound = snd;
            timeStampMills = timeStamp;
            soundLengthMills = (long)snd.getTotalTime().TotalMilliseconds;
        }

        public void onTick(long currentMills)
        {
            finishedPlaying = (currentMills - timeStampMills) > (soundLengthMills + extraPlayMills);
        }

        public bool isPlaying()
        {
            return AL.GetSourceState(srcID) == ALSourceState.Playing;
        }

        public void delete()
        {
            AL.SourceStop(srcID);
            AL.DeleteBuffer(bufferID);
            AL.DeleteSource(srcID);
        }
    }
}
