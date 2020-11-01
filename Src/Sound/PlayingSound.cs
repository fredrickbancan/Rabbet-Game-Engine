using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

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
        public Vector3 sndPos;
        public float volume = 0.0F;
        public float maxDist = 0;

        /// <summary>
        /// true if this sound is not positional
        /// </summary>
        public bool isAux = false;
        public PlayingSound(Sound snd, float volume, float speed, long timeStamp)
        {
            theSound = snd;
            timeStampMills = timeStamp;
            soundLengthMills = (long)snd.getTotalTime().TotalMilliseconds;
            bufferID = AL.GenBuffer();
            srcID = AL.GenSource();
            AL.BufferData(bufferID, ALFormat.Stereo16, snd.getData(), snd.isStereo() ? snd.getSampleRate() : snd.getSampleRate() / 2);
            AL.Source(srcID, ALSourcei.Buffer, bufferID);
            AL.Source(srcID, ALSourcef.Gain, volume);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);
            AL.SourcePlay(srcID);
            isAux = true;
        }

        public PlayingSound(Sound snd, float volume, float speed, Vector3 pos, long timeStamp)
        {
            theSound = snd;
            sndPos = pos;
            this.volume = volume;
            this.maxDist = 100 * volume;
            timeStampMills = timeStamp;
            soundLengthMills = (long)snd.getTotalTime().TotalMilliseconds;
            bufferID = AL.GenBuffer();
            srcID = AL.GenSource();
            AL.BufferData(bufferID, ALFormat.Mono16, snd.getData(), snd.getSampleRate());
            AL.Source(srcID, ALSourcei.Buffer, bufferID);
            AL.Source(srcID, ALSourceb.SourceRelative, true);
            AL.Source(srcID, ALSourcef.Pitch, speed);
            AL.Source(srcID, ALSourcei.SourceType, 4136 /*ALSourceType.Static*/);
            EntityPlayer p = GameInstance.get.thePlayer;
            setGainBasedOnDistance(p.getEyePosition());
            setPanBasedOnAngle(p.getEyePosition(), p.getRightVector());
            AL.SourcePlay(srcID);
            isAux = false;
        }

        public void onTick(EntityPlayer listener, long currentMills)
        {
            finishedPlaying = (currentMills - timeStampMills) > (soundLengthMills + extraPlayMills);

            if(!isAux && !finishedPlaying)
            {
                setGainBasedOnDistance(listener.getEyePosition());
                setPanBasedOnAngle(listener.getEyePosition(), listener.getRightVector());
            }
        }

        private void setGainBasedOnDistance(Vector3 lPos)
        {
            float gain = 0;
            float dst = Vector3.Distance(sndPos, lPos);

            if(dst >= maxDist)
            {
                gain = 0.0F;
            }
            else
            {
                dst = 1.0F - dst / maxDist;
                gain = dst * dst;
                //Gain should only be between 1 and 0. 
                gain = MathHelper.Clamp(gain, 0, 1);
                gain *= MathHelper.Clamp(volume, 0, 1);
            }

            AL.Source(srcID, ALSourcef.Gain, gain * GameSettings.masterVolume);
        }


        private void setPanBasedOnAngle(Vector3 lPos, Vector3 lRight)
        {
            Vector3 to = Vector3.Normalize(sndPos - lPos);
            float dot = Vector3.Dot(lRight, to);
            dot *= 0.5F;
            AL.Source(srcID, ALSource3f.Position, dot, 0, -1);
        }

        public void delete()
        {
            AL.SourceStop(srcID);
            AL.DeleteBuffer(bufferID);
            AL.DeleteSource(srcID);
        }
    }
}
